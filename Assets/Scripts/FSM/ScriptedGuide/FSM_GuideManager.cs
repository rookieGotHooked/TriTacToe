using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Device;

public class FSM_GuideManager : MonoBehaviour
{
	private static FSM_GuideManager _instance;
	public static FSM_GuideManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindAnyObjectByType<FSM_GuideManager>();

				if (_instance == null)
				{
					GameObject manager = new GameObject("GuidePagesManager");
					_instance = manager.AddComponent<FSM_GuideManager>();
				}
			}

			return _instance;
		}
	}

	[SerializeField] List<TileObject> _tilesPrefab = new();
	[SerializeField] GameObject _guideTextPrefab;
	[SerializeField] GameObject _backMenuButtonPrefab;
	[SerializeField] GameObject _nextButtonPrefab;
	[SerializeField] GameObject _hintButtonPrefab;
	[SerializeField] GameObject _textboxHintPrefab;
	[SerializeField] GameObject _screenObject;
	[SerializeField] GameObject _scoreDisplayPrefab;
	public GameObject ScreenObject { get => _screenObject; set => _screenObject = value; }

	private Dictionary<TilePositionIndex, GameObject> _tilesObject = new();
	private GameObject _guideTextGameObject;
	private GameObject _backMenuButtonGameObject;
	private GameObject _nextButtonGameObject;
	private GameObject _hintButtonGameObject;
	private GameObject _textboxHintGameObject;
	private GameObject _scoreDisplayGameObject;

	private Dictionary<GuidePageEnum, BaseGuidePage<GuidePageEnum>> _guidePageDict = new();
	private List<Task> _tweenTasks = new();
	private List<BaseGuidePage<GuidePageEnum>> _guidePages = new();

	private GuidePage0 _guidePage0;
	private GuidePage1 _guidePage1;
	private GuidePage2 _guidePage2;
	private GuidePage3 _guidePage3;
	private GuidePage4 _guidePage4;
	private GuidePage5 _guidePage5;

	private List<Tweens2D> _guideTextChildTween;

	private BaseGuidePage<GuidePageEnum> _currentPage;
	private GuidePageEnum _currentPageKey;
	public GuidePageEnum CurrentPageKey { get => _currentPageKey; }
	private GuidePageEnum _nextPageKey;

	private PressOrHoldGuideButton _backMenuButtonComponent;
	private PressOrHoldGuideButton BackMenuButtonComponent => _backMenuButtonComponent;

	private NextButton _nextButtonComponent;
	public NextButton NextButtonComponent => _nextButtonComponent;

	private TextboxButton _hintButtonComponent;

	private ScoreDisplayController _scoreDisplayController;
	public ScoreDisplayController ScoreDisplayController { get => _scoreDisplayController; }

	GuidePageObjects _guidePageObjects;

	private bool _transitioning = false;

	private void InstantiateObjects()
	{
		List<TileObject> tiles = new();

		foreach (var prefab in _tilesPrefab)
		{
			GameObject tile = Instantiate(prefab.TileGameObject, _screenObject.transform);

			tiles.Add(new TileObject(prefab.PositionIndex, tile));
			_tilesObject.Add(prefab.PositionIndex, tile);
			_tweenTasks.Add(AwaitingInitialTransition(tile));
		}

		_guideTextGameObject = Instantiate(_guideTextPrefab, _screenObject.transform);
		_tweenTasks.Add(AwaitingInitialTransition(_guideTextGameObject));

		_hintButtonGameObject = Instantiate(_hintButtonPrefab, _screenObject.transform);
		_tweenTasks.Add(AwaitingInitialTransition(_hintButtonGameObject));

		_textboxHintGameObject = Instantiate(_textboxHintPrefab, _screenObject.transform);
		_tweenTasks.Add(AwaitingInitialTransition(_textboxHintGameObject));

		_backMenuButtonGameObject = Instantiate(_backMenuButtonPrefab, _screenObject.transform);
		_tweenTasks.Add(AwaitingInitialTransition(_backMenuButtonGameObject));

		_nextButtonGameObject = Instantiate(_nextButtonPrefab, _screenObject.transform);
		_tweenTasks.Add(AwaitingInitialTransition(_nextButtonGameObject));

		_scoreDisplayGameObject = Instantiate(_scoreDisplayPrefab, _screenObject.transform);

		_guidePageObjects = new(tiles, _guideTextGameObject, _nextButtonGameObject, _backMenuButtonGameObject);
	}

	async private Task AwaitingInitialTransition(GameObject gameObject)
	{
		if (gameObject.TryGetComponent(out Tweens2D initialTransition))
		{
			while (!initialTransition.IsInit)
			{
				await Task.Yield();
			}
		}

		_nextButtonComponent.SetInteractable(true);
	}

	private void Awake()
	{
		InstantiateObjects();

		_guidePage0 = new GuidePage0(_guidePageObjects, GuidePageEnum.GuidePage0, GuidePageEnum.GuidePage1, GuidePageEnum.Screen);
		_guidePage1 = new GuidePage1(_guidePageObjects, GuidePageEnum.GuidePage1, GuidePageEnum.GuidePage2, GuidePageEnum.GuidePage0);
		_guidePage2 = new GuidePage2(_guidePageObjects, GuidePageEnum.GuidePage2, GuidePageEnum.GuidePage3, GuidePageEnum.GuidePage1);
		_guidePage3 = new GuidePage3(_guidePageObjects, GuidePageEnum.GuidePage3, GuidePageEnum.GuidePage4, GuidePageEnum.GuidePage2);
		_guidePage4 = new GuidePage4(_guidePageObjects, GuidePageEnum.GuidePage4, GuidePageEnum.GuidePage5, GuidePageEnum.GuidePage3);
		_guidePage5 = new GuidePage5(_guidePageObjects, GuidePageEnum.GuidePage5, GuidePageEnum.None, GuidePageEnum.GuidePage4);

		_guidePages.Add(_guidePage0);
		_guidePages.Add(_guidePage1);
		_guidePages.Add(_guidePage2);
		_guidePages.Add(_guidePage3);
		_guidePages.Add(_guidePage4);
		_guidePages.Add(_guidePage5);

		PopulatePageDict();

		if (!_backMenuButtonGameObject.TryGetComponent(out _backMenuButtonComponent))
		{
			throw new Exception($"{_backMenuButtonGameObject.name} does not contains PressOrHoldGuideButton component");
		}

		if (!_nextButtonGameObject.TryGetComponent(out _nextButtonComponent))
		{
			throw new Exception($"{_nextButtonGameObject.name} does not contains NextButton component");
		}

		if (!_hintButtonGameObject.TryGetComponent(out _hintButtonComponent))
		{
			throw new Exception($"{_hintButtonGameObject.name} does not contains TextboxButton component");
		}

		if (!_scoreDisplayGameObject.TryGetComponent(out _scoreDisplayController))
		{
			throw new Exception($"{_scoreDisplayGameObject.name} does not contains ScoreDisplayController component");
		}

		_hintButtonComponent.AssignTextbox(_textboxHintGameObject);

		_guideTextChildTween = new()
		{
			_guideTextGameObject.transform.GetChild(0).gameObject.GetComponent<Tweens2D>(),
			_guideTextGameObject.transform.GetChild(1).gameObject.GetComponent<Tweens2D>(),
			_guideTextGameObject.transform.GetChild(2).gameObject.GetComponent<Tweens2D>(),
			_guideTextGameObject.transform.GetChild(3).gameObject.GetComponent<Tweens2D>(),
			_guideTextGameObject.transform.GetChild(4).gameObject.GetComponent<Tweens2D>(),
			_guideTextGameObject.transform.GetChild(5).gameObject.GetComponent<Tweens2D>(),
		};
	}

	private void Start()
	{
		_currentPage = _guidePageDict[GuidePageEnum.GuidePage0];
		_currentPageKey = _currentPage.CurrentPageKey;
		_nextPageKey = _currentPage.CurrentPageKey;
		_currentPage.OnEnter();
	}

	private void Update()
	{
		if (!_transitioning && _nextPageKey.Equals(_currentPage.CurrentPageKey))
		{
			_currentPage.OnUpdate();
		}
		else if (!_transitioning)
		{
			ChangePage(_nextPageKey);
		}
	}

	async private void ChangePage(GuidePageEnum pageKey)
	{
		_transitioning = true;
		LockButtons();

		await _currentPage.OnExit();

		_currentPage = _guidePageDict[pageKey];
		_currentPageKey = pageKey;

		await _currentPage.OnEnter();

		UnlockButtons();
		_transitioning = false;
	}

	async public void ChangeNextPage()
	{
		LockButtons();
		List<Task> tweens = new();

		if (_currentPageKey == GuidePageEnum.GuidePage0)
		{
			tweens.Add(_backMenuButtonComponent.MoveTextUp());
		}

		_nextPageKey = _currentPage.NextPageKey;

		if (_nextPageKey.Equals(GuidePageEnum.GuidePage5))
		{
			tweens.Add(_nextButtonComponent.MoveOut());
			tweens.Add(_scoreDisplayController.MoveIn());
		}

		foreach (var tween in _guideTextChildTween)
		{
			tweens.Add(tween.ExecuteTweenOrders("Move Right To Left"));
		}

		foreach (var task in tweens)
		{
			await task;
		}

		tweens.Clear();
	}

	async public Task ChangePreviousPage()
	{
		LockButtons();

		List<Task> tweens = new();

		if (_currentPageKey.Equals(GuidePageEnum.GuidePage5))
		{
			tweens.Add(_nextButtonComponent.MoveIn());
			tweens.Add(_scoreDisplayController.MoveOut());
		}

		_nextPageKey = _currentPage.PreviousPageKey;

		if (_nextPageKey.Equals(GuidePageEnum.GuidePage0))
		{
			tweens.Add(_backMenuButtonComponent.MoveTextDown());
		}

		foreach (var tween in _guideTextChildTween)
		{
			tweens.Add(tween.ExecuteTweenOrders("Move Left To Right"));
		}

		foreach (var task in tweens)
		{
			await task;
		}

		_scoreDisplayController.ResetAll();

		tweens.Clear();
	}

	public void LockButtons()
	{
		_backMenuButtonComponent.SetInteractable(false);
		_hintButtonComponent.SetInteractable(false);
		_nextButtonComponent.SetInteractable(false);
	}

	public void UnlockButtons()
	{
		_backMenuButtonComponent.SetInteractable(true);
		_hintButtonComponent.SetInteractable(true);
		_nextButtonComponent.SetInteractable(true);
	}

	private void PopulatePageDict()
	{
		foreach (var page in _guidePages)
		{
			if (!_guidePageDict.TryAdd(page.CurrentPageKey, page))
			{
				throw new Exception($"Cannot add {page} to guide pages dictionary");
			}
		}
	}

	public void SetScreenTransition(ScreensEnum screensEnum)
	{
		_backMenuButtonComponent.SetTransition(screensEnum);
	}
}

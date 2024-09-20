using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MockSymbolSelectController : MonoBehaviour
{
	#region Prefabs

	[Header("Title")]
    [SerializeField] GameObject _titleChevronLeftPrefab;
    [SerializeField] GameObject _titleChevronRightPrefab;
    [SerializeField] GameObject _titleContentLeftPrefab;
	[SerializeField] GameObject _titleContentRightPrefab;

	[Header("Select Buttons")]
    [SerializeField] GameObject _selectBoxBackgroundLeftPrefab;
	[SerializeField] GameObject _selectBoxBackgroundRightPrefab;
	[SerializeField] GameObject _selectButtonPrefab;
	//[SerializeField] GameObject _backgroundSymbolXPrefab;
	//[SerializeField] GameObject _backgroundSymbolOPrefab;

	[Header("Screen Canvas")]
	[SerializeField] GameObject _screenGameObject;

	#endregion


	#region GameObjects

	GameObject _titleChevronLeftGameObject;
	GameObject _titleChevronRightGameObject;
	GameObject _titleContentLeftGameObject;
	GameObject _titleContentRightGameObject;

	GameObject _selectBoxBackgroundLeftGameObject;
	GameObject _selectBoxBackgroundRightGameObject;
	GameObject _selectButtonGameObject;
	GameObject _buttonSymbolXGameObject;
	GameObject _buttonSymbolOGameObject;
	GameObject _backgroundSymbolXGameObject;
	GameObject _backgroundSymbolOGameObject;

	#endregion


	#region Tweens Components

	Tweens2D _titleChevronLeftTween;
    Tweens2D _titleChevronRightTween;
    Tweens2D _titleContentTween;

	Tweens2D _selectBoxBackgroundTween;
	Tweens2D _selectButtonTween;
	Tweens2D _buttonSymbolXTween;
	Tweens2D _buttonSymbolOTween;
	Tweens2D _backgroundSymbolXTween;
	Tweens2D _backgroundSymbolOTween;

	#endregion


	List<Task> _tweenTasks = new();


	async void Start()
	{
		_titleChevronLeftGameObject = Instantiate(_titleChevronLeftPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_titleChevronLeftGameObject));

		_titleChevronRightGameObject = Instantiate(_titleChevronRightPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_titleChevronRightGameObject));

		foreach (var task in _tweenTasks)
		{
			await task;
		}

		_tweenTasks.Clear();
		
		_titleContentLeftGameObject = Instantiate(_titleContentLeftPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_titleContentLeftGameObject));
		
		_titleContentRightGameObject = Instantiate(_titleContentRightPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_titleContentRightGameObject));
		
		_selectBoxBackgroundLeftGameObject = Instantiate(_selectBoxBackgroundLeftPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_selectBoxBackgroundLeftGameObject));
		
		_selectBoxBackgroundRightGameObject = Instantiate(_selectBoxBackgroundRightPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_selectBoxBackgroundRightGameObject));
		
		_backgroundSymbolXGameObject = _selectBoxBackgroundLeftGameObject.transform.GetChild(0).gameObject;
		_tweenTasks.Add(AwaitInitAnimation(_backgroundSymbolXGameObject));
		
		_backgroundSymbolOGameObject = _selectBoxBackgroundLeftGameObject.transform.GetChild(1).gameObject;
		_tweenTasks.Add(AwaitInitAnimation(_backgroundSymbolOGameObject));

		foreach (var task in _tweenTasks)
		{
			await task;
		}

		_tweenTasks.Clear();

		_selectButtonGameObject = Instantiate(_selectButtonPrefab, _screenGameObject.transform);
		_tweenTasks.Add(AwaitInitAnimation(_selectButtonGameObject));

		_buttonSymbolXGameObject = _selectButtonGameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
		_buttonSymbolOGameObject = _selectButtonGameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;

		if (!_titleChevronLeftGameObject.TryGetComponent(out _titleChevronLeftTween))
		{
			throw new System.Exception($"{_titleChevronLeftGameObject.name} does not contains Tweens2D component");
		}

		if (!_titleChevronRightGameObject.TryGetComponent(out _titleChevronRightTween))
		{
			throw new System.Exception($"{_titleChevronRightGameObject.name} does not contains Tweens2D component");
		}

		if (!_titleContentLeftGameObject.TryGetComponent(out _titleContentTween))
		{
			throw new System.Exception($"{_titleContentLeftGameObject.name} does not contains Tweens2D component");
		}

		if (!_titleContentRightGameObject.TryGetComponent(out _titleContentTween))
		{
			throw new System.Exception($"{_titleContentRightGameObject.name} does not contains Tweens2D component");
		}

		if (!_selectBoxBackgroundLeftGameObject.TryGetComponent(out _selectBoxBackgroundTween))
		{
			throw new System.Exception($"{_selectBoxBackgroundLeftGameObject.name} does not contains Tweens2D component");
		}

		if (!_selectBoxBackgroundRightGameObject.TryGetComponent(out _selectBoxBackgroundTween))
		{
			throw new System.Exception($"{_selectBoxBackgroundRightGameObject.name} does not contains Tweens2D component");
		}

		if (!_selectButtonGameObject.TryGetComponent(out _selectButtonTween))
		{
			throw new System.Exception($"{_selectButtonGameObject.name} does not contains Tweens2D component");
		}

		if (!_buttonSymbolXGameObject.TryGetComponent(out _buttonSymbolXTween))
		{
			throw new System.Exception($"{_buttonSymbolXGameObject.name} does not contains Tweens2D component");
		}

		if (!_buttonSymbolOGameObject.TryGetComponent(out _buttonSymbolOTween))
		{
			throw new System.Exception($"{_buttonSymbolOGameObject.name} does not contains Tweens2D component");
		}
	}

    async private Task StartAnimation()
	{
		List<Task> tweenTasks = new()
		{
			//_titleChevronLeftTween.ExecuteTweenOrders
		};

		foreach (var task in tweenTasks)
		{
			await task;
		}
	}

	async private Task AwaitInitAnimation(GameObject gameObject)
	{
		Tweens2D tween = gameObject.GetComponent<Tweens2D>();
		
		if (tween != null)
		{
			while (!tween.IsInit)
			{
				await Task.Yield();
			}
		}
	}
}

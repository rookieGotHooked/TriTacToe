using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitiateState : BaseState<GameStates>
{
	//GameManager _gameManager;
	GameObject _screenObject;
	GameMode _currentGameMode;

	Image _symbolSelectTitleImageComponent;
	Image _difficultySelectTitleImageComponent;

	RectTransform _symbolSelectTitleRectTransformComponent;
	RectTransform _difficultySelectTitleRectTransformComponent;

	Vector2 _symbolSelectTitleOriginalPosition;
	Vector2 _difficultySelectTitleOriginalPosition;

	SymbolSelectController _symbolSelectController;
	DifficultySelectController _difficultySelectController;
	ConfirmAIConfigsButtonController _confirmAIConfigsButtonController;


	Dictionary<TilePositionIndex, GameObject> _tileGameObjectDict = new();
	Dictionary<TilePositionIndex, TileController> _tileControllerDict = new();
	Dictionary<TilePositionIndex, Symbol> _symbolDict = new();

	GameObject _turnDisplayGameObject;
	GameObject _xScoreDisplayGameObject;
	GameObject _oScoreDisplayGameObject;
	GameObject _victoryBannerGameObject;
	GameObject _resultScoreXGameObject;
	GameObject _resultScoreOGameObject;
	GameObject _quitButtonGameObject;
	GameObject _restartButtonGameObject;
	GameObject _suddenDeathBannerGameObject;
	GameObject _confirmSuddenDeathHintButtonGameObject;
	GameObject _suddenDeathHintDisplayButtonGameObject;
	GameObject _blurLayer;
	GameObject _symbolTitle;
	GameObject _symbolBar;
	GameObject _difficultyTitle;
	GameObject _difficultyBar;
	GameObject _confirmButton;

	public InitiateState(GameStates state) : base(state)
	{
	}

	private List<Task> _tweenTasks = new();
	private bool _isInit = false;
	private bool _isPvPInit = false;
	private bool _isAIInit = false;

	public override void OnEnter()
	{
		//Debug.Log("Enter InitiateState");

		_gameManager = GameManager.Instance;
		_screenObject = _gameManager.ScreenObject;
		_currentGameMode = _gameManager.CurrentGameMode;

		_nextStateKey = GameStates.PlayerAction;

		OnUpdate();
	}

	//public override Task OnEnter()
	//{

	//}

	public override async void OnUpdate()
	{
		if (!_isInit)
		{
			foreach (var tileObject in _gameManager.GameplayTiles)
			{
				GameObject newTileObject = Object.Instantiate(tileObject.TileGameObject, _screenObject.transform);

				TileController tileController;
				Symbol symbol;

				if (!_tileGameObjectDict.TryAdd(tileObject.PositionIndex, newTileObject))
				{
					throw new System.Exception($"Cannot add item to _tileGameObjectDict; values are: {tileObject.PositionIndex}, {newTileObject}");
				}

				if (!newTileObject.TryGetComponent(out tileController))
				{
					throw new System.Exception($"{newTileObject.name} does not contains TileController component");
				}
				else
				{
					if (!_tileControllerDict.TryAdd(tileObject.PositionIndex, tileController))
					{
						throw new System.Exception($"Cannot add item to _tileControllerDict; values are: {tileObject.PositionIndex}, {tileController}");
					}
					else
					{
						symbol = Symbol.None;

						if (!_symbolDict.TryAdd(tileObject.PositionIndex, symbol))
						{
							throw new System.Exception($"Cannot add item to _symbolDict; values are: {tileObject.PositionIndex}, {symbol}");
						}
					}
				}
			}

			_turnDisplayGameObject = Object.Instantiate(_gameManager.TurnDisplayPrefab, _screenObject.transform);
			_tweenTasks.Add(AwaitingInitialTransition(_turnDisplayGameObject));

			_xScoreDisplayGameObject = Object.Instantiate(_gameManager.XScoreDisplayPrefab, _screenObject.transform);
			_tweenTasks.Add(AwaitingInitialTransition(_xScoreDisplayGameObject));

			_oScoreDisplayGameObject = Object.Instantiate(_gameManager.OScoreDisplayPrefab, _screenObject.transform);
			_tweenTasks.Add(AwaitingInitialTransition(_oScoreDisplayGameObject));

			_victoryBannerGameObject = Object.Instantiate(_gameManager.VictoryBannerPrefab, _screenObject.transform);
			_tweenTasks.Add(AwaitingInitialTransition(_victoryBannerGameObject));

			_resultScoreXGameObject = Object.Instantiate(_gameManager.ResultScoreXPrefab, _screenObject.transform);
			_resultScoreOGameObject = Object.Instantiate(_gameManager.ResultScoreOPrefab, _screenObject.transform);
			_quitButtonGameObject = Object.Instantiate(_gameManager.QuitButtonPrefab, _screenObject.transform);
			_restartButtonGameObject = Object.Instantiate(_gameManager.RestartButtonPrefab, _screenObject.transform);

			_suddenDeathHintDisplayButtonGameObject = Object.Instantiate(_gameManager.SuddenDeathHintDisplayButtonPrefab, _screenObject.transform);
			_blurLayer = Object.Instantiate(_gameManager.GameplayBlurLayerPrefab, _screenObject.transform);

			_suddenDeathBannerGameObject = Object.Instantiate(_gameManager.SuddenDeathBannerPrefab, _screenObject.transform);
			_confirmSuddenDeathHintButtonGameObject = Object.Instantiate(_gameManager.ConfirmSuddenDeathHintButtonPrefab, _screenObject.transform);


			foreach (var task in _tweenTasks)
			{
				await task;
			}

			_tweenTasks.Clear();

			_isInit = true;
 		}

		if (_currentGameMode == GameMode.Local && !_isPvPInit)
		{
			_blurLayer.SetActive(false);

			_gameManager.AssignObjectsLocalMode(
				_turnDisplayGameObject,
				_xScoreDisplayGameObject,
				_oScoreDisplayGameObject,
				_victoryBannerGameObject,
				_tileGameObjectDict,
				_tileControllerDict,
				_symbolDict,
				_resultScoreXGameObject,
				_resultScoreOGameObject,
				_quitButtonGameObject,
				_restartButtonGameObject,
				_suddenDeathBannerGameObject,
				_blurLayer,
				_confirmSuddenDeathHintButtonGameObject,
				_suddenDeathHintDisplayButtonGameObject);

			_isPvPInit = true;

			OnExit();

		}
		else if (_currentGameMode == GameMode.VersusAI && !_isAIInit)
		{
			_symbolTitle = Object.Instantiate(_gameManager.SymbolSelectTitlePrefab, _screenObject.transform);

			if (!_symbolTitle.TryGetComponent(out _symbolSelectTitleImageComponent))
			{
				throw new System.Exception($"{_symbolTitle.name} does not contains Image component");
			}

			if (!_symbolTitle.TryGetComponent(out _symbolSelectTitleRectTransformComponent))
			{
				throw new System.Exception($"{_symbolTitle.name} does not contains RectTransform component");
			}

			_symbolSelectTitleOriginalPosition = _symbolSelectTitleRectTransformComponent.anchoredPosition;


			_symbolBar = Object.Instantiate(_gameManager.SymbolSelectBarPrefab, _screenObject.transform);

			if (!_symbolBar.TryGetComponent(out _symbolSelectController))
			{
				throw new System.Exception($"{_symbolBar.name} does not contains SymbolSelectController component");
			}


			_difficultyTitle = Object.Instantiate(_gameManager.DifficultylSelectTitlePrefab, _screenObject.transform);

			if (!_difficultyTitle.TryGetComponent(out _difficultySelectTitleImageComponent))
			{
				throw new System.Exception($"{_difficultyTitle.name} does not contains Image component");
			}

			if (!_difficultyTitle.TryGetComponent(out _difficultySelectTitleRectTransformComponent))
			{
				throw new System.Exception($"{_difficultyTitle.name} does not contains RectTransform component");
			}

			_difficultySelectTitleOriginalPosition = _difficultySelectTitleRectTransformComponent.anchoredPosition;


			_difficultyBar = Object.Instantiate(_gameManager.DifficultylSelectBarPrefab, _screenObject.transform);

			if (!_difficultyBar.TryGetComponent(out _difficultySelectController))
			{
				throw new System.Exception($"{_difficultyBar.name} does not contains DifficultySelectController component");
			}

			_confirmButton = Object.Instantiate(_gameManager.AIConfigsConfirmButtonPrefab, _screenObject.transform);

			if (!_confirmButton.TryGetComponent(out _confirmAIConfigsButtonController))
			{
				throw new System.Exception($"{_confirmButton.name} does not contains DifficultySelectController component");
			}

			//Debug.Log($"_blurLayer: {_blurLayer}");

			_gameManager.AssignObjectsAIMode(
				_turnDisplayGameObject,
				_xScoreDisplayGameObject,
				_oScoreDisplayGameObject,
				_victoryBannerGameObject,
				_tileGameObjectDict,
				_tileControllerDict,
				_symbolDict,
				_resultScoreXGameObject,
				_resultScoreOGameObject,
				_quitButtonGameObject,
				_restartButtonGameObject,
				_suddenDeathBannerGameObject,
				_confirmSuddenDeathHintButtonGameObject,
				_suddenDeathHintDisplayButtonGameObject,
				_blurLayer,
				_symbolTitle,
				_symbolBar,
				_difficultyTitle,
				_difficultyBar,
				_confirmButton);


			//_isInit = true;
			_isAIInit = false;
		}

		if (_gameManager.CurrentGameMode == GameMode.VersusAI)
		{
			_gameManager.GameplayBlurLayerGameObject.SetActive(true);

			_symbolSelectTitleImageComponent.color = new(
				_symbolSelectTitleImageComponent.color.r,
				_symbolSelectTitleImageComponent.color.g,
				_symbolSelectTitleImageComponent.color.b,
				255f);
			_symbolSelectController.SetTransparencyAll(255f);

			_difficultySelectTitleImageComponent.color = new(
				_difficultySelectTitleImageComponent.color.r,
				_difficultySelectTitleImageComponent.color.g,
				_difficultySelectTitleImageComponent.color.b,
				255f);
			_difficultySelectController.SetTransparencyAll(255f);

			_confirmAIConfigsButtonController.SetTransparent(255f);


			List<Task> tasks = new()
			{
				_gameManager.GameplayBlurLayerTweenComponent.ExecuteTweenOrders("Appear"),

				_gameManager.SymbolSelectTitleTweenComponent.ExecuteTweenOrders("Move Left To Right"),
				_gameManager.SymbolSelectBarTweenComponent.ExecuteTweenOrders("Move Right To Left"),

				_gameManager.DifficultylSelectTitleTweenComponent.ExecuteTweenOrders("Move Left To Right"),
				_gameManager.DifficultylSelectBarTweenComponent.ExecuteTweenOrders("Move Right To Left"),

				_gameManager.AIConfigsConfirmButtonTweenComponent.ExecuteTweenOrders("Move Left To Right"),
			};

			foreach (var task in tasks)
			{
				await task;
			}
		}
	}

	async public void SetGameConfigs()
	{
		List<Task> tasks = new()
		{
			_gameManager.GameplayBlurLayerTweenComponent.ExecuteTweenOrders("Disappear"),

			_gameManager.SymbolSelectTitleTweenComponent.ExecuteTweenOrders("Move Left To Right"),
			_gameManager.SymbolSelectBarTweenComponent.ExecuteTweenOrders("Move Right To Left"),

			_gameManager.DifficultylSelectTitleTweenComponent.ExecuteTweenOrders("Move Left To Right"),
			_gameManager.DifficultylSelectBarTweenComponent.ExecuteTweenOrders("Move Right To Left"),

			_gameManager.AIConfigsConfirmButtonTweenComponent.ExecuteTweenOrders("Move Left To Right"),
		};

		foreach (var task in tasks)
		{
			await task;
		}

		_gameManager.GameplayBlurLayerGameObject.SetActive(false);
		_gameManager.GetAIConfigs();

		_symbolSelectTitleImageComponent.color = new(
			_symbolSelectTitleImageComponent.color.r,
			_symbolSelectTitleImageComponent.color.g,
			_symbolSelectTitleImageComponent.color.b,
			0f);
		_symbolSelectController.SetTransparencyAll(0f);

		_difficultySelectTitleImageComponent.color = new(
			_difficultySelectTitleImageComponent.color.r,
			_difficultySelectTitleImageComponent.color.g,
			_difficultySelectTitleImageComponent.color.b,
			0f);
		_difficultySelectController.SetTransparencyAll(0f);
		_confirmAIConfigsButtonController.SetTransparent(0f);

		_symbolSelectTitleRectTransformComponent.anchoredPosition = _symbolSelectTitleOriginalPosition;
		_symbolSelectController.ResetPosition();

		_difficultySelectTitleRectTransformComponent.anchoredPosition = _difficultySelectTitleOriginalPosition; 
		_difficultySelectController.ResetPosition();

		_confirmAIConfigsButtonController.ResetPosition();

		OnExit();
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
	}

	public override void OnExit()
	{
		_gameManager.SetAllTilesInteractable(true);
		//_gameManager.ChangeState();

		if (_currentGameMode == GameMode.Local)
		{
			_gameManager.GameStateDict[GameStates.PlayerAction].OnEnter();
		}
		else
		{
			if (_gameManager.PlayerSymbol == Symbol.X)
			{
				_gameManager.GameStateDict[GameStates.PlayerAction].OnEnter();
			}
			else if (_gameManager.PlayerSymbol == Symbol.O)
			{
				_gameManager.GameStateDict[GameStates.AIAction].OnEnter();
			}
			else
			{
				throw new System.Exception($"Invalid player symbol received: {_gameManager.PlayerSymbol}");
			}
		}
	}
}

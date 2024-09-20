using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private bool _isShowingSuddenDeathHint = false;
	public bool IsShowingSuddenDeathHint => _isShowingSuddenDeathHint;

	public static GameManager Instance { get; private set; }

	Symbol _currentSymbol = Symbol.X;
	public Symbol CurrentSymbol => _currentSymbol;

	LastActionTakenBy _lastAction;
	public LastActionTakenBy LastAction => _lastAction;

	Symbol _playerSymbol;
	public Symbol PlayerSymbol => _playerSymbol;

	AIDifficulties _difficulty;
	public AIDifficulties Difficulty => _difficulty;

	private GameMode _currentGameMode;
	public GameMode CurrentGameMode => _currentGameMode;

	TilePositionIndex _lastMarkedTile;
	public TilePositionIndex LastMarkedTile
	{
		get
		{
			return _lastMarkedTile;
		}
		set
		{
			_lastMarkedTile = value switch
			{
				TilePositionIndex.Tile00 => TilePositionIndex.Tile00,
				TilePositionIndex.Tile01 => TilePositionIndex.Tile01,
				TilePositionIndex.Tile02 => TilePositionIndex.Tile02,
				TilePositionIndex.Tile10 => TilePositionIndex.Tile10,
				TilePositionIndex.Tile11 => TilePositionIndex.Tile11,
				TilePositionIndex.Tile12 => TilePositionIndex.Tile12,
				TilePositionIndex.Tile20 => TilePositionIndex.Tile20,
				TilePositionIndex.Tile21 => TilePositionIndex.Tile21,
				TilePositionIndex.Tile22 => TilePositionIndex.Tile22,

				_ => throw new System.Exception($"Unexpected tile index received: {value}")
			};
		}
	}

	private bool _isGameEnd = false;
	public bool IsGameEnd { get { return _isGameEnd; } set {_isGameEnd = value; } }

	#region AI Configs

	[SerializeField] GameObject _gameplayBlurLayerPrefab;
	public GameObject GameplayBlurLayerPrefab => _gameplayBlurLayerPrefab;

	GameObject _gameplayBlurLayerGameObject;
	public GameObject GameplayBlurLayerGameObject => _gameplayBlurLayerGameObject;

	Tweens2D _gameplayBlurLayerTweenComponent;

	public Tweens2D GameplayBlurLayerTweenComponent => _gameplayBlurLayerTweenComponent;

	[SerializeField] GameObject _symbolSelectTitlePrefab;
	public GameObject SymbolSelectTitlePrefab => _symbolSelectTitlePrefab;

	GameObject _symbolSelectTitleGameObject;
	public GameObject SymbolSelectTitleGameObject => _symbolSelectTitleGameObject;

	Tweens2D _symbolSelectTitleTweenComponent;
	public Tweens2D SymbolSelectTitleTweenComponent => _symbolSelectTitleTweenComponent;


	[SerializeField] GameObject _symbolSelectBarPrefab;
	public GameObject SymbolSelectBarPrefab => _symbolSelectBarPrefab;

	GameObject _symbolSelectBarGameObject;
	public GameObject SymbolSelectBarGameObject => _symbolSelectBarGameObject;

	Tweens2D _symbolSelectBarTweenComponent;
	public Tweens2D SymbolSelectBarTweenComponent => _symbolSelectBarTweenComponent;

	SymbolSelectController _symbolSelectController;
	public SymbolSelectController SymbolSelectController => _symbolSelectController;


	[SerializeField] GameObject _difficultylSelectTitlePrefab;
	public GameObject DifficultylSelectTitlePrefab => _difficultylSelectTitlePrefab;

	GameObject _difficultylSelectTitleGameObject;
	public GameObject DifficultylSelectTitleGameObject => _difficultylSelectTitleGameObject;

	Tweens2D _difficultylSelectTitleTweenComponent;
	public Tweens2D DifficultylSelectTitleTweenComponent => _difficultylSelectTitleTweenComponent;


	[SerializeField] GameObject _difficultylSelectBarPrefab;
	public GameObject DifficultylSelectBarPrefab => _difficultylSelectBarPrefab;

	GameObject _difficultylSelectBarGameObject;
	public GameObject DifficultylSelectBarGameObject => _difficultylSelectBarGameObject;

	Tweens2D _difficultylSelectBarTweenComponent;
	public Tweens2D DifficultylSelectBarTweenComponent => _difficultylSelectBarTweenComponent;

	DifficultySelectController _difficultySelectController;
	public DifficultySelectController DifficultySelectController => _difficultySelectController;


	[SerializeField] GameObject _AIConfigsConfirmButtonPrefab;
	public GameObject AIConfigsConfirmButtonPrefab => _AIConfigsConfirmButtonPrefab;

	GameObject _aiConfigsConfirmButtonGameObject;
	public GameObject AIConfigsConfirmButtonGameObject => _aiConfigsConfirmButtonGameObject;

	Tweens2D _aiConfigsConfirmButtonTweenComponent;
	public Tweens2D AIConfigsConfirmButtonTweenComponent => _aiConfigsConfirmButtonTweenComponent;

	ConfirmAIConfigsButtonController _confirmAIConfigsButtonController;
	public ConfirmAIConfigsButtonController ConfirmAIConfigsButtonController => _confirmAIConfigsButtonController;

	#endregion


	#region Result Score X

	[SerializeField] GameObject _resultScoreXPrefab;
	public GameObject ResultScoreXPrefab => _resultScoreXPrefab;

	GameObject _resultScoreXGameObject;
	public GameObject ResultScoreXGameObject => _resultScoreXGameObject;

	private ResultScoreController _resultScoreXController;
	public ResultScoreController ResultScoreXController => _resultScoreXController;

	#endregion


	#region Result Score O

	[SerializeField] GameObject _resultScoreOPrefab;
	public GameObject ResultScoreOPrefab => _resultScoreOPrefab;

	GameObject _resultScoreOGameObject;
	public GameObject ResultScoreOGameObject => _resultScoreOGameObject;

	private ResultScoreController _resultScoreOController;
	public ResultScoreController ResultScoreOController => _resultScoreOController;

	#endregion


	#region Quit Button

	[SerializeField] GameObject _quitButtonPrefab;
	public GameObject QuitButtonPrefab => _quitButtonPrefab;

	GameObject _quitButtonGameObject;
	public GameObject QuitButtonGameObject => _quitButtonGameObject;

	private QuitButtonController _quitButtonController;
	public QuitButtonController QuitButtonController => _quitButtonController;

	#endregion


	#region Restart Button

	[SerializeField] GameObject _restartButtonPrefab;
	public GameObject RestartButtonPrefab => _restartButtonPrefab;

	GameObject _rematchButtonGameObject;
	public GameObject RematchButtonGameObject => _rematchButtonGameObject;

	private RematchButtonController _rematchButtonController;
	public RematchButtonController RematchButtonController => _rematchButtonController;

	#endregion


	#region Turn Display

	TurnDisplayController _turnDisplayController;
	public TurnDisplayController TurnDisplayController => _turnDisplayController;

	[SerializeField] List<TileObject> _gameplayTiles;
    public List<TileObject> GameplayTiles { get => _gameplayTiles; }

	[SerializeField] GameObject _turnDisplayPrefab;
	public GameObject TurnDisplayPrefab => _turnDisplayPrefab;

	private GameObject _turnDisplayGameObject;
	public GameObject TurnDisplayGameObject => _turnDisplayGameObject;

	#endregion


	#region X Score Display

	[SerializeField] GameObject _xScoreDisplayPrefab;
	public GameObject XScoreDisplayPrefab => _xScoreDisplayPrefab;

	private GameObject _xScoreDisplayGameObject;
	public GameObject XScoreDisplayGameObject => _xScoreDisplayGameObject;

	private ScoreDisplayController _scoreXDisplayController;
	public ScoreDisplayController ScoreXDisplayController => _scoreXDisplayController;

	#endregion


	#region Sudden Death Display

	[SerializeField] private GameObject _suddenDeathBannerPrefab;
	public GameObject SuddenDeathBannerPrefab => _suddenDeathBannerPrefab;

	private GameObject _suddenDeathBannerGameObject;
	public GameObject SuddenDeathBannerGameObject => _suddenDeathBannerGameObject;

	private SuddenDeathBannerController _suddenDeathBannerController;
	public SuddenDeathBannerController SuddenDeathBannerController => _suddenDeathBannerController;


	[SerializeField] private GameObject _confirmSuddenDeathHintButtonPrefab;
	public GameObject ConfirmSuddenDeathHintButtonPrefab => _confirmSuddenDeathHintButtonPrefab;

	private GameObject _confirmSuddenDeathHintButtonGameObject;
	public GameObject ConfirmSuddenDeathHintButtonGameObject => _confirmSuddenDeathHintButtonGameObject;

	private ConfirmSuddenDeathBannerHintController _confirmSuddenDeathBannerHintButtonController;
	public ConfirmSuddenDeathBannerHintController ConfirmSuddenDeathBannerHintButtonController => _confirmSuddenDeathBannerHintButtonController;


	[SerializeField] private GameObject _suddenDeathHintDisplayButtonPrefab;
	public GameObject SuddenDeathHintDisplayButtonPrefab => _suddenDeathHintDisplayButtonPrefab;

	private GameObject _suddenDeathHintDisplayButtonGameObject;
	public GameObject SuddenDeathHintDisplayButtonGameObject => _suddenDeathHintDisplayButtonGameObject;

	private ShowSuddenDeathHintButtonController _suddenDeathHintDisplayButtonController;
	public ShowSuddenDeathHintButtonController SuddenDeathHintDisplayButtonController => _suddenDeathHintDisplayButtonController;


	#endregion

	#region O Score Display

	[SerializeField] GameObject _oScoreDisplayPrefab;
	public GameObject OScoreDisplayPrefab => _oScoreDisplayPrefab;

	private GameObject _oScoreDisplayGameObject;
	public GameObject OScoreDisplayGameObject => _oScoreDisplayGameObject;

	private ScoreDisplayController _scoreODisplayController;
	public ScoreDisplayController ScoreODisplayController => _scoreODisplayController;

	#endregion


	#region Tile Controllers

	Dictionary<TilePositionIndex, GameObject> _tileGameObjectDict;
	private Dictionary<TilePositionIndex, TileController> _tileControllerDict;
	public Dictionary<TilePositionIndex, TileController> TileControllerDict => _tileControllerDict;

	#endregion


	#region Symbol

	private Dictionary<TilePositionIndex, Symbol> _symbolDict;
    public Dictionary<TilePositionIndex, Symbol> SymbolDict => _symbolDict;

	#endregion


	#region Scores

	private int _xScore = 0; 
	public int XScore => _xScore;

	private int _oScore = 0;
	public int OScore => _oScore;

	#endregion


	#region Screen GameObject

	[SerializeField] GameObject _screenObject;
    public GameObject ScreenObject { get => _screenObject; set => _screenObject = value; }

	#endregion


	#region States

	BaseState<GameStates> _currentState;
	Dictionary<GameStates, BaseState<GameStates>> _gameStateDict = new();
	public Dictionary<GameStates, BaseState<GameStates>> GameStateDict => _gameStateDict;

	InitiateState _initiateState;
	public InitiateState InitiateState => _initiateState;

	PlayerActionState _playerActionState;
	public PlayerActionState PlayerActionState => _playerActionState;

	AIActionState _aiActionState;
	public AIActionState AIActionState => _aiActionState;

	TurnFinalizeState _turnFinalizeState;
	public TurnFinalizeState TurnFinalizeState => _turnFinalizeState;

	EndGameState _endGameState;
	public EndGameState EndGameState => _endGameState;

	#endregion


	#region Victory Banner

	[SerializeField] GameObject _victoryBannerPrefab;
	public GameObject VictoryBannerPrefab => _victoryBannerPrefab;

	GameObject _victoryBannerGameObject;
	public GameObject VictoryBannerGameObject => _victoryBannerGameObject;

	private VictoryBannerController _victoryBannerController;
	public VictoryBannerController VictoryBannerController => _victoryBannerController;

	#endregion


	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}

		Instance = this;

		_currentGameMode = ScreenManager.Instance.CurrentGameMode;

		_initiateState = new (GameStates.Initiate);
		_playerActionState = new (GameStates.PlayerAction);
		_turnFinalizeState = new (GameStates.TurnFinalize);
		_endGameState = new (GameStates.EndGame);
		_aiActionState = new(GameStates.AIAction);

		_gameStateDict.Add(GameStates.Initiate, _initiateState);
		_gameStateDict.Add(GameStates.PlayerAction, _playerActionState);
		_gameStateDict.Add(GameStates.TurnFinalize, _turnFinalizeState);
		_gameStateDict.Add(GameStates.EndGame, _endGameState);
		_gameStateDict.Add(GameStates.AIAction, _aiActionState);
	}
	private void Start()
	{
		_currentState = _gameStateDict[GameStates.Initiate];
		_currentState.OnEnter();
	}

	public void AssignObjectsLocalMode(
		GameObject turnDisplayGameObject,
		GameObject xScoreDisplayGameObject,
		GameObject oScoreDisplayGameObject,
		GameObject victoryBannerGameObject,
		Dictionary<TilePositionIndex, GameObject> tileGameObjectDict,
		Dictionary<TilePositionIndex, TileController> tileControllerDict,
		Dictionary<TilePositionIndex, Symbol> symbolDict,
		GameObject resultScoreXGameObject,
		GameObject resultScoreOGameObject,
		GameObject quitButtonGameObject,
		GameObject restartButtonGameObject,
		GameObject suddenDeathBannerGameObject,
		GameObject blurLayer,
		GameObject confirmSuddenDeathHintButtonGameObject,
		GameObject suddenDeathHintDisplayButtonGameObject)
	{
		_turnDisplayGameObject = turnDisplayGameObject;

		if (!_turnDisplayGameObject.TryGetComponent(out _turnDisplayController))
		{
			throw new System.Exception($"{_turnDisplayGameObject} does not contains TurnDisplayController component");
		}

		_xScoreDisplayGameObject = xScoreDisplayGameObject;
		
		if (!_xScoreDisplayGameObject.TryGetComponent(out _scoreXDisplayController))
		{
			throw new System.Exception($"{_xScoreDisplayGameObject.name} does not contains ScoreDisplayController component");
		}

		_oScoreDisplayGameObject = oScoreDisplayGameObject;

		if (!_oScoreDisplayGameObject.TryGetComponent(out _scoreODisplayController))
		{
			throw new System.Exception($"{_oScoreDisplayGameObject.name} does not contains ScoreDisplayController component");
		}

		_victoryBannerGameObject = victoryBannerGameObject;

		if (!_victoryBannerGameObject.TryGetComponent(out _victoryBannerController))
		{
			throw new System.Exception($"{_victoryBannerGameObject.name} does not contains VictoryBannerController component");
		}

		_resultScoreXGameObject = resultScoreXGameObject;
		
		if (!_resultScoreXGameObject.TryGetComponent(out _resultScoreXController))
		{
			throw new System.Exception($"{_resultScoreXGameObject.name} does not contains ResultScoreController component");
		}

		_resultScoreOGameObject = resultScoreOGameObject;

		if (!_resultScoreOGameObject.TryGetComponent(out _resultScoreOController))
		{
			throw new System.Exception($"{_resultScoreOGameObject.name} does not contains ResultScoreController component");
		}

		_quitButtonGameObject = quitButtonGameObject;

		if (!_quitButtonGameObject.TryGetComponent(out _quitButtonController))
		{
			throw new System.Exception($"{_quitButtonGameObject.name} does not contains QuitButtonController component");
		}

		_rematchButtonGameObject = restartButtonGameObject;

		if (!_rematchButtonGameObject.TryGetComponent(out _rematchButtonController))
		{
			throw new System.Exception($"{_rematchButtonGameObject.name} does not contains RematchButtonController component");
		}

		_tileGameObjectDict = tileGameObjectDict;
		_tileControllerDict = tileControllerDict;
		_symbolDict = symbolDict;

		_suddenDeathBannerGameObject = suddenDeathBannerGameObject;

		if (!_suddenDeathBannerGameObject.TryGetComponent(out _suddenDeathBannerController))
		{
			throw new System.Exception($"{_suddenDeathBannerGameObject.name} does not contains SuddenDeathBannerController component");
		}

		_gameplayBlurLayerGameObject = blurLayer;

		if (!_gameplayBlurLayerGameObject.TryGetComponent(out _gameplayBlurLayerTweenComponent))
		{
			throw new System.Exception($"{_gameplayBlurLayerGameObject.name} does not contains Tweens2D component");
		}

		_confirmSuddenDeathHintButtonGameObject = confirmSuddenDeathHintButtonGameObject;

		if (!_confirmSuddenDeathHintButtonGameObject.TryGetComponent(out _confirmSuddenDeathBannerHintButtonController))
		{
			throw new System.Exception($"{_confirmSuddenDeathHintButtonGameObject.name} does not contains ConfirmSuddenDeathBannerHintButtonController component");
		}

		_suddenDeathHintDisplayButtonGameObject = suddenDeathHintDisplayButtonGameObject;

		if (!_suddenDeathHintDisplayButtonGameObject.TryGetComponent(out _suddenDeathHintDisplayButtonController))
		{
			throw new System.Exception($"{_suddenDeathHintDisplayButtonController.name} does not contains ShowSuddenDeathHintButtonController component");
		}
	}

	public void AssignObjectsAIMode(
		GameObject turnDisplayGameObject,
		GameObject xScoreDisplayGameObject,
		GameObject oScoreDisplayGameObject,
		GameObject victoryBannerGameObject,
		Dictionary<TilePositionIndex, GameObject> tileGameObjectDict,
		Dictionary<TilePositionIndex, TileController> tileControllerDict,
		Dictionary<TilePositionIndex, Symbol> symbolDict,
		GameObject resultScoreXGameObject,
		GameObject resultScoreOGameObject,
		GameObject quitButtonGameObject,
		GameObject restartButtonGameObject,
		GameObject suddenDeathBannerGameObject,
		GameObject confirmSuddenDeathHintButtonGameObject,
		GameObject suddenDeathHintDisplayButtonGameObject,
		GameObject blurLayerGameObject,
		GameObject symbolSelectTitleGameObject,
		GameObject symbolSelectBarGameObject,
		GameObject difficultyTitleGameObject,
		GameObject difficultyBarGameObject,
		GameObject confirmAiConfigsButtonGameObject)
	{
		_turnDisplayGameObject = turnDisplayGameObject;

		if (!_turnDisplayGameObject.TryGetComponent(out _turnDisplayController))
		{
			throw new System.Exception($"{_turnDisplayGameObject} does not contains TurnDisplayController component");
		}

		_xScoreDisplayGameObject = xScoreDisplayGameObject;

		if (!_xScoreDisplayGameObject.TryGetComponent(out _scoreXDisplayController))
		{
			throw new System.Exception($"{_xScoreDisplayGameObject.name} does not contains ScoreDisplayController component");
		}

		_oScoreDisplayGameObject = oScoreDisplayGameObject;

		if (!_oScoreDisplayGameObject.TryGetComponent(out _scoreODisplayController))
		{
			throw new System.Exception($"{_oScoreDisplayGameObject.name} does not contains ScoreDisplayController component");
		}

		_victoryBannerGameObject = victoryBannerGameObject;

		if (!_victoryBannerGameObject.TryGetComponent(out _victoryBannerController))
		{
			throw new System.Exception($"{_victoryBannerGameObject.name} does not contains VictoryBannerController component");
		}

		_resultScoreXGameObject = resultScoreXGameObject;

		if (!_resultScoreXGameObject.TryGetComponent(out _resultScoreXController))
		{
			throw new System.Exception($"{_resultScoreXGameObject.name} does not contains ResultScoreController component");
		}

		_resultScoreOGameObject = resultScoreOGameObject;

		if (!_resultScoreOGameObject.TryGetComponent(out _resultScoreOController))
		{
			throw new System.Exception($"{_resultScoreOGameObject.name} does not contains ResultScoreController component");
		}

		_quitButtonGameObject = quitButtonGameObject;

		if (!_quitButtonGameObject.TryGetComponent(out _quitButtonController))
		{
			throw new System.Exception($"{_quitButtonGameObject.name} does not contains QuitButtonController component");
		}

		_rematchButtonGameObject = restartButtonGameObject;

		if (!_rematchButtonGameObject.TryGetComponent(out _rematchButtonController))
		{
			throw new System.Exception($"{_rematchButtonGameObject.name} does not contains RematchButtonController component");
		}

		_suddenDeathBannerGameObject = suddenDeathBannerGameObject;

		if (!_suddenDeathBannerGameObject.TryGetComponent(out _suddenDeathBannerController))
		{
			throw new System.Exception($"{_suddenDeathBannerGameObject.name} does not contains SuddenDeathBannerController component");
		}

		_confirmSuddenDeathHintButtonGameObject = confirmSuddenDeathHintButtonGameObject;

		if (!_confirmSuddenDeathHintButtonGameObject.TryGetComponent(out _confirmSuddenDeathBannerHintButtonController))
		{
			throw new System.Exception($"{_confirmSuddenDeathHintButtonGameObject.name} does not contains ConfirmSuddenDeathBannerHintButtonController component");
		}

		_suddenDeathHintDisplayButtonGameObject = suddenDeathHintDisplayButtonGameObject;

		if (!_suddenDeathHintDisplayButtonGameObject.TryGetComponent(out _suddenDeathHintDisplayButtonController))
		{
			throw new System.Exception($"{_suddenDeathHintDisplayButtonController.name} does not contains ShowSuddenDeathHintButtonController component");
		}

		_gameplayBlurLayerGameObject = blurLayerGameObject;

		if (!_gameplayBlurLayerGameObject.TryGetComponent(out _gameplayBlurLayerTweenComponent))
		{
			throw new System.Exception($"{_gameplayBlurLayerGameObject.name} does not contains Tweens2D component");
		}

		_symbolSelectTitleGameObject = symbolSelectTitleGameObject;

		if (!_symbolSelectTitleGameObject.TryGetComponent(out _symbolSelectTitleTweenComponent))
		{
			throw new System.Exception($"{_symbolSelectTitleGameObject.name} does not contains Tweens2D component");
		}

		_symbolSelectBarGameObject = symbolSelectBarGameObject;

		if (!_symbolSelectBarGameObject.TryGetComponent(out _symbolSelectBarTweenComponent))
		{
			throw new System.Exception($"{_symbolSelectBarGameObject.name} does not contains Tweens2D component");
		}

		if (!_symbolSelectBarGameObject.TryGetComponent(out _symbolSelectController))
		{
			throw new System.Exception($"{_symbolSelectBarGameObject.name} does not contains SymbolSelectController component");
		}


		_difficultylSelectTitleGameObject = difficultyTitleGameObject;

		if (!_difficultylSelectTitleGameObject.TryGetComponent(out _difficultylSelectTitleTweenComponent))
		{
			throw new System.Exception($"{_difficultylSelectTitleGameObject.name} does not contains Tweens2D component");
		}

		_difficultylSelectBarGameObject = difficultyBarGameObject;

		if (!_difficultylSelectBarGameObject.TryGetComponent(out _difficultylSelectBarTweenComponent))
		{
			throw new System.Exception($"{_difficultylSelectBarGameObject.name} does not contains Tweens2D component");
		}

		if (!_difficultylSelectBarGameObject.TryGetComponent(out _difficultySelectController))
		{
			throw new System.Exception($"{_difficultylSelectBarGameObject.name} does not contains DifficultySelectController component");
		}

		_aiConfigsConfirmButtonGameObject = confirmAiConfigsButtonGameObject;

		if (!_aiConfigsConfirmButtonGameObject.TryGetComponent(out _aiConfigsConfirmButtonTweenComponent))
		{
			throw new System.Exception($"{_aiConfigsConfirmButtonGameObject.name} does not contains Tweens2D component");
		}

		_tileGameObjectDict = tileGameObjectDict;
		_tileControllerDict = tileControllerDict;
		_symbolDict = symbolDict;
	}

	public void SetGameMode(GameMode mode)
	{
		_currentGameMode = mode;
	}

	async public void UpdateScore(int scoreChange)
    {
		ScoreDisplayController scoreDisplayController;

		if (_currentSymbol == Symbol.X)
		{
			scoreDisplayController = _scoreXDisplayController;
		}
		else if (_currentSymbol == Symbol.O)
		{
			scoreDisplayController = _scoreODisplayController;
		}
		else
		{
			throw new System.Exception($"Unexpected symbol detected: {_currentSymbol}");
		}

		List<Task> tweenTask = new();

		switch (scoreChange)
		{
			case 1:
				if (_currentSymbol == Symbol.X)
				{
					if (_xScore < 3 && _xScore >= 0)
					{
						_xScore += scoreChange;

						if (_xScore == 2) 
						{
							tweenTask.Add(scoreDisplayController.HighlighAppear());
						}

						tweenTask.Add(scoreDisplayController.MoveTextUp(scoreChange));
					}
					else
					{
						throw new System.Exception($"Invalid _xScore detected: {_xScore}");
					}
				}
				else
				{
					if (_oScore < 3 && _oScore >= 0)
					{
						_oScore += scoreChange;

						if (_oScore == 2)
						{
							tweenTask.Add(scoreDisplayController.HighlighAppear());
						}

						tweenTask.Add(scoreDisplayController.MoveTextUp(scoreChange));
					}
					else
					{
						throw new System.Exception($"Invalid _oScore detected: {_oScore}");
					}
				}
				break;

			case 2:
				if (_currentSymbol == Symbol.X)
				{
					if (_xScore == 0 || _xScore == 1)
					{
						_xScore += scoreChange;

						tweenTask.Add(scoreDisplayController.HighlighAppear());
						tweenTask.Add(scoreDisplayController.MoveTextUp(scoreChange));
					}
					else if (_xScore == 2)
					{
						_xScore += 1;
						tweenTask.Add(scoreDisplayController.MoveTextUp(1));
					}
					else
					{
						throw new System.Exception($"Invalid _xScore detected: {_xScore}");
					}
				}
				else
				{
					if (_oScore == 0 || _oScore == 1)
					{
						_oScore += scoreChange;

						tweenTask.Add(scoreDisplayController.HighlighAppear());
						tweenTask.Add(scoreDisplayController.MoveTextUp(scoreChange));
					}
					else if (_oScore == 2)
					{
						_oScore += 1;
						tweenTask.Add(scoreDisplayController.MoveTextUp(1));
					}
					else
					{
						throw new System.Exception($"Invalid _xScore detected: {_oScore}");
					}
				}
				break;

			case 3:
				if (_currentSymbol == Symbol.X)
				{
					if (_xScore == 0)
					{
						_xScore += scoreChange;

						tweenTask.Add(scoreDisplayController.HighlighAppear());
						tweenTask.Add(scoreDisplayController.MoveTextUp(scoreChange));
					}
					else if (_xScore == 1)
					{
						_xScore += 2;

						tweenTask.Add(scoreDisplayController.HighlighAppear());
						tweenTask.Add(scoreDisplayController.MoveTextUp(2));
					}
					else if (_xScore == 2)
					{
						_xScore += 1;

						tweenTask.Add(scoreDisplayController.MoveTextUp(1));
					}
					else
					{
						throw new System.Exception($"Invalid _xScore detected: {_xScore}");
					}
				}
				else
				{
					if (_oScore == 0)
					{
						_oScore += scoreChange;

						tweenTask.Add(scoreDisplayController.HighlighAppear());
						tweenTask.Add(scoreDisplayController.MoveTextUp(scoreChange));
					}
					else if (_oScore == 1)
					{
						_oScore += 2;

						tweenTask.Add(scoreDisplayController.HighlighAppear());
						tweenTask.Add(scoreDisplayController.MoveTextUp(2));
					}
					else if (_oScore == 2)
					{
						_oScore += 1;

						tweenTask.Add(scoreDisplayController.MoveTextUp(1));
					}
					else
					{
						throw new System.Exception($"Invalid _xScore detected: {_oScore}");
					}
				}
				break;

			default:
				throw new System.Exception($"Unexpected scoreChange value detected: {scoreChange}");
		}

		foreach (var task in tweenTask)
		{
			await task;
		}
    }

    async public Task MarkTile(TilePositionIndex index, Symbol symbol)
    {
		_symbolDict[index] = symbol;
		await _tileControllerDict[index].MarkSymbol(symbol);
	}

	async public Task ChangeSymbol()
	{
		if (_currentSymbol.Equals(Symbol.X))
		{
			_currentSymbol = Symbol.O;
			await _turnDisplayController.ChangeSymbol(Symbol.O);
		}
		else if (_currentSymbol.Equals(Symbol.O))
		{
			_currentSymbol = Symbol.X;
			await _turnDisplayController.ChangeSymbol(Symbol.X);
		}
		else
		{
			throw new System.Exception($"Unexpected symbol detected: {_currentSymbol}");
		}
	}

	public async Task ShowSuddenDeathHint()
	{
		Debug.Log($"ShowSuddenDeathHint called");

		_suddenDeathBannerController.Show();
		_confirmSuddenDeathBannerHintButtonController.Show();

		_gameplayBlurLayerGameObject.SetActive(true);
		
		List<Task> tasks = new() 
		{
			_gameplayBlurLayerTweenComponent.ExecuteTweenOrders("Appear"),
			_suddenDeathBannerController.Move(),
			_confirmSuddenDeathBannerHintButtonController.Move()
		};
		
		foreach (var task in tasks)
		{
			await task;
		}

		_isShowingSuddenDeathHint = true;
	}

	public async Task HideSuddenDeathHint()
	{
		//Debug.Log($"HideSuddenDeathHint called");

		List<Task> tasks = new()
		{
			_gameplayBlurLayerTweenComponent.ExecuteTweenOrders("Disappear"),
			_suddenDeathBannerController.Move(),
			_confirmSuddenDeathBannerHintButtonController.Move()
		};

		foreach (var task in tasks)
		{
			await task;
		}

		_suddenDeathBannerController.Hide();
		_suddenDeathBannerController.ResetPosition();

		_confirmSuddenDeathBannerHintButtonController.Hide();
		_confirmSuddenDeathBannerHintButtonController.ResetPosition();

		_gameplayBlurLayerGameObject.SetActive(false);

		_isShowingSuddenDeathHint = false;
	}

	public void GetAIConfigs()
	{
		_playerSymbol = _symbolSelectController.SelectedSymbol;
		_difficulty = _difficultySelectController.CurrentDifficulty;
	}

    public async Task ResetTiles()
	{
		List<Task> tasks = new();

		foreach (var kvp in _tileControllerDict)
		{
			tasks.Add(kvp.Value.SymbolClear());
			_symbolDict[kvp.Key] = Symbol.None;
		}

		foreach (var task in tasks)
		{
			await task;
		}
	}

	public async Task ResetTilesByList(List<TilePositionIndex> list)
	{
		List<Task> tasks = new();

		foreach (var index in list)
		{
			tasks.Add(_tileControllerDict[index].SymbolClear());
			_symbolDict[index] = Symbol.None;
		}

		foreach (var task in tasks)
		{
			await task;
		}
	}

	async public Task ResetGame(bool useTween, bool replay)
	{
		List<Task> tasks = new();

		if (useTween)
		{
			tasks.Add(_quitButtonController.MoveOut());
			tasks.Add(_rematchButtonController.MoveOut());
			tasks.Add(_resultScoreXController.MoveOut());
			tasks.Add(_resultScoreOController.MoveOut());
		}

		foreach (var task in tasks)
		{
			await task;
		}

		await ResetTiles();

		tasks.Clear();

		_quitButtonController.ResetPosition();
		_rematchButtonController.ResetPosition();
		_resultScoreXController.ResetPosition();
		_resultScoreOController.ResetPosition();

		_turnDisplayController.ResetPosition();
		_scoreXDisplayController.ResetPosition();
		_scoreODisplayController.ResetPosition();
		
		SetAllTilesInteractable(true);

		_currentSymbol = Symbol.X;
		_xScore = 0;
		_oScore = 0;
		_isGameEnd = false;

		_resultScoreXController.SetTransparencyAll(0f, 0f);
		_resultScoreOController.SetTransparencyAll(0f, 0f);

		if (useTween)
		{
			_turnDisplayController.SetTransparencyAll(255f);
			_scoreXDisplayController.SetTransparencyAll(0f, 255f);
			_scoreODisplayController.SetTransparencyAll(0f, 255f);
		}

		tasks = new()
		{
			_turnDisplayController.ResetSymbol(),
			_turnDisplayController.MoveIn(),
			_scoreXDisplayController.MoveIn(),
			_scoreODisplayController.MoveIn()
		};

		foreach (var task in tasks)
		{
			await task;
		}

		_scoreXDisplayController.ResetAll();
		_scoreODisplayController.ResetAll();

		if (!useTween)
		{
			_turnDisplayController.SetTransparencyAll(255f);
			_scoreXDisplayController.SetTransparencyAll(0f, 255f);
			_scoreODisplayController.SetTransparencyAll(0f, 255f);
		}

		if (replay)
		{
			if (_currentGameMode == GameMode.Local)
			{
				_gameStateDict[GameStates.PlayerAction].OnEnter();
			}
			else
			{
				_gameStateDict[GameStates.Initiate].OnEnter();
			}
		}
	}

	//public void ChangeState()
	//{
	//	_currentState = _gameStateDict[_currentState.NextStateKey];
	//	_currentState.OnEnter();

	//	Debug.Log($"{_currentState} called");
	//}


	public void SetAllTilesInteractable(bool value)
	{
		foreach (var component in _tileControllerDict)
		{
			component.Value.SetInteractable(value);
		}
	}

	public void SetLastAction(LastActionTakenBy action)
	{
		_lastAction = action;
	}
}

public enum GameStates
{
    Initiate,
    PlayerAction,
	AIAction,
    TurnFinalize,
	EndGame
}

public enum LastActionTakenBy
{
	Player,
	AI
}
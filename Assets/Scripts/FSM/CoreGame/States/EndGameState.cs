using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EndGameState : BaseState<GameStates>
{
	private bool _isInit = false;
	private TurnDisplayController _turnDisplayController;

	private ScoreDisplayController _scoreXDisplayController;
	private ScoreDisplayController _scoreODisplayController;

	private ResultScoreController _xScoreResultController;
	private ResultScoreController _oScoreResultController;

	private QuitButtonController _quitButtonController;
	private RematchButtonController _rematchButtonController;

	private bool _isRematch = false;
	public bool IsRematch => _isRematch;

	ScreensEnum _transitionTo;
	ScreenTransitionCode _transitionCode;


	public EndGameState(GameStates state) : base(state)
	{
	}

	public override void OnEnter()
	{
		//Debug.Log("Enter EndGameState");

		_gameManager = GameManager.Instance;

		if (!_isInit)
		{
			_turnDisplayController = _gameManager.TurnDisplayController;

			_scoreXDisplayController = _gameManager.ScoreXDisplayController;
			_scoreODisplayController = _gameManager.ScoreODisplayController;

			_xScoreResultController = _gameManager.ResultScoreXController;
			_oScoreResultController = _gameManager.ResultScoreOController;

			_quitButtonController = _gameManager.QuitButtonController;
			_rematchButtonController = _gameManager.RematchButtonController;

			_isInit = true;
		}

		if (_gameManager.CurrentSymbol == Symbol.X || _gameManager.CurrentSymbol == Symbol.O)
		{
			_gameManager.VictoryBannerController.SetSymbol(_gameManager.CurrentSymbol);
		}
		else
		{
			throw new System.Exception($"Unexpected symbol detected: {_gameManager.CurrentSymbol}");
		}

		OnUpdate();
	}
	async public override void OnUpdate()
	{
		_xScoreResultController.SetTransparencyAll(0f, 255);
		_oScoreResultController.SetTransparencyAll(0f, 255);

		await _gameManager.VictoryBannerController.StartAnimation();

		_xScoreResultController.SetScore(_gameManager.XScore);
		_oScoreResultController.SetScore(_gameManager.OScore);

		List<Task> tweenTasks = new()
		{
			_turnDisplayController.MoveOut(),
			_scoreXDisplayController.MoveOut(),
			_scoreODisplayController.MoveOut(),
			_gameManager.SuddenDeathHintDisplayButtonController.MoveOut()
		};

		foreach (var task in tweenTasks)
		{
			await task;
		}

		_turnDisplayController.SetTransparencyAll(0f);
		_scoreXDisplayController.SetTransparencyAll(0f, 0f);
		_scoreODisplayController.SetTransparencyAll(0f, 0f);
		_gameManager.SuddenDeathHintDisplayButtonController.Hide();

		if (_gameManager.CurrentSymbol == Symbol.X)
		{
			tweenTasks.Add(_xScoreResultController.HighlightAppear());
		}
		else if (_gameManager.CurrentSymbol == Symbol.O)
		{
			tweenTasks.Add(_oScoreResultController.HighlightAppear());
		}
		else
		{
			throw new System.Exception($"Invalid symbol detected: {_gameManager.CurrentSymbol}");
		}

		foreach (var task in tweenTasks)
		{
			await task;
		}

		tweenTasks.Clear();

		tweenTasks = new()
		{
			_xScoreResultController.MoveIn(),
			_oScoreResultController.MoveIn(),
			_quitButtonController.MoveIn(),
			_rematchButtonController.MoveIn()
		};

		foreach (var task in tweenTasks)
		{
			await task;
		}

		_turnDisplayController.ResetPosition();
		_scoreXDisplayController.ResetPosition();
		_scoreODisplayController.ResetPosition();
		_gameManager.SuddenDeathHintDisplayButtonController.ResetPosition();
	}
	async public override void OnExit()
	{
		if (_isRematch)
		{
			await GameManager.Instance.ResetGame(true, true);
		}
		else
		{
			await GameManager.Instance.ResetGame(false, false);
			ScreenManager.Instance.SetNextScreen(_transitionTo, _transitionCode);
		}
	}

	public void Rematch()
	{
		_isRematch = true;

		AIActionState aIActionState = (AIActionState)_gameManager.GameStateDict[GameStates.AIAction];
		aIActionState.ResetData();

		OnExit();
	}

	public void QuitGame(ScreensEnum transitionTo, ScreenTransitionCode transitionCode)
	{
		_isRematch = false;

		_transitionTo = transitionTo;
		_transitionCode = transitionCode;

		OnExit();
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerActionState : BaseState<GameStates>
{
	Dictionary<TilePositionIndex, Symbol> _symbolDict;
	//Symbol _currentSymbol;

	//bool _isClicked = false;
	bool _isInit = false;

	public PlayerActionState(GameStates state) : base(state)
	{
	}

	public override void OnEnter()
	{
		//Debug.Log($"Enter PlayerActionState; Symbol: {_gameManager.CurrentSymbol}");

		if (!_isInit)
		{
			_gameManager = GameManager.Instance;
			_symbolDict = _gameManager.SymbolDict;
			//_currentSymbol = _gameManager.CurrentSymbol;

			_isInit = true;
		}

		_gameManager.SetAllTilesInteractable(true);
	}

	async public Task TileClick(TilePositionIndex index)
	{
		if (_symbolDict[index] == Symbol.None)
		{
			_gameManager.SetAllTilesInteractable(false);

			await _gameManager.MarkTile(index, _gameManager.CurrentSymbol);
			_gameManager.LastMarkedTile = index;
			OnExit();
		}
	}

	public override void OnUpdate()
	{
	}

	public override void OnExit()
	{
		if (_gameManager.CurrentGameMode == GameMode.VersusAI)
		{
			_gameManager.SetLastAction(LastActionTakenBy.Player);
		}

		_gameManager.GameStateDict[GameStates.TurnFinalize].OnEnter();
	}
}

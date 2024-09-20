using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<GameStates>
{
	protected GameManager _gameManager;
	protected GameStates _currentStateKey;
	public GameStates CurrentStateKey => _currentStateKey;

	protected GameStates _nextStateKey;
	public GameStates NextStateKey => _nextStateKey;

	public BaseState(GameStates state) 
    {
        _gameManager = GameManager.Instance;
		_currentStateKey = state;
	}

    public abstract void OnEnter();
	public abstract void OnUpdate();
	public abstract void OnExit();
}

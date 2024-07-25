using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseGuidePage<EPage>: IState 
    where EPage: Enum
{
    // guide page manager instance;
    //List<>
    private Symbol _randomSymbol;
 //   private int _randomColIndex;
	//private int _randomColIndex;
	private Dictionary<Tuple<int, int>, GameObject> _tiles;
	private Dictionary<Tuple<int, int>, Tweens2D> _tileTweens;
	private Button _nextButtonComponent;
    private Button _backMenuButtonComponent;

    public BaseGuidePage()
    {

    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

    async public Task Reset()
    {
        foreach (var tileTween in _tileTweens.Values)
        {
            //tileTween.
        }

        await Task.Yield();
    }
}

public enum LineType
{
    Row, Column, Diagonal
}

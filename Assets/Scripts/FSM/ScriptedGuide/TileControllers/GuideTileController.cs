using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTileController : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Tweens2D _highlightTween;
    private Tweens2D _symbolTween;
    private TileSymbolController _symbolController;

	private void Awake()
	{
        GameObject childHighlight, childSymbol;

        if (TryGetComponent(out _rectTransform)) 
        {
            throw new Exception($"{gameObject.name} must contain RectTransform component"); 
        }
        else
        {
            childHighlight = _rectTransform.GetChild(0).gameObject;
            childSymbol = _rectTransform.GetChild(1).gameObject;
        }

		if (!childHighlight.TryGetComponent(out _highlightTween))
        {
            throw new Exception($"Cannot find Tweens2D component within {childHighlight.name} of {gameObject.name}");
        }

        if (!childSymbol.TryGetComponent(out _symbolTween))
        {
			throw new Exception($"Cannot find Tweens2D component within {childSymbol.name} of {gameObject.name}");
		}

        if (!childSymbol.TryGetComponent(out _symbolController))
        {
			throw new Exception($"Cannot find TileSymbolController component within {childSymbol.name} of {gameObject.name}");
		}
	}

    public void SetTileSymbol(Symbol symbol)
    {
		Symbol validatedSymbol = symbol switch
		{
			Symbol.X => Symbol.X,
			Symbol.O => Symbol.O,
			_ => throw new Exception($"Unexpected symbol detected: {symbol} in {gameObject.name}")
		};

		_symbolController.SetSymbol(validatedSymbol);
    }

	async public Task ResetTile()
	{
        List<Task> tasks = new List<Task>()
		{
		    _highlightTween.ExecuteTweenOrders("Highlight Off"),
		    _symbolTween.ExecuteTweenOrders("Symbol Off")
	    };

        foreach (var task in tasks)
        {
            await task;
        }
	}

    async public Task HighlightOn()
    {
        await _highlightTween.ExecuteTweenOrders("Highlight On");
    }

	async public Task SymbolOn()
	{
		await _symbolTween.ExecuteTweenOrders("Symbol On");
	}
}

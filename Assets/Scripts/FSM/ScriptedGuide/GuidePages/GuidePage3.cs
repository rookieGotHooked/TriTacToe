using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GuidePage3 : BaseGuidePage<GuidePageEnum>
{
	private FSM_GuideManager _instance;
	private bool _isInit = false;
	public GuidePage3(GuidePageObjects objects, GuidePageEnum currentPageKey, GuidePageEnum nextPageKey, GuidePageEnum previousPageKey) :
		base(objects, currentPageKey, nextPageKey, previousPageKey)
	{
	}

	async public override Task OnEnter()
	{
		if (!_isInit)
		{
			_instance = FSM_GuideManager.Instance;
		}

		_symbolControllers[TilePositionIndex.Tile00].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile01].SetSymbol(Symbol.O);
		_symbolControllers[TilePositionIndex.Tile02].SetSymbol(Symbol.X);

		_symbolControllers[TilePositionIndex.Tile10].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile11].SetSymbol(Symbol.O);
		_symbolControllers[TilePositionIndex.Tile12].SetSymbol(Symbol.X);

		_symbolControllers[TilePositionIndex.Tile20].SetSymbol(Symbol.O);
		_symbolControllers[TilePositionIndex.Tile21].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile22].SetSymbol(Symbol.O);


		List<Task> tweenHighlightSymbolTasks = new()
		{
			_tileSymbolTweens[TilePositionIndex.Tile00].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile01].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile02].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile20].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile21].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile22].ExecuteTweenOrders("Symbol On"),
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}

		await Task.Delay(500);

		tweenHighlightSymbolTasks = new()
		{
			_tileSymbolTweens[TilePositionIndex.Tile00].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile01].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile02].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile20].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile21].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile22].ExecuteTweenOrders("Symbol Off"),
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}
	}

	async public override Task OnUpdate()
	{
		await Task.Yield();
	}

	async public override Task OnExit()
	{
		await Task.Yield();
	}
}

using System.Threading.Tasks;
using System.Collections.Generic;

public class GuidePage1 : BaseGuidePage<GuidePageEnum>
{
	private FSM_GuideManager _instance;
	private bool _isInit = false;

	public GuidePage1(GuidePageObjects objects, GuidePageEnum currentPageKey, GuidePageEnum nextPageKey, GuidePageEnum previousPageKey) :
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
		_symbolControllers[TilePositionIndex.Tile01].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile02].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile10].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile11].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile12].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile20].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile21].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile22].SetSymbol(Symbol.X);

		List<Task> tweenHighlightSymbolTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Highlight On"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight On"),
			_tileHighlightTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Highlight On"),

			_tileSymbolTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Symbol On"),
			
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}

		//await Task.Delay(500);
		await DelayHelper.Delay(0.5f);

		tweenHighlightSymbolTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Highlight Off"),

			_tileSymbolTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Symbol Off"),
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}

		tweenHighlightSymbolTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile01].ExecuteTweenOrders("Highlight On"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight On"),
			_tileHighlightTweens[TilePositionIndex.Tile21].ExecuteTweenOrders("Highlight On"),

			_tileSymbolTweens[TilePositionIndex.Tile01].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile21].ExecuteTweenOrders("Symbol On"),
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}

		//await Task.Delay(500);
		await DelayHelper.Delay(0.5f);

		tweenHighlightSymbolTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile01].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile21].ExecuteTweenOrders("Highlight Off"),

			_tileSymbolTweens[TilePositionIndex.Tile01].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile21].ExecuteTweenOrders("Symbol Off"),
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}

		tweenHighlightSymbolTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile02].ExecuteTweenOrders("Highlight On"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight On"),
			_tileHighlightTweens[TilePositionIndex.Tile20].ExecuteTweenOrders("Highlight On"),

			_tileSymbolTweens[TilePositionIndex.Tile02].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol On"),
			_tileSymbolTweens[TilePositionIndex.Tile20].ExecuteTweenOrders("Symbol On"),
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
		List<Task> tweenHighlightSymbolTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile02].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile20].ExecuteTweenOrders("Highlight Off"),

			_tileSymbolTweens[TilePositionIndex.Tile02].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile20].ExecuteTweenOrders("Symbol Off"),
		};

		foreach (var task in tweenHighlightSymbolTasks)
		{
			await task;
		}
	}
}

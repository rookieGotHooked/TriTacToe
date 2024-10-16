using System.Threading.Tasks;
using System.Collections.Generic;

public class GuidePage0 : BaseGuidePage<GuidePageEnum>
{
	private FSM_GuideManager _instance;
	private bool _isInit = false;

	public GuidePage0(GuidePageObjects objects, GuidePageEnum currentPageKey, GuidePageEnum nextPageKey, GuidePageEnum previousPageKey) : 
		base(objects, currentPageKey, nextPageKey, previousPageKey)
	{
	}

    async public override Task OnEnter()
    {
		if (!_isInit)
		{
			_instance = FSM_GuideManager.Instance;

			List<Task> tweenTileTasks = new()
			{
				AwaitTween(_tileTweens[TilePositionIndex.Tile00]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile01]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile02]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile10]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile11]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile12]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile20]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile21]),
				AwaitTween(_tileTweens[TilePositionIndex.Tile22])
			};

			foreach (var task in tweenTileTasks)
			{
				await task;
			}
		}

		_instance.NextButtonComponent.SetInteractable(false);

		_symbolControllers[TilePositionIndex.Tile10].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile11].SetSymbol(Symbol.X);
		_symbolControllers[TilePositionIndex.Tile12].SetSymbol(Symbol.X);

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

		_instance.NextButtonComponent.SetInteractable(true);
	}

	async private Task AwaitTween(Tweens2D tweens)
	{
		while (!tweens.IsInit)
		{
			await Task.Yield();
		}
	}

	async public override Task OnUpdate()
	{
		await Task.Yield();
	}

	async public override Task OnExit()
	{
		List<Task> tweenTasks = new()
		{
			_tileHighlightTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Highlight Off"),
			_tileHighlightTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Highlight Off"),

			_tileSymbolTweens[TilePositionIndex.Tile10].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile11].ExecuteTweenOrders("Symbol Off"),
			_tileSymbolTweens[TilePositionIndex.Tile12].ExecuteTweenOrders("Symbol Off"),
		};

		foreach (var task in tweenTasks)
		{
			await task;
		}
	}
}

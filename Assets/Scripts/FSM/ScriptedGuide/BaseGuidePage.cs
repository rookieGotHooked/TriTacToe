using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseGuidePage<EPage> where EPage: Enum
{
	protected Dictionary<TilePositionIndex, GameObject> _tiles = new();
    protected Dictionary<TilePositionIndex, TileSymbolController> _symbolControllers = new();
	protected Dictionary<TilePositionIndex, Tweens2D> _tileTweens = new();
	protected Dictionary<TilePositionIndex, Tweens2D> _tileHighlightTweens = new();
	protected Dictionary<TilePositionIndex, Tweens2D> _tileSymbolTweens = new();

	private GameObject _guideTextGameObject;

    protected Tweens2D _guideTextTween;

	private List<TilePositionIndex> _activeTiles;

    private EPage _nextPageKey;
    public EPage NextPageKey { get => _nextPageKey; }

    private EPage _currentPageKey;
    public EPage CurrentPageKey { get => _currentPageKey; }

    private EPage _previousPageKey;
    public EPage PreviousPageKey { get => _previousPageKey; }

    public BaseGuidePage(GuidePageObjects objects, EPage currentPageKey, EPage nextPageKey, EPage previousPageKey)
    {
        foreach (var tile in objects.Tiles)
        {
            if (!_tiles.TryAdd(tile.PositionIndex, tile.TileGameObject))
            {
                throw new Exception($"Invalid value added into _tiles dictionary in {GetType().Name}. " +
                    $"Value: {tile.PositionIndex}, {tile.TileGameObject}");
            }

            if (!_tileTweens.TryAdd(tile.PositionIndex, tile.TileGameObject.GetComponent<Tweens2D>()))
            {
                throw new Exception($"Invalid value added into _tileTweens dictionary in {GetType().Name}. " +
                    $"Value: {tile.PositionIndex}, {tile.TileGameObject.GetComponent<Tweens2D>()}");
            }

            GameObject highlightChild = tile.TileGameObject.transform.GetChild(0).gameObject;
            GameObject symbolChild = tile.TileGameObject.transform.GetChild(1).gameObject;

			if (!_tileHighlightTweens.TryAdd(tile.PositionIndex, highlightChild.GetComponent<Tweens2D>()))
			{
				throw new Exception($"Invalid value added into _tileHighlightTweens dictionary in {GetType().Name}. " +
					$"Value: {tile.PositionIndex}, {highlightChild.GetComponent<Tweens2D>()}");
			}

			if (!_tileSymbolTweens.TryAdd(tile.PositionIndex, symbolChild.GetComponent<Tweens2D>()))
			{
				throw new Exception($"Invalid value added into _tileSymbolTweens dictionary in {GetType().Name}. " +
					$"Value: {tile.PositionIndex}, {symbolChild.GetComponent<Tweens2D>()}");
			}

            if (!_symbolControllers.TryAdd(tile.PositionIndex, symbolChild.GetComponent<TileSymbolController>()))
			{
				throw new Exception($"Invalid value added into _symbolControllers dictionary in {GetType().Name}. " +
					$"Value: {tile.PositionIndex}, {symbolChild.GetComponent<TileSymbolController>()}");
			}
		}

        if (nextPageKey != null)
        {
			_nextPageKey = nextPageKey;
		}
        else
        {
            throw new Exception("nextPageKey is null");
        }

        if (currentPageKey != null)
        {
            _currentPageKey = currentPageKey;
		}
		else
		{
			throw new Exception("currentPageKey is null");
		}

        if (objects.GuideTextGameObject == null)
        {
            throw new Exception("GuideTextGameObject is null");
        }
        else
        {
            _guideTextGameObject = objects.GuideTextGameObject;

			if (!_guideTextGameObject.TryGetComponent(out _guideTextTween))
            {
                throw new Exception($"{_guideTextGameObject.name} does not contains Tweens2D component");
            }
		}

		if (previousPageKey != null)
		{
			_previousPageKey = previousPageKey;
		}
		else
		{
			throw new Exception("previousPageKey is null");
		}
	}


    async public Task ExecuteTransitionAnimations()
    {
        await _guideTextTween.ExecuteTweenOrders("");

        List<Task> tasks = new();

        foreach (var tileIndex in _activeTiles)
        {
            tasks.Add(_tileTweens[tileIndex].ExecuteTweenOrders("Appear"));
        }

        foreach (var task in tasks)
        {
            await task;
        }
    }

    public abstract Task OnEnter();
    public abstract Task OnUpdate();
    public abstract Task OnExit();

    async public Task ResetTilesWithAnimation()
    {
        List<Task> tweenTasks = new();

        foreach (var index in _activeTiles)
        {
            tweenTasks.Add(_tileTweens[index].ExecuteTweenOrders("Reset Tile"));
        }

        foreach (var task in tweenTasks)
        {
            await task;
        }
    }
}

public class GuidePageObjects 
{
    public GuidePageObjects(List<TileObject> tiles, GameObject guideTextGameObject, GameObject nextButton, GameObject backMenuButton)
    {
        Tiles = tiles;
        GuideTextGameObject = guideTextGameObject;
		NextButton = nextButton;
        BackMenuButton = backMenuButton;
    }

    public List<TileObject> Tiles;
    public GameObject GuideTextGameObject;
    public GameObject NextButton;
    public GameObject BackMenuButton;
}

[Serializable]
public class TileObject
{
    public TileObject(TilePositionIndex positionIndex, GameObject tileGameObject)
    {
        _positionIndex = positionIndex;
		_tileGameObject = tileGameObject;
    }

	[SerializeField] TilePositionIndex _positionIndex;
	public TilePositionIndex PositionIndex { get => _positionIndex; }

	[SerializeField] GameObject _tileGameObject;
	public GameObject TileGameObject { get => _tileGameObject; }
}

public enum TilePositionIndex
{
	Tile00, Tile01, Tile02,
	Tile10, Tile11, Tile12,
	Tile20, Tile21, Tile22
}

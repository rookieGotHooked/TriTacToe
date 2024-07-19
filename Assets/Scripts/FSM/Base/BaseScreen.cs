using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.TimeZoneInfo;

public abstract class BaseScreen<EScreen>: IState 
	where EScreen: Enum
{
	#region Variables and Constructor
	protected bool _clicked = false;

	protected GameObject _screenObject;
	public GameObject ScreenObject { get => _screenObject; }

	protected RectTransform _screenCanvasRectTransform;
	protected Tweens2D _screenTransitionTween;
	protected ScreenDefinition<EScreen> _screenDefinition;

	protected bool _isInit = false;

	public EScreen ScreenKey { get; protected set; }

	public BaseScreen(ScreenDefinition<EScreen> screenDefinition)
	{
		if (screenDefinition != null)
		{
			ScreenKey = screenDefinition.ScreenEnum;

			_screenObject = screenDefinition.ScreenObject;
			_screenDefinition = screenDefinition;
			_screenCanvasRectTransform = _screenObject.GetComponent<RectTransform>();
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();
		}
		else
		{
			throw new Exception($"{ GetType().Name } assets is null.");
		}
	}

	protected List<GameObject> _buttonsGameObject = new List<GameObject>();
	protected List<GameObject> _staticSpritesGameObject = new List<GameObject>();
	protected List<GameObject> _dynamicSpritesGameObject = new List<GameObject>();
	#endregion


	#region Abstract functions
	public abstract void OnEnter();
	public abstract void OnUpdate();
	public abstract void OnExit();
	#endregion


	#region Helper virtual functions

	async virtual public Task InstantiateObjects()
	{
		foreach (var group in _screenDefinition.ObjectGroupsList)
		{
			List<Task> transitionTask = new List<Task>();

			foreach (var asset in group.Objects)
			{
				GameObject newGameObject = UnityEngine.Object.Instantiate(asset.Object, _screenCanvasRectTransform);
				ObjectClassification(newGameObject, asset.Type);

				LinearTransformTween initialTransition = newGameObject.GetComponent<LinearTransformTween>();

				if (initialTransition != null && !initialTransition.IsFunctionCallsOnly)
				{
					transitionTask.Add(initialTransition.TweenPosition());
				}
			}

			foreach (var task in transitionTask)
			{
				await task;
			}
		}
	}

	public void ObjectClassification(GameObject gameObj, AssetType assetType)
	{
		switch (assetType)
		{
			case AssetType.Button:
				_buttonsGameObject.Add(gameObj);
				break;

			case AssetType.StaticSprite:
				_staticSpritesGameObject.Add(gameObj);
				break;

			case AssetType.DynamicSprite:
				_dynamicSpritesGameObject.Add(gameObj);
				break;
		}
	}

	async public virtual Task MoveLeft(TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();

			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x - 1080f, currentRectTransform.anchoredPosition.y),
			tweenFormula,
			transitionTime);
	}
	async public virtual Task MoveRight(TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x + 1080f, currentRectTransform.anchoredPosition.y),
			tweenFormula, transitionTime);
	}

	async public virtual Task MoveUp(TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y + 1920f),
			tweenFormula, transitionTime);
	}

	async public virtual Task MoveDown(TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}

			await _screenTransitionTween.TweenPosition(
				currentRectTransform.anchoredPosition, 
				new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y - 1920f),
				tweenFormula, transitionTime);
		}
	}
	#endregion
}

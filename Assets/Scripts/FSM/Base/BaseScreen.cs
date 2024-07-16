using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class BaseScreen<EScreen>: IState 
	where EScreen: Enum
{
	#region Variables and Constructor
	protected bool _clicked = false;
	protected GameObject _screenObject;
	public GameObject ScreenObject { get => _screenObject; }
	protected RectTransform _screenCanvasRectTransform;
	protected LinearTransformTween _screenTransitionTween;
	protected bool _isInit = false;
	protected AssetsGroup<EScreen> _assetsGroup;

	protected const TweenType _tweenType = TweenType.EaseOutQuint;

	protected float _transitionTime = 0.5f;

	public EScreen ScreenKey { get; protected set; }
	protected BaseScreen(EScreen screenKey)
	{
		ScreenKey = screenKey;
	}
	protected EScreen _nextScreen;
	public EScreen NextScreen { get => _nextScreen; }

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

	public virtual void AssignAssets(AssetsGroup<EScreen> assetsGroup)
	{
		_screenObject = assetsGroup.ScreenObject;
		_screenCanvasRectTransform = _screenObject.GetComponent<RectTransform>();
		_screenTransitionTween = _screenObject.GetComponent<LinearTransformTween>();

		_assetsGroup = assetsGroup;
	}

	async virtual public Task InstantiateObjects()
	{
		foreach (var group in _assetsGroup.ObjectGroupsList)
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

	async public virtual Task MoveLeft()
	{
		if (_screenTransitionTween == null)
		{
			Debug.Log($"Curent GameObject: {_screenObject}");
			_screenTransitionTween = _screenObject.GetComponent<LinearTransformTween>();

			if (_screenTransitionTween == null)
			{
				throw new Exception("Cannot find LinearTransformTween component within the game object.");
			}
			else
			{
				RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
				await _screenTransitionTween.TweenPosition(
					currentRectTransform.anchoredPosition, 
					new Vector2(currentRectTransform.anchoredPosition.x - 1080f, currentRectTransform.anchoredPosition.y),
					_tweenType,
					_transitionTime);
			}
		}
		else
		{
			RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
			await _screenTransitionTween.TweenPosition(
				currentRectTransform.anchoredPosition, 
				new Vector2(currentRectTransform.anchoredPosition.x - 1080f, currentRectTransform.anchoredPosition.y),
				_tweenType, 
				_transitionTime);
		}
	}
	async public virtual Task MoveRight()
	{
		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<LinearTransformTween>();

			if (_screenTransitionTween == null)
			{
				throw new Exception("Cannot find LinearTransformTween component within the game object.");
			}
			else
			{
				RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
				await _screenTransitionTween.TweenPosition(
					currentRectTransform.anchoredPosition, 
					new Vector2(currentRectTransform.anchoredPosition.x + 1080f, currentRectTransform.anchoredPosition.y),
					_tweenType,
					_transitionTime);
			}
		}
		else
		{
			RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
			await _screenTransitionTween.TweenPosition(
				currentRectTransform.anchoredPosition, 
				new Vector2(currentRectTransform.anchoredPosition.x + 1080f, currentRectTransform.anchoredPosition.y), 
				_tweenType,
				_transitionTime);
		}
	}

	async public virtual Task MoveUp()
	{
		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<LinearTransformTween>();

			if (_screenTransitionTween == null)
			{
				throw new Exception("Cannot find LinearTransformTween component within the game object.");
			}
			else
			{
				//Debug.Log(_screenObject);
				RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
				await _screenTransitionTween.TweenPosition(
					currentRectTransform.anchoredPosition, 
					new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y + 1920f),
					_tweenType,
					_transitionTime);
			}
		}
		else
		{
			RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
			await _screenTransitionTween.TweenPosition(
				currentRectTransform.anchoredPosition, 
				new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y + 1920f), 
				_tweenType,
				_transitionTime);
		}
	}

	async public virtual Task MoveDown()
	{
		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<LinearTransformTween>();

			if (_screenTransitionTween == null)
			{
				throw new Exception("Cannot find LinearTransformTween component within the game object.");
			}
			else
			{
				RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
				await _screenTransitionTween.TweenPosition(
					currentRectTransform.anchoredPosition, 
					new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y - 1920f), 
					_tweenType,
					_transitionTime);
			}
		}
		else
		{
			RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();
			await _screenTransitionTween.TweenPosition(
				currentRectTransform.anchoredPosition, 
				new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y - 1920f), 
				_tweenType, 
				_transitionTime);
		}
	}
	#endregion
}

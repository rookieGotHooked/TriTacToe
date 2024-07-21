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

	protected List<Func<float, TweenFormulas, float, Task>> _transitionFunctions;

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

			//_transitionFunctions.Add()
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


	#region Abstract Enter / Update / Exit functions
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

				Tweens2D initialTransition;

				if (newGameObject.TryGetComponent(out initialTransition))
				{

				}

				//if (initialTransition != null && !initialTransition.IsFunctionCallsOnly)
				//{
				//	transitionTask.Add(initialTransition.TweenPosition());
				//}
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

	public virtual Task TransitionMapping(MovementDirection direction, float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		Task transitionTask = direction switch
		{
			MovementDirection.MoveLeft => MoveLeft(movementValue, tweenFormula, transitionTime),
			MovementDirection.MoveRight => MoveRight(movementValue, tweenFormula, transitionTime),
			MovementDirection.MoveUp => MoveUp(movementValue, tweenFormula, transitionTime),
			MovementDirection.MoveDown => MoveDown(movementValue, tweenFormula, transitionTime),

			_ => throw new Exception("Unexpected movement direction received")
		};

		return transitionTask;
	}

	#endregion

	#region Screen Transition functions
	async public virtual Task MoveLeft(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current {GetType().Name} MoveDown is called with negative movementValue; recommend using MoveUp function.");
			}
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of {GetType().Name} MoveDown is equals 0. No action taken.");
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x - movementValue, currentRectTransform.anchoredPosition.y),
			tweenFormula,
			transitionTime);
	}
	async public virtual Task MoveRight(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current {GetType().Name} MoveDown is called with negative movementValue; recommend using MoveUp function.");
			}
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of {GetType().Name} MoveDown is equals 0. No action taken.");
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x + movementValue, currentRectTransform.anchoredPosition.y),
			tweenFormula, transitionTime);
	}

	async public virtual Task MoveUp(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current {GetType().Name} MoveDown is called with negative movementValue; recommend using MoveUp function.");
			}
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of {GetType().Name} MoveDown is equals 0. No action taken.");
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition,
			new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y + movementValue),
			tweenFormula, transitionTime);
	}

	async public virtual Task MoveDown(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = _screenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = _screenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current { GetType().Name } MoveDown is called with negative movementValue; recommend using MoveUp function.");
			} 
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of { GetType().Name } MoveDown is equals 0. No action taken.");
				return;
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition,
			new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y - movementValue),
			tweenFormula, transitionTime);
	}
	#endregion
}

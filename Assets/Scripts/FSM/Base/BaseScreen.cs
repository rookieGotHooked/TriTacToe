using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseScreen<EScreen>: IState 
	where EScreen: Enum
{
	#region Variables and Constructor
	protected bool _clicked = false;

	protected GameObject _screenObject;
	public GameObject ScreenObject { get => _screenObject; }

	protected bool _isInit = false;
	public bool IsInit { get => _isInit; }

	protected RectTransform _screenCanvasRectTransform;
	protected Tweens2D _screenTransitionTween;
	protected ScreenDefinition<EScreen> _screenDefinition;

	//protected List<Func<float, TweenFormulas, float, Task>> _transitionFunctions;

	public EScreen ScreenKey { get; protected set; }

	public BaseScreen(ScreenDefinition<EScreen> screenDefinition)
	{
		if (screenDefinition != null)
		{
			ScreenKey = screenDefinition.ScreenEnum;

			_screenDefinition = screenDefinition;
			_screenObject = UnityEngine.Object.Instantiate(screenDefinition.ScreenObject);
			_screenCanvasRectTransform = _screenObject.GetComponent<RectTransform>();
			_screenTransitionTween = _screenObject.GetComponent<Tweens2D>();
		}
		else
		{
			throw new Exception($"{ GetType().Name } assets is null.");
		}
	}

	protected List<GameObject> _buttonsGameObject = new();
	protected List<GameObject> _staticSpritesGameObject = new();
	protected List<GameObject> _dynamicSpritesGameObject = new();
	protected List<GameObject> _slidersGameObject = new();
	protected List<GameObject> _customGameObject = new();
	#endregion


	#region Abstract Enter / Update / Exit functions
	public abstract void OnEnter();
	public abstract void OnUpdate();
	public abstract void OnExit();
	#endregion


	#region Helper virtual functions

	public void SetInit()
	{
		_isInit = true;
	}

	async virtual public Task InstantiateObjects()
	{
		foreach (var group in _screenDefinition.ObjectGroupsList)
		{
			List<Task> transitionTask = new List<Task>();

			foreach (var asset in group.Objects)
			{
				GameObject newGameObject = UnityEngine.Object.Instantiate(asset.Object, _screenCanvasRectTransform);
				ObjectClassification(newGameObject, asset.Type);
				transitionTask.Add(AwaitingTransition(newGameObject));
			}

			foreach (var task in transitionTask)
			{
				await task;
			}
		}
	}

	public List<AudioSource> GetAllSFXSource()
	{
		List<AudioSource> audioSources = new();

		List<GameObject> allGameObjects = new();

		allGameObjects.AddRange(_buttonsGameObject);
		allGameObjects.AddRange(_staticSpritesGameObject);
		allGameObjects.AddRange(_dynamicSpritesGameObject);
		allGameObjects.AddRange(_slidersGameObject);
		allGameObjects.AddRange(_customGameObject);

		foreach (var gameObj in allGameObjects)
		{
			AudioSource audioSource;

			if (gameObj.TryGetComponent(out audioSource))
			{
				audioSources.Add(audioSource);
			}
		}

		return audioSources;
	}

	async public Task AwaitingTransition(GameObject gameObject)
	{
		if (gameObject.TryGetComponent(out Tweens2D initialTransition))
		{
			while (initialTransition.IsTweening)
			{
				await Task.Yield();
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

			case AssetType.Slider:
				_slidersGameObject.Add(gameObj);
				break;

			case AssetType.Custom:
				_customGameObject.Add(gameObj);
				break;
		}
	}

	public virtual Task TransitionMapping(MovementDirection direction, float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		Task transitionTask = direction switch
		{
			MovementDirection.MoveLeftToRight => MoveLeftToRight(movementValue, tweenFormula, transitionTime),
			MovementDirection.MoveRightToLeft => MoveRightToLeft(movementValue, tweenFormula, transitionTime),
			MovementDirection.MoveBottomToTop => MoveBottomToTop(movementValue, tweenFormula, transitionTime),
			MovementDirection.MoveTopToBottom => MoveTopToBottom(movementValue, tweenFormula, transitionTime),

			_ => throw new Exception("Unexpected movement direction received")
		};

		return transitionTask;
	}

	#endregion

	#region Screen Transition functions
	async public virtual Task MoveLeftToRight(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = ScreenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = ScreenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = ScreenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current {GetType().Name} MoveLeftToRight is called with negative movementValue; recommend using MoveRightToLeft function.");
			}
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of {GetType().Name} MoveLeftToRight is equals 0. No action taken.");
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x + movementValue, currentRectTransform.anchoredPosition.y),
			tweenFormula,
			transitionTime);
	}
	async public virtual Task MoveRightToLeft(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = ScreenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = ScreenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = ScreenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current {GetType().Name} MoveRightToLeft is called with negative movementValue; recommend using MoveLeftToRight function.");
			}
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of {GetType().Name} MoveRightToLeft is equals 0. No action taken.");
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition, 
			new Vector2(currentRectTransform.anchoredPosition.x - movementValue, currentRectTransform.anchoredPosition.y),
			tweenFormula, transitionTime);
	}

	async public virtual Task MoveBottomToTop(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = ScreenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = ScreenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = ScreenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current {GetType().Name} MoveBottomToTop is called with negative movementValue; recommend using MoveTopToBottom function.");
			}
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of {GetType().Name} MoveBottomToTop is equals 0. No action taken.");
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition,
			new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y + movementValue),
			tweenFormula, transitionTime);
	}

	async public virtual Task MoveTopToBottom(float movementValue, TweenFormulas tweenFormula, float transitionTime)
	{
		RectTransform currentRectTransform = ScreenObject.GetComponent<RectTransform>();

		if (_screenTransitionTween == null)
		{
			_screenTransitionTween = ScreenObject.GetComponent<Tweens2D>();

			if (_screenTransitionTween == null)
			{
				_screenTransitionTween = ScreenObject.AddComponent<Tweens2D>();
			}

			if (movementValue < 0)
			{
				Debug.LogWarning($"The current { GetType().Name } MoveTopToBottom is called with negative movementValue; recommend using MoveBottomToTop function.");
			} 
			else if (movementValue == 0)
			{
				Debug.LogWarning($"Tweening value of { GetType().Name } MoveTopToBottom is equals 0. No action taken.");
				return;
			}
		}

		await _screenTransitionTween.TweenPosition(
			currentRectTransform.anchoredPosition,
			new Vector2(currentRectTransform.anchoredPosition.x, currentRectTransform.anchoredPosition.y - movementValue),
			tweenFormula, transitionTime);
	}
	#endregion

	public void SetInteractableButtons(bool value)
	{
		foreach (var btn in _buttonsGameObject)
		{
			btn.GetComponent<Button>().interactable = value;
		}
	}
}

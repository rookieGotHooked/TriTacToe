using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class Tweens2D : MonoBehaviour
{
	private RectTransform _rectTransform;
	private readonly Transform _transform;

	[Header("Usage Options")]

	[SerializeField]
	[Tooltip("Set this component to only calls by functions; all attached tweens 2d scriptable objects will not be used")]
	private bool _isFunctionCallsOnly;
	public bool IsFunctionCallsOnly { get => _isFunctionCallsOnly; }

	[SerializeField]
	[Tooltip("Use anchored position instead of world space position for tweening; will use rectTransform.position by default")]
	private bool _useAnchoredPosition;
	public bool UseAnchoredPosition { get => _useAnchoredPosition; }

	[SerializeField]
	[Tooltip("Tween group orders that would run sequentially by configs")]
	private List<TweenParallels> _tweenOrders = new();

	private readonly List<TweenParallels> _tweenCleanOrders = new();

	private delegate float DelegatedFloat(float x);
	private readonly DelegatedFloat _tweenPositionDelegate;
	private readonly DelegatedFloat _tweenScaleDelegate;
	private readonly DelegatedFloat _tweenCustomSizeDelegate;

	private void Awake()
	{
		if (!TryGetComponent(out _rectTransform))
		{
			throw new Exception("GameObject does not contains RectTransform component");
		}

		CheckAndPopulateTweenOrder();

	}

	async void Start()
	{
		if (!_isFunctionCallsOnly)
		{
			await ExecuteTweenOrders();
		}
	}

	#region Internal handler functions
	private void CheckAndPopulateTweenOrder()
	{
		if (_isFunctionCallsOnly && _tweenOrders.Count > 0)
		{
			Debug.LogWarning($"{gameObject.name} is set to function calls only while contains existing tween orders; tween orders will be ignored");
		}
		else if (!_isFunctionCallsOnly && _tweenOrders.Count == 0)
		{
			throw new Exception($"Missing values: {gameObject.name} does not have any tween orders and does not set to function calls only");
		}
		else
		{
			int posCount = 0;
			int scaleCount = 0;
			int sizeCount = 0;

			foreach (var item in _tweenOrders)
			{
				foreach (var subItem in item.parallelTweens)
				{
					switch (subItem.tweenTypes)
					{
						case TweenTypes.Position:
							posCount++;
							break;
						case TweenTypes.Scale:
							scaleCount++;
							break;
						case TweenTypes.CustomSize:
							sizeCount++;
							break;
					}

					if (posCount > 1 || scaleCount > 1 || sizeCount > 1)
					{
						throw new Exception("Invalid data: Multiple calls of the same tween type at the same time is not supported");
					}
				}

				_tweenCleanOrders.Add(item);
			}
		}
	}

	private float TweenMapping(DelegatedFloat delegatedFloat, TweenFormulas selectedTweenType, float value)
	{
		delegatedFloat = selectedTweenType switch
		{
			TweenFormulas.EaseInSine => TweenMethods.EaseInSine,
			TweenFormulas.EaseOutSine => TweenMethods.EaseOutSine,
			TweenFormulas.EaseInOutSine => TweenMethods.EaseInOutSine,
			TweenFormulas.EaseInQuad => TweenMethods.EaseInQuad,
			TweenFormulas.EaseOutQuad => TweenMethods.EaseOutQuad,
			TweenFormulas.EaseInOutQuad => TweenMethods.EaseInOutQuad,
			TweenFormulas.EaseInCubic => TweenMethods.EaseInCubic,
			TweenFormulas.EaseOutCubic => TweenMethods.EaseOutCubic,
			TweenFormulas.EaseInOutCubic => TweenMethods.EaseInOutCubic,
			TweenFormulas.EaseInQuart => TweenMethods.EaseInQuart,
			TweenFormulas.EaseOutQuart => TweenMethods.EaseOutQuart,
			TweenFormulas.EaseInOutQuart => TweenMethods.EaseInOutQuart,
			TweenFormulas.EaseInQuint => TweenMethods.EaseInQuint,
			TweenFormulas.EaseOutQuint => TweenMethods.EaseOutQuint, 
			TweenFormulas.EaseInOutQuint => TweenMethods.EaseInOutQuint,
			TweenFormulas.EaseInExpo => TweenMethods.EaseInExpo,
			TweenFormulas.EaseOutExpo => TweenMethods.EaseOutExpo,
			TweenFormulas.EaseInOutExpo => TweenMethods.EaseInOutExpo,
			TweenFormulas.EaseInCirc => TweenMethods.EaseInCirc,
			TweenFormulas.EaseOutCirc => TweenMethods.EaseOutCirc,
			TweenFormulas.EaseInOutCirc => TweenMethods.EaseInOutCirc,
			TweenFormulas.EaseInBack => TweenMethods.EaseInBack,
			TweenFormulas.EaseOutBack => TweenMethods.EaseOutBack,
			TweenFormulas.EaseInOutBack => TweenMethods.EaseInOutBack,
			TweenFormulas.EaseInElastic => TweenMethods.EaseInElastic,
			TweenFormulas.EaseOutElastic => TweenMethods.EaseOutElastic,
			TweenFormulas.EaseInOutElastic => TweenMethods.EaseInOutElastic,
			TweenFormulas.EaseInBounce => TweenMethods.EaseInBounce,
			TweenFormulas.EaseOutBounce => TweenMethods.EaseOutBounce,
			TweenFormulas.EaseInOutBounce => TweenMethods.EaseInOutBounce,

			_ => throw new Exception($"Unexpected TweenFormulas detected: {selectedTweenType}")
		};

		return delegatedFloat(value);
	}

	async private Task ExecuteTweenOrders()
	{
		foreach (var item in _tweenCleanOrders)
		{
			List<Task> tweenTask = new();

			foreach (var subItem in item.parallelTweens)
			{
				switch (subItem.tweenTypes) 
				{
					case TweenTypes.Position:

						Vector2 currentPosition, finalPosition;

						if (subItem is DirectionalMovement movementType)
						{
							if (_useAnchoredPosition)
							{
								currentPosition = _rectTransform.anchoredPosition;
							}
							else
							{
								currentPosition = _rectTransform.position;
							}

							DirectionalMovement temp = movementType;

							finalPosition = temp.direction switch
							{
								MovementDirection.MoveLeft 
									=> new Vector2(currentPosition.x - temp.movementValue, currentPosition.y),
								MovementDirection.MoveRight 
									=> new Vector2(currentPosition.x + temp.movementValue, currentPosition.y),
								MovementDirection.MoveUp 
									=> new Vector2(currentPosition.x, currentPosition.y + temp.movementValue),
								MovementDirection.MoveDown 
									=> new Vector2(currentPosition.x, currentPosition.y - temp.movementValue),

								_ => throw new Exception($"Unexpected value detected: {temp.direction}. Expected values are: MoveLeft, MoveRight, MoveUp, MoveDown"),
							}; 
						}
						else if (subItem is AbsoluteTween positionAbsoluteType)
						{
							AbsoluteTween temp = positionAbsoluteType;

							currentPosition = temp.finalValue;
							finalPosition = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected movement type detected: {subItem.GetType()}. Expected values are: DirectionalMovement, PositionalMovement");
						}
						tweenTask.Add(TweenPosition(currentPosition, finalPosition, subItem.tweenFormula, subItem.duration));

						break;

					case TweenTypes.Scale:
						
						Vector2 currentScale, finalScale;

						if (subItem is AbsoluteTween scaleAbsoluteType)
						{
							AbsoluteTween temp = scaleAbsoluteType;

							currentScale = _rectTransform.localScale;
							finalScale = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {subItem.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						tweenTask.Add(TweenCustomSize(currentScale, finalScale, subItem.tweenFormula, subItem.duration));
						
						break;

					case TweenTypes.CustomSize:

						Vector2 currentSize, finalSize;

						if (subItem is AbsoluteTween customSizeAbsoluteType)
						{
							AbsoluteTween temp = customSizeAbsoluteType;

							currentSize = _rectTransform.sizeDelta;
							finalSize = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {subItem.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						tweenTask.Add(TweenScale(currentSize, finalSize, subItem.tweenFormula, subItem.duration));
						break;
				}
			}

			foreach (var tween in tweenTask)
			{
				await tween;
			}
		}
	}

	private bool CheckExternalCalls([CallerMemberName] string callerMemberName = "")
	{
		return callerMemberName == nameof(CheckExternalCalls);
	}

	#endregion

	#region Tween functions
	async public Task TweenPosition(Vector2 currentPosition, Vector2 targetPosition, TweenFormulas TweenFormulas, float duration)
	{
		if (!_isFunctionCallsOnly)
		{
			if (CheckExternalCalls(nameof(TweenPosition)))
			{
				throw new Exception($"{gameObject.name} TweenPosition called when component is not set to function calls only");
			}
		}
		else
		{
			float elapsedTime = 0f;

			if (duration <= 0f)
			{
				throw new Exception("Invalid value: Duration cannot be equals or smaller than 0.");
			}

			while (elapsedTime < duration)
			{

				float tweenValue = TweenMapping(_tweenPositionDelegate, TweenFormulas, elapsedTime / duration);
				Vector2 newPosition = currentPosition + (targetPosition - currentPosition) * tweenValue;

				if (_rectTransform)
				{
					if (_useAnchoredPosition)
					{
						_rectTransform.anchoredPosition = newPosition;
					}
					else
					{
						_rectTransform.position = newPosition;
					}
				}

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
			if (_useAnchoredPosition)
			{
				_rectTransform.anchoredPosition = targetPosition;
			}
			else
			{
				_rectTransform.position = targetPosition;
			}
		}
	}

	async public Task TweenCustomSize(Vector2 currentSize, Vector2 targetSize, TweenFormulas TweenFormulas, float duration)
	{
		if (!_isFunctionCallsOnly)
		{
			if (CheckExternalCalls(nameof(TweenCustomSize)))
			{
				throw new Exception($"{gameObject.name} TweenPosition called when component is not set to function calls only");
			}
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < duration)
			{
				float tweenValue = TweenMapping(_tweenCustomSizeDelegate, TweenFormulas, elapsedTime / duration);
				Vector2 newCustomSize = currentSize + (targetSize - currentSize) * tweenValue;

				_rectTransform.sizeDelta = newCustomSize;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

			_rectTransform.sizeDelta = targetSize;
		}
	}

	async public Task TweenScale(Vector2 currentScale, Vector2 targetScale, TweenFormulas TweenFormulas, float duration)
	{
		if (!_isFunctionCallsOnly)
		{
			if (CheckExternalCalls(nameof(TweenScale)))
			{
				throw new Exception($"{gameObject.name} TweenPosition called when component is not set to function calls only");
			}
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < duration)
			{
				float tweenValue = TweenMapping(_tweenScaleDelegate, TweenFormulas, elapsedTime / duration);
				Vector2 newScale = currentScale + (targetScale - currentScale) * tweenValue;

				if (_rectTransform)
				{
					_rectTransform.localScale = newScale;
				}
				else
				{
					_transform.localScale = newScale;
				}

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

			_rectTransform.localScale = targetScale;
		}

	}

	#endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Tweens2D : MonoBehaviour
{
	private RectTransform _rectTransform;
	private readonly Transform _transform;
	private Image _imageComponent;
	private bool _isTweening = false;
	public bool IsTweening { get => _isTweening; }

	[Header("Usage Options")]

	//[SerializeField]
	//[Tooltip("Set this component to only calls by functions; all attached tweens 2d scriptable objects will not be used")]
	//private bool _isFunctionCallsOnly;
	//public bool IsFunctionCallsOnly { get => _isFunctionCallsOnly; }

	[SerializeField]
	[Tooltip("Use anchored position instead of world space position for tweening; will use rectTransform.position by default")]
	private bool _useAnchoredPosition;
	public bool UseAnchoredPosition { get => _useAnchoredPosition; }

	[SerializeField]
	[Tooltip("Tween group orders that would run sequentially by configs")]
	private List<TweenParallels> _tweenInitialOrders = new();

	[SerializeField]
	[Tooltip("Presets of separated tween group orders for function calls; will be added with string into a dictionary for using as order call")]
	private List<TweenRepeatableOrders> _tweenRepeatableOrders = new();

	private readonly List<TweenParallels> _tweenCleanOrders = new();
	private readonly Dictionary<string, TweenParallels> _tweenRepeatableDict = new();

	private delegate float DelegatedFloat(float x);
	private readonly DelegatedFloat _tweenDelegate;

	private void Awake()
	{
		if (!TryGetComponent(out _rectTransform))
		{
			throw new Exception("GameObject does not contains RectTransform component");
		}
		TryGetComponent(out _imageComponent);

		CheckAndPopulateTweenInitialOrder();
		CheckAndPopulateRepeatableTweenOrders();
	}

	async void Start()
	{
		await ExecuteTweenOrders();
	}

	#region Internal handler functions
	private void CheckAndPopulateTweenInitialOrder()
	{
		//if (_isFunctionCallsOnly && _tweenInitialOrders.Count > 0)
		//{
		//	Debug.LogWarning($"{gameObject.name} is set to function calls only while contains existing tween orders; tween orders will be ignored");
		//}
		//else if (!_isFunctionCallsOnly && _tweenInitialOrders.Count == 0)
		//{
		//	throw new Exception($"Missing values: {gameObject.name} does not have any tween orders and does not set to function calls only");
		//}
		//else
		//{
		foreach (var item in _tweenInitialOrders)
		{
			int posCount = 0;
			int scaleCount = 0;
			int sizeCount = 0;
			int transparencyCount = 0;

			foreach (var subItem in item.parallelTweens)
			{
				TweenTypes subItemTweenType;

				subItemTweenType = subItem switch
				{
					AbsoluteTween absoluteTween => absoluteTween.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					_ => throw new Exception($"Unexpected values: {subItem.GetType()} is not supported")
				};

				switch (subItemTweenType)
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
					case TweenTypes.Transparency:
						transparencyCount++;
						break;
				}

				if (posCount > 1 || scaleCount > 1 || sizeCount > 1 || transparencyCount > 1)
				{
					throw new Exception("Invalid data: Multiple calls of the same tween type at the same time is not supported");
				}
			}

			_tweenCleanOrders.Add(item);
		}
		//}
	}

	public void CheckAndPopulateRepeatableTweenOrders()
	{
		foreach(var item in _tweenRepeatableOrders)
		{
			int posCount = 0;
			int scaleCount = 0;
			int sizeCount = 0;
			int transparencyCount = 0;

			foreach (var subItem in item.TweenParallel.parallelTweens)
			{
				TweenTypes subItemTweenType;

				subItemTweenType = subItem switch
				{
					AbsoluteTween absoluteTween => absoluteTween.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					_ => throw new Exception($"Unexpected values: {subItem.GetType()} is not supported")
				};

				switch (subItemTweenType)
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
					case TweenTypes.Transparency:
						transparencyCount++;
						break;
				}

				if (posCount > 1 || scaleCount > 1 || sizeCount > 1 || transparencyCount > 1)
				{
					throw new Exception("Invalid data: Multiple calls of the same tween type at the same time is not supported");
				}
			}

			bool res = _tweenRepeatableDict.TryAdd(item.RepeatableOrderName, item.TweenParallel);

			if (!res)
			{
				throw new Exception("Failed to add repeatable tween order into repeatable order dictionary");
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
		_isTweening = true;

		foreach (var item in _tweenCleanOrders)
		{
			List<Task> tweenTask = new();

			foreach (var subItem in item.parallelTweens)
			{
				TweenTypes subItemTweenType;

				subItemTweenType = subItem switch
				{
					AbsoluteTween absoluteTween => absoluteTween.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					_ => throw new Exception($"Unexpected item type received: {subItem.GetType().Name}; please check the dictionary populate functions")
				};

				switch (subItemTweenType) 
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
								MovementDirection.MoveLeftToRight
									=> new Vector2(currentPosition.x + temp.movementValue, currentPosition.y),
								MovementDirection.MoveRightToLeft
									=> new Vector2(currentPosition.x - temp.movementValue, currentPosition.y),
								MovementDirection.MoveBottomToTop
									=> new Vector2(currentPosition.x, currentPosition.y + temp.movementValue),
								MovementDirection.MoveTopToBottom
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
						
						tweenTask.Add(TweenScale(currentScale, finalScale, subItem.tweenFormula, subItem.duration));
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

						tweenTask.Add(TweenCustomSize(currentSize, finalSize, subItem.tweenFormula, subItem.duration));
						break;

					case TweenTypes.Transparency:

						float currentAlpha, finalAlpha;

						if (subItem is SingleFloatTween alphaTweenType)
						{
							SingleFloatTween temp = alphaTweenType;

							currentAlpha = _imageComponent.color.a;
							finalAlpha = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {subItem.GetType()}. Expected value(s) are: SingleFloatTween");
						}

						tweenTask.Add(TweenTransparent(currentAlpha, finalAlpha, subItem.tweenFormula, subItem.duration));
						break;
				}
			}

			foreach (var tween in tweenTask)
			{
				await tween;
			}
		}

		_isTweening = false;
	}

	async public Task ExecuteTweenOrders(string orderName)
	{
		_isTweening = true;

		if (!_tweenRepeatableDict.ContainsKey(orderName))
		{
			throw new Exception($"Received order does not exists; orderName: {orderName}");
		}
		else
		{
			List<Task> tweenTask = new();

			foreach (var item in _tweenRepeatableDict[orderName].parallelTweens)
			{
				TweenTypes itemTweenType;

				itemTweenType = item switch
				{
					AbsoluteTween absoluteTween => absoluteTween.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					_ => throw new Exception($"Unexpected item type received: {item.GetType().Name}; please check the dictionary populate functions")
				};

				switch (itemTweenType)
				{
					case TweenTypes.Position:

						Vector2 currentPosition, finalPosition;

						if (item is DirectionalMovement movementType)
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
								MovementDirection.MoveLeftToRight
									=> new Vector2(currentPosition.x + temp.movementValue, currentPosition.y),
								MovementDirection.MoveRightToLeft
									=> new Vector2(currentPosition.x - temp.movementValue, currentPosition.y),
								MovementDirection.MoveBottomToTop
									=> new Vector2(currentPosition.x, currentPosition.y + temp.movementValue),
								MovementDirection.MoveTopToBottom
									=> new Vector2(currentPosition.x, currentPosition.y - temp.movementValue),

								_ => throw new Exception($"Unexpected value detected: {temp.direction}. Expected values are: MoveLeft, MoveRight, MoveUp, MoveDown"),
							};
						}
						else if (item is AbsoluteTween positionAbsoluteType)
						{
							AbsoluteTween temp = positionAbsoluteType;

							currentPosition = temp.finalValue;
							finalPosition = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected movement type detected: {item.GetType()}. Expected values are: DirectionalMovement, PositionalMovement");
						}
						tweenTask.Add(TweenPosition(currentPosition, finalPosition, item.tweenFormula, item.duration));

						break;

					case TweenTypes.Scale:

						Vector2 currentScale, finalScale;

						if (item is AbsoluteTween scaleAbsoluteType)
						{
							AbsoluteTween temp = scaleAbsoluteType;

							currentScale = _rectTransform.localScale;
							finalScale = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						tweenTask.Add(TweenScale(currentScale, finalScale, item.tweenFormula, item.duration));

						break;

					case TweenTypes.CustomSize:

						Vector2 currentSize, finalSize;

						if (item is AbsoluteTween customSizeAbsoluteType)
						{
							AbsoluteTween temp = customSizeAbsoluteType;

							currentSize = _rectTransform.sizeDelta;
							finalSize = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						tweenTask.Add(TweenCustomSize(currentSize, finalSize, item.tweenFormula, item.duration));
						break;

					case TweenTypes.Transparency:

						float currentAlpha, finalAlpha;

						if (item is SingleFloatTween alphaTweenType)
						{
							SingleFloatTween temp = alphaTweenType;

							currentAlpha = _imageComponent.color.a;
							finalAlpha = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: SingleFloatTween");
						}

						tweenTask.Add(TweenTransparent(currentAlpha, finalAlpha, item.tweenFormula, item.duration));
						break;
				}
			}

			foreach (var task in tweenTask)
			{
				await task;
			}
		}

		_isTweening = false;
	}

	private bool CheckExternalCalls([CallerMemberName] string callerMemberName = "")
	{
		return callerMemberName == nameof(CheckExternalCalls);
	}

	#endregion

	#region Tween functions
	async public Task TweenPosition(Vector2 currentPosition, Vector2 targetPosition, TweenFormulas tweenFormulas, float duration)
	{
		//if (!_isFunctionCallsOnly)
		//{
		//	if (CheckExternalCalls(nameof(TweenPosition)))
		//	{
		//		throw new Exception($"{gameObject.name} TweenPosition called when component is not set to function calls only");
		//	}
		//}

		float elapsedTime = 0f;

		if (duration <= 0f)
		{
			throw new Exception("Invalid value: Duration cannot be equals or smaller than 0.");
		}

		while (elapsedTime < duration)
		{

			float tweenValue = TweenMapping(_tweenDelegate, tweenFormulas, elapsedTime / duration);
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

	async public Task TweenCustomSize(Vector2 currentSize, Vector2 targetSize, TweenFormulas tweenFormulas, float duration)
	{
		//if (!_isFunctionCallsOnly)
		//{
		//	if (CheckExternalCalls(nameof(TweenCustomSize)))
		//	{
		//		throw new Exception($"{gameObject.name} TweenCustomSize called when component is not set to function calls only");
		//	}
		//}
		//else
		//{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormulas, elapsedTime / duration);
			Vector2 newCustomSize = currentSize + (targetSize - currentSize) * tweenValue;

			_rectTransform.sizeDelta = newCustomSize;

			elapsedTime += Time.deltaTime;
			await Task.Yield();
		}

		_rectTransform.sizeDelta = targetSize;
		//}
	}

	async public Task TweenScale(Vector2 currentScale, Vector2 targetScale, TweenFormulas tweenFormulas, float duration)
	{
		//if (!_isFunctionCallsOnly)
		//{
		//	if (CheckExternalCalls(nameof(TweenScale)))
		//	{
		//		throw new Exception($"{gameObject.name} TweenScale called when component is not set to function calls only");
		//	}
		//}
		//else
		//{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormulas, elapsedTime / duration);
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
		//}

	}

	async public Task TweenTransparent(float currentAlpha, float finalAlpha, TweenFormulas tweenFormulas, float duration)
	{
		//if (!_isFunctionCallsOnly)
		//{
		//	if (CheckExternalCalls(nameof(TweenTransparent)))
		//	{
		//		throw new Exception($"{gameObject.name} TweenTransparent called when component is not set to function calls only");
		//	}
		//}
		//else
		//{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormulas, elapsedTime / duration);
			float newAlpha = currentAlpha + (finalAlpha - currentAlpha) * tweenValue;

			_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, newAlpha);
				
			elapsedTime += Time.deltaTime;
			await Task.Yield();
		}

		_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, finalAlpha);
		//}
	}

	#endregion
}

[Serializable]
public class TweenRepeatableOrders
{
	[SerializeField] string _repeatableOrderName;
	public string RepeatableOrderName { get => _repeatableOrderName; }

	[SerializeField] TweenParallels _tweenParallel;
	public TweenParallels TweenParallel { get => _tweenParallel; }
}
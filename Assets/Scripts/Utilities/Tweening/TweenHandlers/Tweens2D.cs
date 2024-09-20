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
	private bool _isInit = false;

	public bool IsTweening { get => _isTweening; }
	public bool IsInit { get => _isInit; }
	private bool _isDisrupted = false;
	private List<Task> _tweeningTasks = new();

	[Header("Usage Options")]

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
		if (_tweenInitialOrders.Count > 0)
		{
			await ExecuteTweenOrders();
		}
	}

	#region Internal handler functions
	private void CheckAndPopulateTweenInitialOrder()
	{
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
					AbsoluteTweenVector2 absoluteTweenVector2 => absoluteTweenVector2.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					_ => throw new Exception($"{gameObject.name} - unexpected values: {subItem.GetType()} is not supported")
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
	}

	public void CheckAndPopulateRepeatableTweenOrders()
	{
		foreach(var item in _tweenRepeatableOrders)
		{
			int posCount = 0, 
				scaleCount = 0,
				sizeCount = 0, 
				transparencyCount = 0,
				rotationCount = 0;

			foreach (var subItem in item.TweenParallel.parallelTweens)
			{
				TweenTypes subItemTweenType;

				subItemTweenType = subItem switch
				{
					AbsoluteTweenVector2 absoluteTweenVector2 => absoluteTweenVector2.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					RotationVector3 rotationVector3 => TweenTypes.Rotation,
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
					case TweenTypes.Rotation:
						rotationCount++;
						break;
				}

				if (posCount > 1 || scaleCount > 1 || sizeCount > 1 || transparencyCount > 1 || rotationCount > 1)
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
		if (!_isTweening)
		{
			_isTweening = true;
		}
		else
		{
			_isDisrupted = true;
			
			foreach (var task in _tweeningTasks)
			{
				Debug.Log(task.AsyncState);
				await task;
			}

			_tweeningTasks.Clear();

			_isDisrupted = false;
		}

		//Debug.Log($"gameObject: {gameObject}; _isTweening: {IsTweening}");
		//Debug.Log($"ExecuteTweenOrders: {gameObject.name} - {gameObject.GetHashCode()}");

		foreach (var item in _tweenCleanOrders)
		{
			foreach (var subItem in item.parallelTweens)
			{
				TweenTypes subItemTweenType;

				subItemTweenType = subItem switch
				{
					AbsoluteTweenVector2 absoluteTweenVector2 => absoluteTweenVector2.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					RotationVector3 rotationVector3 => TweenTypes.Rotation,
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
						else if (subItem is AbsoluteTweenVector2 positionAbsoluteType)
						{
							AbsoluteTweenVector2 temp = positionAbsoluteType;

							currentPosition = temp.finalValue;
							finalPosition = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected movement type detected: {subItem.GetType()}. Expected values are: DirectionalMovement, PositionalMovement");
						}

						_tweeningTasks.Add(TweenPosition(currentPosition, finalPosition, subItem.tweenFormula, subItem.duration));

						break;

					case TweenTypes.Scale:

						Vector2 currentScale, finalScale;

						if (subItem is AbsoluteTweenVector2 scaleAbsoluteType)
						{
							AbsoluteTweenVector2 temp = scaleAbsoluteType;

							currentScale = _rectTransform.localScale;
							finalScale = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {subItem.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						_tweeningTasks.Add(TweenScale(currentScale, finalScale, subItem.tweenFormula, subItem.duration));
						break;

					case TweenTypes.CustomSize:

						Vector2 currentSize, finalSize;

						if (subItem is AbsoluteTweenVector2 customSizeAbsoluteType)
						{
							AbsoluteTweenVector2 temp = customSizeAbsoluteType;

							currentSize = _rectTransform.sizeDelta;
							finalSize = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {subItem.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						_tweeningTasks.Add(TweenCustomSize(currentSize, finalSize, subItem.tweenFormula, subItem.duration));
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

						_tweeningTasks.Add(TweenTransparent(currentAlpha, finalAlpha, subItem.tweenFormula, subItem.duration));
						break;

					case TweenTypes.Rotation:

						Vector3 currentRotation, finalRotation;

						if (subItem is RotationVector3 rotationVector3)
						{
							RotationVector3 temp = rotationVector3;

							float simplifiedX = 0f, simplifiedY = 0f, simplifiedZ = 0f,
								xCoefficiency = 1f, yCoefficiency = 1f, zCoefficiency = 1f;

							currentRotation = _rectTransform.rotation.eulerAngles;
							finalRotation = temp.additionalRotationValue;

							if (finalRotation.x > 360f || finalRotation.y > 360f || finalRotation.z > 360f ||
								finalRotation.x < -360f || finalRotation.y < -360f || finalRotation.z < -360f)
							{
								Debug.LogWarning($"Final rotation value is currently larger / smaller than expected. " +
									$"The received value shall be simplified for processing rotation. Current values are: {gameObject.name}: {finalRotation}");

								simplifiedX = finalRotation.x % 360f;
								simplifiedY = finalRotation.y % 360f;
								simplifiedZ = finalRotation.z % 360f;
							}
							else
							{
								simplifiedX = finalRotation.x;
								simplifiedY = finalRotation.y;
								simplifiedZ = finalRotation.z;
							}

							if (temp.isXClockwise)
							{
								xCoefficiency = -1f;
							}
							if (temp.isYClockwise)
							{
								yCoefficiency = -1f;
							}
							if (temp.isZClockwise)
							{
								zCoefficiency = -1f;
							}

							if (temp.xFullRotationTime > 0 && simplifiedX > 0f)
							{
								finalRotation.x = currentRotation.x + (temp.xFullRotationTime * xCoefficiency * 360f + simplifiedX);
							}
							else
							{
								finalRotation.x = currentRotation.x + (simplifiedX * xCoefficiency);
							}

							if (temp.yFullRotationTime > 0 && simplifiedY > 0f)
							{
								finalRotation.y = currentRotation.y + (temp.yFullRotationTime * yCoefficiency * 360f + simplifiedY);
							}
							else
							{
								finalRotation.y = currentRotation.y + (simplifiedY * yCoefficiency);
							}

							if (temp.zFullRotationTime > 0 && simplifiedZ > 0f)
							{
								finalRotation.z = currentRotation.z + (temp.zFullRotationTime * zCoefficiency * 360f + simplifiedZ);
							}
							else
							{
								finalRotation.z = currentRotation.z + (simplifiedZ * zCoefficiency);
							}
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: RotationVector3");
						}

						_tweeningTasks.Add(TweenRotation(currentRotation, finalRotation, subItem.tweenFormula, subItem.duration));
						break;

				}
			}

			foreach (var tween in _tweeningTasks)
			{
				await tween;
			}
		}

		_tweeningTasks.Clear();

		_isTweening = false;
		_isDisrupted = false;
		_isInit = true;
	}

	async public Task ExecuteTweenOrders(string orderName)
	{
		if (!_tweenRepeatableDict.ContainsKey(orderName))
		{
			throw new Exception($"Received order does not exists; orderName: {orderName}; gameObject: {gameObject.name}");
		}
		else
		{
			if (!_isTweening)
			{
				_isTweening = true;
			}
			else
			{
				_isDisrupted = true;

				foreach (var task in _tweeningTasks)
				{
					await task;
				}

				_tweeningTasks.Clear();

				_isDisrupted = false;
			}

			foreach (var item in _tweenRepeatableDict[orderName].parallelTweens)
			{
				TweenTypes itemTweenType;

				itemTweenType = item switch
				{
					AbsoluteTweenVector2 absoluteTweenVector2 => absoluteTweenVector2.tweenType,
					DirectionalMovement => TweenTypes.Position,
					SingleFloatTween singleFloatTween => singleFloatTween.tweenType,
					RotationVector3 rotationVector3 => TweenTypes.Rotation,
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
						else if (item is AbsoluteTweenVector2 positionAbsoluteType)
						{
							AbsoluteTweenVector2 temp = positionAbsoluteType;

							currentPosition = temp.finalValue;
							finalPosition = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected movement type detected: {item.GetType()}. Expected values are: DirectionalMovement, PositionalMovement");
						}
						_tweeningTasks.Add(TweenPosition(currentPosition, finalPosition, item.tweenFormula, item.duration));

						break;

					case TweenTypes.Scale:

						Vector2 currentScale, finalScale;

						if (item is AbsoluteTweenVector2 scaleAbsoluteType)
						{
							AbsoluteTweenVector2 temp = scaleAbsoluteType;

							currentScale = _rectTransform.localScale;
							finalScale = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						_tweeningTasks.Add(TweenScale(currentScale, finalScale, item.tweenFormula, item.duration));

						break;

					case TweenTypes.CustomSize:

						Vector2 currentSize, finalSize;

						if (item is AbsoluteTweenVector2 customSizeAbsoluteType)
						{
							AbsoluteTweenVector2 temp = customSizeAbsoluteType;

							currentSize = _rectTransform.sizeDelta;
							finalSize = temp.finalValue;
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: AbsoluteTween");
						}

						_tweeningTasks.Add(TweenCustomSize(currentSize, finalSize, item.tweenFormula, item.duration));
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

						_tweeningTasks.Add(TweenTransparent(currentAlpha, finalAlpha, item.tweenFormula, item.duration));
						break;

					case TweenTypes.Rotation:
						Vector3 currentRotation, finalRotation;

						if (item is RotationVector3 rotationVector3)
						{
							RotationVector3 temp = rotationVector3;

							float simplifiedX = 0f, simplifiedY = 0f, simplifiedZ = 0f, 
								xCoefficiency = 1f, yCoefficiency = 1f, zCoefficiency = 1f;

							currentRotation = _rectTransform.rotation.eulerAngles;
							finalRotation = temp.additionalRotationValue;

							if (finalRotation.x > 360f || finalRotation.y > 360f || finalRotation.z > 360f ||
								finalRotation.x < -360f || finalRotation.y < -360f || finalRotation.z < -360f)
							{
								Debug.LogWarning($"Final rotation value is currently larger / smaller than expected. " +
									$"The received value shall be simplified for processing rotation. Current values are: {gameObject.name}: {finalRotation}");

								simplifiedX = finalRotation.x % 360f;
								simplifiedY = finalRotation.y % 360f;
								simplifiedZ = finalRotation.z % 360f;

							}
							else
							{
								simplifiedX = finalRotation.x; 
								simplifiedY = finalRotation.y; 
								simplifiedZ = finalRotation.z;
							}

							if (temp.isXClockwise)
							{
								xCoefficiency = -1f;
							}
							if (temp.isYClockwise)
							{
								yCoefficiency = -1f;
							}
							if (temp.isZClockwise)
							{
								zCoefficiency = -1f;
							}

							if (temp.xFullRotationTime > 0 && simplifiedX > 0f)
							{
								finalRotation.x = currentRotation.x + (temp.xFullRotationTime * xCoefficiency * 360f + simplifiedX);
							}
							else
							{
								finalRotation.x = currentRotation.x + (simplifiedX * xCoefficiency);
							}

							if (temp.yFullRotationTime > 0 && simplifiedY > 0f)
							{
								finalRotation.y = currentRotation.y + (temp.yFullRotationTime * yCoefficiency * 360f + simplifiedY);
							}
							else
							{
								finalRotation.y = currentRotation.y + (simplifiedY * yCoefficiency);
							}

							if (temp.zFullRotationTime > 0 && simplifiedZ > 0f)
							{
								finalRotation.z = currentRotation.z + (temp.zFullRotationTime * zCoefficiency * 360f + simplifiedZ);
							}
							else
							{
								finalRotation.z = currentRotation.z + (simplifiedZ * zCoefficiency);
							}
						}
						else
						{
							throw new Exception($"Unexpected tweening type detected: {item.GetType()}. Expected value(s) are: RotationVector3");
						}

						_tweeningTasks.Add(TweenRotation(currentRotation, finalRotation, item.tweenFormula, item.duration));
						break;

					default:
						throw new Exception($"Unexpected tween type detected: {itemTweenType}");
				}
			}

			foreach (var task in _tweeningTasks)
			{
				await task;
			}

			_tweeningTasks.Clear();
		}

		_isTweening = false;
		_isDisrupted = false;
	}

	private bool CheckExternalCalls([CallerMemberName] string callerMemberName = "")
	{
		return callerMemberName == nameof(CheckExternalCalls);
	}

	#endregion

	#region Tween functions
	async public Task TweenPosition(Vector2 currentPosition, Vector2 targetPosition, TweenFormulas tweenFormula, float duration)
	{
		_isTweening = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		//Debug.Log($"GameObject: {gameObject.name}; duration: {duration}");

		while (elapsedTime < duration)
		{
			if (!_isDisrupted)
			{
				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				Vector2 newPosition = currentPosition + (targetPosition - currentPosition) * tweenValue;

				if (_useAnchoredPosition)
				{
					_rectTransform.anchoredPosition = newPosition;
				}
				else
				{
					_rectTransform.position = newPosition;
				}

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
			else
			{
				break;
			}
		}

		if (_useAnchoredPosition)
		{
			_rectTransform.anchoredPosition = targetPosition;
		}
		else
		{
			_rectTransform.position = targetPosition;
		}

		_isTweening = false;
	}

	async public Task TweenCustomSize(Vector2 currentSize, Vector2 targetSize, TweenFormulas tweenFormula, float duration)
	{
		_isTweening = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		while (elapsedTime < duration)
		{
			if (!_isDisrupted)
			{
				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				Vector2 newCustomSize = currentSize + (targetSize - currentSize) * tweenValue;

				_rectTransform.sizeDelta = newCustomSize;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
			else
			{
				break;
			}
		}

		_rectTransform.sizeDelta = targetSize;

		_isTweening = false;
	}

	async public Task TweenScale(Vector2 currentScale, Vector2 targetScale, TweenFormulas tweenFormula, float duration)
	{
		_isTweening = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		while (elapsedTime < duration)
		{
			if (!_isDisrupted)
			{
				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
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
			else
			{
				break;
			}
		}

		_rectTransform.localScale = targetScale;

		_isTweening = false;
	}

	async public Task TweenTransparent(float currentAlpha, float finalAlpha, TweenFormulas tweenFormula, float duration)
	{
		_isTweening = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		while (elapsedTime < duration)
		{
			if (!_isDisrupted)
			{
				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				float newAlpha = currentAlpha + (finalAlpha - currentAlpha) * tweenValue;

				_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, newAlpha);

				//Debug.Log($"{gameObject.name} newAlpha: {newAlpha}");

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
			else
			{
				break;
			}
		}

		_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, finalAlpha);

		_isTweening = false;
	}

	async public Task TweenRotation(Vector3 currentRotation, Vector3 finalRotation, TweenFormulas tweenFormula, float duration)
	{
		_isTweening = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		while (elapsedTime < duration)
		{
			if (!_isDisrupted)
			{
				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				Vector3 newRotation = currentRotation + (finalRotation - currentRotation) * tweenValue;

				//Debug.Log($"newRotation: {newRotation}");

				Quaternion newQuaternion = Quaternion.Euler(newRotation);

				_rectTransform.rotation = newQuaternion;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
			else
			{
				break;
			}
		}

		_rectTransform.rotation = Quaternion.Euler(finalRotation);

		_isTweening = false;
	}

	async public Task ManualDisrupt()
	{
		_isDisrupted = true;

		foreach (var task in _tweeningTasks)
		{
			await task;
		}

		_tweeningTasks.Clear();

		_isDisrupted = false;
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
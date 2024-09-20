using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
//using static UnityEditor.Progress;

public class DisruptibleTweens : MonoBehaviour
{
	#region Object components
	
	RectTransform _rectTransformComp;
	Image _imageComp;

	#endregion


	#region Original component values

	Vector2 _originalAnchoredPosition;
	Vector2 _originalWorldPosition;
	Vector2 _originalCustomSize;
	Color _originalColor;
	Quaternion _originalRotation;

	#endregion


	#region Last component values

	Vector2 _lastAnchoredPosition;
	Vector2 _lastWorldPosition;
	Vector2 _lastCustomSize;
	Color _lastColor;
	Quaternion _lastRotation;

	#endregion


	#region Configs
	
	[Header("Configs")]
	[SerializeField] bool _useAnchoredPosition;

	[Header("Tween Orders")]
	[SerializeField] List<TweenOrder> _repeatableOrders;

	#endregion


	#region Private properties

	Dictionary<string, List<BaseTween>> _repeatableOrdersDict;
	private delegate float DelegatedFloat(float x);
	private readonly DelegatedFloat _tweenDelegate;
	private CancellationTokenSource _cancelTokenSource;
	
	private bool _isTweening = false;
	public bool IsTweening => _isTweening;


	#endregion


	#region Awake function

	private void Awake()
	{
		_cancelTokenSource = new();
		_repeatableOrdersDict = new();

		if (!TryGetComponent (out _rectTransformComp))
		{
			throw new Exception($"{gameObject.name} does not contains RectTransform component.");
		}

		if (!TryGetComponent(out _imageComp))
		{
			throw new Exception($"{gameObject.name} does not contains Image component.");
		}

		_originalAnchoredPosition = _rectTransformComp.anchoredPosition;
		_originalWorldPosition = _rectTransformComp.position;
		_originalCustomSize = _rectTransformComp.sizeDelta;
		_originalColor = _imageComp.color;
		_originalRotation = _rectTransformComp.rotation;

		CheckRepeatableTweenOrders();
		PopulateRepeatableTweenOrders();
	}

	#endregion


	#region Control functions

	async public Task ExecuteRepeatableOrder(string orderName)
	{
		_isTweening = true;

		List<Task> tweenTasks = new();
 
		foreach (var baseTween in _repeatableOrdersDict[orderName])
		{
			if (baseTween is AbsoluteTweenVector2 absoluteTweenVector2)
			{
				if (absoluteTweenVector2.tweenType == TweenTypes.Position)
				{
					tweenTasks.Add(AbsoluteTweenVector2_Position(absoluteTweenVector2));
				}
				else if (absoluteTweenVector2.tweenType == TweenTypes.CustomSize)
				{
					tweenTasks.Add(AbsoluteTweenVector2_CustomSize(absoluteTweenVector2));
				}
				else
				{
					throw new Exception($"Unsupported tween types: {absoluteTweenVector2.tweenType}");
				}
			}

			else if (baseTween is DirectionalMovement directionalMovement)
			{
				tweenTasks.Add(DirectionalMovementTween(directionalMovement));
			}

			else if (baseTween is SingleFloatTween singleFloatTween)
			{
				if (singleFloatTween.tweenType == TweenTypes.Transparency)
				{
					tweenTasks.Add(SingleFloatTween_Transparency(singleFloatTween));
				}
				else
				{
					throw new Exception($"Unsupported tween types: {singleFloatTween.tweenType}");
				}
			}

			else if (baseTween is RotationVector3 rotationVector3)
			{
				tweenTasks.Add(RotationVector3Tween(rotationVector3));
			}

			else
			{
				throw new Exception($"Unexpected tween preset: {baseTween.GetType()}");
			}
		}

		foreach (var task in tweenTasks)
		{
			if (!Application.isPlaying)
			{
				_cancelTokenSource.Cancel();

				_rectTransformComp.anchoredPosition = _originalAnchoredPosition;
				_rectTransformComp.position = _originalWorldPosition;
				_rectTransformComp.sizeDelta = _originalCustomSize;
				_imageComp.color = _originalColor;
				_rectTransformComp.rotation = _originalRotation;

				return;
			}

			await task;
		}

		_isTweening = false;
	}

	public Task AbsoluteTweenVector2_Position(AbsoluteTweenVector2 absoluteTweenVector2)
	{
		Vector2 currentPosition;

		if (_useAnchoredPosition)
		{
			_lastAnchoredPosition = _rectTransformComp.anchoredPosition;
			currentPosition = _lastAnchoredPosition;
		}
		else
		{
			_lastWorldPosition = _rectTransformComp.position;
			currentPosition = _lastWorldPosition;
		}

		return TweenPosition(
				currentPosition,
				absoluteTweenVector2.finalValue,
				absoluteTweenVector2.tweenFormula,
				absoluteTweenVector2.duration,
				_cancelTokenSource.Token);
	}

	public Task AbsoluteTweenVector2_CustomSize(AbsoluteTweenVector2 absoluteTweenVector2)
	{
		_lastCustomSize = _rectTransformComp.sizeDelta;
		Vector2 currentSize = _lastCustomSize;

		return TweenPosition(
				currentSize,
				absoluteTweenVector2.finalValue,
				absoluteTweenVector2.tweenFormula,
				absoluteTweenVector2.duration,
				_cancelTokenSource.Token);
	}

	public Task DirectionalMovementTween(DirectionalMovement directionalMovement)
	{
		Vector2 currentPosition;

		if (_useAnchoredPosition)
		{
			_lastAnchoredPosition = _rectTransformComp.anchoredPosition;
			currentPosition = _lastAnchoredPosition;
		}
		else
		{
			_lastWorldPosition = _rectTransformComp.position;
			currentPosition = _lastWorldPosition;
		}

		if (directionalMovement.direction == MovementDirection.MoveLeftToRight)
		{
			return TweenPosition(
				currentPosition,
				new Vector2(currentPosition.x + directionalMovement.movementValue, currentPosition.y),
				directionalMovement.tweenFormula,
				directionalMovement.duration,
				_cancelTokenSource.Token);

		}
		else if (directionalMovement.direction == MovementDirection.MoveRightToLeft)
		{
			return TweenPosition(
				currentPosition,
				new Vector2(currentPosition.x - directionalMovement.movementValue, currentPosition.y),
				directionalMovement.tweenFormula,
				directionalMovement.duration,
				_cancelTokenSource.Token);
		}
		else if (directionalMovement.direction == MovementDirection.MoveTopToBottom)
		{
			return TweenPosition(
				currentPosition,
				new Vector2(currentPosition.x, currentPosition.y - directionalMovement.movementValue),
				directionalMovement.tweenFormula,
				directionalMovement.duration,
				_cancelTokenSource.Token);
		}
		else if (directionalMovement.direction == MovementDirection.MoveBottomToTop)
		{
			return TweenPosition(
				currentPosition,
				new Vector2(currentPosition.x, currentPosition.y + directionalMovement.movementValue),
				directionalMovement.tweenFormula,
				directionalMovement.duration,
				_cancelTokenSource.Token);
		}
		else
		{
			throw new Exception($"Unexpected direction: {directionalMovement.direction}");
		}
	}

	public Task SingleFloatTween_Transparency(SingleFloatTween singleFloatTween)
	{
		return TweenTransparent(
			singleFloatTween.initialValue,
			singleFloatTween.finalValue,
			singleFloatTween.tweenFormula,
			singleFloatTween.duration,
			_cancelTokenSource.Token);
	}

	public Task RotationVector3Tween(RotationVector3 rotationVector3)
	{
		Vector3 currentRotation, finalRotation;

		float simplifiedX = 0f, simplifiedY = 0f, simplifiedZ = 0f,
			xCoefficiency = 1f, yCoefficiency = 1f, zCoefficiency = 1f;

		currentRotation = _rectTransformComp.rotation.eulerAngles;
		finalRotation = rotationVector3.additionalRotationValue;

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

		if (rotationVector3.isXClockwise)
		{
			xCoefficiency = -1f;
		}
		if (rotationVector3.isYClockwise)
		{
			yCoefficiency = -1f;
		}
		if (rotationVector3.isZClockwise)
		{
			zCoefficiency = -1f;
		}

		if (rotationVector3.xFullRotationTime > 0 && simplifiedX > 0f)
		{
			finalRotation.x = currentRotation.x + (rotationVector3.xFullRotationTime * xCoefficiency * 360f + simplifiedX);
		}
		else
		{
			finalRotation.x = currentRotation.x + (simplifiedX * xCoefficiency);
		}

		if (rotationVector3.yFullRotationTime > 0 && simplifiedY > 0f)
		{
			finalRotation.y = currentRotation.y + (rotationVector3.yFullRotationTime * yCoefficiency * 360f + simplifiedY);
		}
		else
		{
			finalRotation.y = currentRotation.y + (simplifiedY * yCoefficiency);
		}

		if (rotationVector3.zFullRotationTime > 0 && simplifiedZ > 0f)
		{
			finalRotation.z = currentRotation.z + (rotationVector3.zFullRotationTime * zCoefficiency * 360f + simplifiedZ);
		}
		else
		{
			finalRotation.z = currentRotation.z + (simplifiedZ * zCoefficiency);
		}

		return TweenRotation(
			currentRotation, 
			finalRotation, 
			rotationVector3.tweenFormula, 
			rotationVector3.duration, 
			_cancelTokenSource.Token);
	}

	public void SkipTween()
	{
		_cancelTokenSource.Cancel();

		_isTweening = false;

		_cancelTokenSource.Dispose();
		_cancelTokenSource = new();
	}

	public void CancelTween()
	{
		_cancelTokenSource.Cancel();

		if (_useAnchoredPosition)
		{
			_rectTransformComp.anchoredPosition = _lastAnchoredPosition;
		}
		else
		{
			_rectTransformComp.position = _lastWorldPosition;
		}

		_imageComp.color = _lastColor;

		_rectTransformComp.rotation = _lastRotation;

		_rectTransformComp.sizeDelta = _lastCustomSize;

		_isTweening = false;

		_cancelTokenSource = new();
	}

	#endregion


	#region Tween functions

	async public Task TweenPosition(Vector2 currentPosition, Vector2 targetPosition, TweenFormulas tweenFormula, float duration, CancellationToken cancellationToken)
	{
		try
		{
			if (_useAnchoredPosition)
			{
				_lastAnchoredPosition = _rectTransformComp.anchoredPosition;
			}
			else
			{
				_lastWorldPosition = _rectTransformComp.position;
			}

			float elapsedTime = 0f;

			if (duration < 0f)
			{
				throw new Exception("Invalid value: Duration cannot be smaller than 0.");
			}

			while (elapsedTime < duration)
			{
				cancellationToken.ThrowIfCancellationRequested();

				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				Vector2 newPosition = currentPosition + (targetPosition - currentPosition) * tweenValue;

				if (_useAnchoredPosition)
				{
					_rectTransformComp.anchoredPosition = newPosition;
				}
				else
				{
					_rectTransformComp.position = newPosition;
				}

				elapsedTime += Time.deltaTime;

				await Task.Yield();
			}
		}
		catch (OperationCanceledException)
		{
			if (_useAnchoredPosition)
			{
				_rectTransformComp.anchoredPosition = targetPosition;

				Debug.Log($"anchoredPosition: {_rectTransformComp.anchoredPosition}; targetPosition: {targetPosition}");

				//OperationCanceledException
			}
			else
			{
				_rectTransformComp.position = targetPosition;

				Debug.Log($"position: {_rectTransformComp.position}; targetPosition: {targetPosition}");
			}

			Debug.Log($"{gameObject.name} TweenPosition disrupted");
		}

		if (_useAnchoredPosition)
		{
			_rectTransformComp.anchoredPosition = targetPosition;

			Debug.Log($"anchoredPosition: {_rectTransformComp.anchoredPosition}; targetPosition: {targetPosition}");
		}
		else
		{
			_rectTransformComp.position = targetPosition;

			Debug.Log($"position: {_rectTransformComp.position}; targetPosition: {targetPosition}");
		}
	}

	async public Task TweenCustomSize(Vector2 currentSize, Vector2 targetSize, TweenFormulas tweenFormula, float duration, CancellationToken cancellationToken)
	{
		try
		{
			_lastCustomSize = _rectTransformComp.sizeDelta;

			float elapsedTime = 0f;

			if (duration < 0f)
			{
				throw new Exception("Invalid value: Duration cannot be smaller than 0.");
			}
			while (elapsedTime < duration)
			{
				cancellationToken.ThrowIfCancellationRequested();

				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				Vector2 newCustomSize = currentSize + (targetSize - currentSize) * tweenValue;

				_rectTransformComp.sizeDelta = newCustomSize;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

		}
		catch (OperationCanceledException)
		{
			Debug.Log($"{gameObject.name} TweenCustomSize disrupted");
		}

		_rectTransformComp.sizeDelta = targetSize;
	}

	async public Task TweenTransparent(float currentAlpha, float finalAlpha, TweenFormulas tweenFormula, float duration, CancellationToken cancellationToken)
	{
		try
		{
			_lastColor = _imageComp.color;

			float elapsedTime = 0f;

			if (duration < 0f)
			{
				throw new Exception("Invalid value: Duration cannot be smaller than 0.");
			}

			while (elapsedTime < duration)
			{
				cancellationToken.ThrowIfCancellationRequested();

				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				float newAlpha = currentAlpha + (finalAlpha - currentAlpha) * tweenValue;

				_imageComp.color = new Color(_imageComp.color.r, _imageComp.color.g, _imageComp.color.b, newAlpha);

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
		}
		catch (OperationCanceledException)
		{
			Debug.Log($"{gameObject.name} TweenTransparent disrupted");
		}

		_imageComp.color = new Color(_imageComp.color.r, _imageComp.color.g, _imageComp.color.b, finalAlpha);
	}

	async public Task TweenRotation(Vector3 currentRotation, Vector3 finalRotation, TweenFormulas tweenFormula, float duration, CancellationToken cancellationToken)
	{
		try
		{
			_lastRotation = _rectTransformComp.rotation;

			float elapsedTime = 0f;

			if (duration < 0f)
			{
				throw new Exception("Invalid value: Duration cannot be smaller than 0.");
			}

			while (elapsedTime < duration)
			{
				cancellationToken.ThrowIfCancellationRequested();

				float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
				Vector3 newRotation = currentRotation + (finalRotation - currentRotation) * tweenValue;

				Quaternion newQuaternion = Quaternion.Euler(newRotation);

				_rectTransformComp.rotation = newQuaternion;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}
		}
		catch (OperationCanceledException)
		{
		}

		_rectTransformComp.rotation = Quaternion.Euler(finalRotation);
	}

	#endregion


	#region Helper functions

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

	private void CheckRepeatableTweenOrders()
	{
		foreach (var item in _repeatableOrders)
		{
			int posCount = 0,
				sizeCount = 0,
			transparencyCount = 0;

			foreach (var subItem in item.Tweens)
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
					case TweenTypes.CustomSize:
						sizeCount++;
						break;
					case TweenTypes.Transparency:
						transparencyCount++;
						break;
				}

				if (posCount > 1 || sizeCount > 1 || transparencyCount > 1)
				{
					throw new Exception("Invalid data: Multiple calls of the same tween type at the same time is not supported");
				}
			}
		}
		
	}

	public void PopulateRepeatableTweenOrders()
	{
		foreach (var order in _repeatableOrders)
		{
			_repeatableOrdersDict.Add(order.OrderName, order.Tweens);
		}
	}

	#endregion
}

[Serializable]
public class TweenOrder
{
	private readonly MonoBehaviour _caller;

	[SerializeField] string _orderName;
	public string OrderName => _orderName;

	[SerializeField] List<BaseTween> _tweens;
	public List<BaseTween> Tweens => _tweens;

	public TweenOrder(MonoBehaviour caller)
	{
		_caller = caller;
	}

	public void AddTween(BaseTween tween)
	{
		_tweens.Add(tween);
	}
}

public enum Direction
{
    ToLeft,
    ToRight,
    ToUp,
    ToDown
}

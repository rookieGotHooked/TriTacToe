using System;
using System.Threading.Tasks;
using UnityEngine;

public class LinearTransformTween: MonoBehaviour 
{
	private RectTransform _rectTransform;
	private Transform _transform;

	[Header("Usage Options")]
	[SerializeField] bool _isFunctionCallsOnly;
	public bool IsFunctionCallsOnly { get => _isFunctionCallsOnly; }

	[SerializeField] bool _useAnchoredPosition;
	public bool UseAnchoredPosition { get => _useAnchoredPosition; }

	[Header("Tweens")]
	[SerializeField] PositionTween _positionTween;
	[SerializeField] CustomSizeTween _customSizeTween;
	[SerializeField] ScaleTween _scaleTween;

	private Vector2 _initialPosition;
	private Vector2 _initialCustomSize;
	private Vector2 _initialScale;

	private delegate float DelegatedFloat(float x);
	private DelegatedFloat _tweenPositionDelegate;
	private DelegatedFloat _tweenCustomSizeDelegate;
	private DelegatedFloat _tweenScaleDelegate;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();

		if (!_isFunctionCallsOnly)
		{
			if (_rectTransform == null)
			{
				throw new Exception($"RectTransform component is null;\nGameObject: {gameObject}");
			}
			else if (!_positionTween.IsUse && !_customSizeTween.IsUse && !_scaleTween.IsUse)
			{
				throw new Exception($"No tweening configuration received\nGameObject: {gameObject}");
			}
			else if (_positionTween.IsUse && _positionTween.Duration == 0f)
			{
				throw new Exception($"Tween position duration cannot be 0\nGameObject: {gameObject}");
			}
			else if (_customSizeTween.IsUse && _customSizeTween.Duration == 0f)
			{
				throw new Exception($"Tween custom size duration cannot be 0\nGameObject: {gameObject}");
			}
			else if (_scaleTween.IsUse && _scaleTween.Duration == 0f)
			{
				throw new Exception($"Tween scale duration cannot be 0\nGameObject: {gameObject}");
			}
			else
			{
				if (_useAnchoredPosition) 
				{
					_initialPosition = _rectTransform.anchoredPosition;
				}
				else
				{
					_initialPosition = _rectTransform.position;
				}

				_initialCustomSize = _rectTransform.sizeDelta;
				_initialScale = _rectTransform.localScale;
			}
		}
	}
	
	async public Task TweenPosition()
	{
		if (_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < _positionTween.Duration)
			{
				float tweenValue = TweenMapping(_tweenPositionDelegate, _positionTween.TweenFormula, elapsedTime / _positionTween.Duration);
				Vector2 newPosition = _initialPosition + (_positionTween.Target - _initialPosition) * tweenValue;

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

			if (_useAnchoredPosition)
			{
				_rectTransform.anchoredPosition = _positionTween.Target;
			}
			else
			{
				_rectTransform.position = _positionTween.Target;
			}
		}
	}

	async public Task TweenPosition(Vector2 currentPosition, Vector2 targetPosition, TweenFormulas TweenFormulas, float duration)
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

	async public Task TweenPositionReverse()
	{
		if (_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < _positionTween.Duration)
			{
				float tweenValue = TweenMapping(_tweenPositionDelegate, _positionTween.TweenFormula, elapsedTime / _positionTween.Duration);
				Vector2 newPosition = _positionTween.Target + (_initialPosition - _positionTween.Target) * tweenValue;

				if (_useAnchoredPosition)
				{
					_rectTransform.localPosition = newPosition;
				}
				else
				{
					_rectTransform.position = newPosition;
				}

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

			if (_useAnchoredPosition)
			{
				_rectTransform.localPosition = _positionTween.Target;
			}
			else
			{
				_rectTransform.position = _positionTween.Target;
			}
		}
	}

	async public Task TweenCustomSize()
	{
		if (!_rectTransform)
		{
			throw new Exception("Transform component is found on the game object;" +
				"\nTransform component does contain definition of sizeDelta for tweening");
		}
		else if (_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < _customSizeTween.Duration)
			{
				float tweenValue = TweenMapping(_tweenCustomSizeDelegate, _customSizeTween.TweenFormula, elapsedTime / _customSizeTween.Duration);
				Vector2 newCustomSize = _initialCustomSize + (_customSizeTween.Target - _initialCustomSize) * tweenValue;

				_rectTransform.sizeDelta = newCustomSize;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

			_rectTransform.sizeDelta = _customSizeTween.Target;
		}
	}

	async public Task TweenCustomSize(Vector2 currentSize, Vector2 targetSize, TweenFormulas TweenFormulas, float duration)
	{
		if (!_rectTransform)
		{
			throw new Exception("Transform component is found on the game object;" +
				"\nTransform component does contain definition of sizeDelta for tweening");
		}
		else if (!_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < duration)
			{
				float tweenValue = TweenMapping(_tweenCustomSizeDelegate, TweenFormulas, elapsedTime / _customSizeTween.Duration);
				Vector2 newCustomSize = currentSize + (targetSize - currentSize) * tweenValue;

				_rectTransform.sizeDelta = newCustomSize;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

			_rectTransform.sizeDelta = targetSize;
		}
	}

	async public Task TweenCustomSizeReverse()
	{
		if (!_rectTransform)
		{
			throw new Exception("Transform component is found on the game object;" +
				"\nTransform component does contain definition of sizeDelta for tweening");
		}
		else if (_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < _customSizeTween.Duration)
			{
				float tweenValue = TweenMapping(_tweenCustomSizeDelegate, _customSizeTween.TweenFormula, elapsedTime / _customSizeTween.Duration);
				Vector2 newCustomSize = _customSizeTween.Target + (_initialCustomSize - _customSizeTween.Target) * tweenValue;

				_rectTransform.sizeDelta = newCustomSize;

				elapsedTime += Time.deltaTime;
				await Task.Yield();
			}

			_rectTransform.sizeDelta = _initialCustomSize;
		}
	}

	async public Task TweenScale()
	{
		if (_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < _scaleTween.Duration)
			{
				float tweenValue = TweenMapping(_tweenScaleDelegate, _scaleTween.TweenFormula, elapsedTime / _scaleTween.Duration);
				Vector2 newScale = _initialScale + (_scaleTween.Target - _initialScale) * tweenValue;

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

			if (_rectTransform)
			{
				_rectTransform.localScale = _scaleTween.Target;
			}
			else
			{
				_transform.localScale = _scaleTween.Target;
			}
		}
		
	}

	async public Task TweenScaleReverse()
	{
		if (_isFunctionCallsOnly)
		{
			throw new Exception("Invalid call: Component is set to function call only");
		}
		else
		{
			float elapsedTime = 0f;

			while (elapsedTime < _scaleTween.Duration)
			{
				float tweenValue = TweenMapping(_tweenScaleDelegate, _scaleTween.TweenFormula, elapsedTime / _scaleTween.Duration);
				Vector2 newScale = _scaleTween.Target + (_initialScale - _scaleTween.Target) * tweenValue;

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

			if (_rectTransform)
			{
				_rectTransform.localScale = _initialScale;
			}
			else
			{
				_transform.localScale = _initialScale;
			}
		}
	}

	private float TweenMapping(DelegatedFloat delegatedFloat, TweenFormulas selectedTweenType, float value)
	{
		switch (selectedTweenType)
		{
			case TweenFormulas.EaseInSine:
				delegatedFloat = TweenMethods.EaseInSine;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutSine:
				delegatedFloat = TweenMethods.EaseOutSine;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutSine:
				delegatedFloat = TweenMethods.EaseInOutSine;
				return delegatedFloat(value);
			case TweenFormulas.EaseInQuad:
				delegatedFloat = TweenMethods.EaseInQuad;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutQuad:
				delegatedFloat = TweenMethods.EaseOutQuad;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutQuad:
				delegatedFloat = TweenMethods.EaseInOutQuad;
				return delegatedFloat(value);
			case TweenFormulas.EaseInCubic:
				delegatedFloat = TweenMethods.EaseInCubic;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutCubic:
				delegatedFloat = TweenMethods.EaseOutCubic;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutCubic:
				delegatedFloat = TweenMethods.EaseInOutCubic;
				return delegatedFloat(value);
			case TweenFormulas.EaseInQuart:
				delegatedFloat = TweenMethods.EaseInQuart;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutQuart:
				delegatedFloat = TweenMethods.EaseOutQuart;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutQuart:
				delegatedFloat = TweenMethods.EaseInOutQuart;
				return delegatedFloat(value);
			case TweenFormulas.EaseInQuint:
				delegatedFloat = TweenMethods.EaseInQuint;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutQuint:
				delegatedFloat = TweenMethods.EaseOutQuint;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutQuint:
				delegatedFloat = TweenMethods.EaseInOutQuint;
				return delegatedFloat(value);
			case TweenFormulas.EaseInExpo:
				delegatedFloat = TweenMethods.EaseInExpo;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutExpo:
				delegatedFloat = TweenMethods.EaseOutExpo;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutExpo:
				delegatedFloat = TweenMethods.EaseInOutExpo;
				return delegatedFloat(value);
			case TweenFormulas.EaseInCirc:
				delegatedFloat = TweenMethods.EaseInCirc;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutCirc:
				delegatedFloat = TweenMethods.EaseOutCirc;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutCirc:
				delegatedFloat = TweenMethods.EaseInOutCirc;
				return delegatedFloat(value);
			case TweenFormulas.EaseInBack:
				delegatedFloat = TweenMethods.EaseInBack;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutBack:
				delegatedFloat = TweenMethods.EaseOutBack;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutBack:
				delegatedFloat = TweenMethods.EaseInOutBack;
				return delegatedFloat(value);
			case TweenFormulas.EaseInElastic:
				delegatedFloat = TweenMethods.EaseInElastic;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutElastic:
				delegatedFloat = TweenMethods.EaseOutElastic;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutElastic:
				delegatedFloat = TweenMethods.EaseInOutElastic;
				return delegatedFloat(value);
			case TweenFormulas.EaseInBounce:
				delegatedFloat = TweenMethods.EaseInBounce;
				return delegatedFloat(value);
			case TweenFormulas.EaseOutBounce:
				delegatedFloat = TweenMethods.EaseOutBounce;
				return delegatedFloat(value);
			case TweenFormulas.EaseInOutBounce:
				delegatedFloat = TweenMethods.EaseInOutBounce;
				return delegatedFloat(value);
			default:
				throw new Exception($"Unexpected TweenFormulas detected: {selectedTweenType}");
		}
	}
}

[Serializable]
public class PositionTween
{
	[SerializeField] [HideInInspector] string _id = Guid.NewGuid().ToString();
	public string ID { get => _id; }

	[SerializeField] bool _isUse = false;
	public bool IsUse { get => _isUse; }

	[SerializeField] TweenFormulas _tweenFormula;
	public TweenFormulas TweenFormula { get => _tweenFormula; }

	[SerializeField] Vector2 _target;
	public Vector2 Target { get => _target; }

	[SerializeField] float _duration;
	public float Duration { get => _duration; }
}

[Serializable]
public class CustomSizeTween
{
	[SerializeField][HideInInspector] string _id = Guid.NewGuid().ToString();
	public string ID { get => _id; }

	[SerializeField] bool _isUse = false;
	public bool IsUse { get => _isUse; }

	[SerializeField] TweenFormulas _tweenFormula;
	public TweenFormulas TweenFormula { get => _tweenFormula; }

	[SerializeField] Vector2 _target;
	public Vector2 Target { get => _target; }

	[SerializeField] float _duration;
	public float Duration { get => _duration; }
}

[Serializable]
public class ScaleTween
{
	[SerializeField][HideInInspector] string _id = Guid.NewGuid().ToString();
	public string ID { get => _id; }

	[SerializeField] bool _isUse = false;
	public bool IsUse { get => _isUse; }

	[SerializeField] TweenFormulas _tweenFormula;
	public TweenFormulas TweenFormula { get => _tweenFormula; }

	[SerializeField] Vector2 _target;
	public Vector2 Target { get => _target; }

	[SerializeField] float _duration;
	public float Duration { get => _duration; }
}
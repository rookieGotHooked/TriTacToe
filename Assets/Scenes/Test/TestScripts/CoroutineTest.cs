using System;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CoroutineTest : MonoBehaviour
{
    RectTransform _rectTransform;
	Vector2 _currentPosition;
	Vector3 _currentRotation;

	[SerializeField] float _time;
	[SerializeField] float _movementValue;
	[SerializeField] Vector3 _newRotationAngles;
	[SerializeField] TweenFormulas _formula;

	private delegate float DelegatedFloat(float x);
	private readonly DelegatedFloat _tweenDelegate;

	private bool _isRunning = false;
	private bool _isRotating = false;

	private AudioSource _audioSource;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_currentPosition = _rectTransform.anchoredPosition;
		_currentRotation = _rectTransform.rotation.eulerAngles;

		_audioSource = GetComponent<AudioSource>();
	}

	public void DoTweenCoroutine()
	{
		MoveUpCoroutine();
		DoRotateCoroutine();

		_audioSource.Play();
	}

	public async void DoTweenAsync()
	{
		List<Task> tweenTasks = new()
		{
			MoveUpAsync(),
			DoRotateAsync()
		};

		_audioSource.Play();

		foreach (var task in tweenTasks)
		{
			await task;
		}
	}

	public void MoveUpCoroutine()
	{
		if (!_isRunning)
		{
			StartCoroutine(TweenPositionCoroutine(new Vector2(_currentPosition.x, _currentPosition.y + _movementValue), _formula, _time));
		}
	}

	public void DoRotateCoroutine()
	{
		if (!_isRotating)
		{
			StartCoroutine(TweenRotationCoroutine(_newRotationAngles, _formula, _time));
		}
	}

	async public Task MoveUpAsync()
	{
		if (!_isRunning)
		{
			await TweenPositionAsync(new Vector2(_currentPosition.x, _currentPosition.y + _movementValue), _formula, _time);
		}
		else
		{
			return;
		}
	}

	async public Task DoRotateAsync()
	{
		if (!_isRotating)
		{
			await TweenRotationAsync(_newRotationAngles, _formula, _time);
		}
		else
		{
			return;
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

	private IEnumerator TweenPositionCoroutine(Vector2 targetPosition, TweenFormulas tweenFormula, float duration)
	{
		_isRunning = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
			Vector2 newPosition = _currentPosition + (targetPosition - _currentPosition) * tweenValue;

			_rectTransform.anchoredPosition = newPosition;

			elapsedTime += Time.deltaTime;
			
			yield return null;
		}
			
		_rectTransform.anchoredPosition = targetPosition;
		_currentPosition = targetPosition;

		_isRunning = false;
	}


	private IEnumerator TweenRotationCoroutine(Vector3 finalRotation, TweenFormulas tweenFormula, float duration)
	{
		_isRotating = true;

		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
			Vector3 newRotation = _currentRotation + (finalRotation - _currentRotation) * tweenValue;

			Quaternion newQuaternion = Quaternion.Euler(newRotation);

			_rectTransform.rotation = newQuaternion;

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		_rectTransform.rotation = Quaternion.Euler(finalRotation);

		_isRotating = false;
	}


	private async Task TweenPositionAsync(Vector2 targetPosition, TweenFormulas tweenFormula, float duration)
	{
		_isRunning = true;

		float elapsedTime = 0f;

		if (duration < 0f)
		{
			throw new Exception("Invalid value: Duration cannot be smaller than 0.");
		}

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
			Vector2 newPosition = _currentPosition + (targetPosition - _currentPosition) * tweenValue;

			_rectTransform.anchoredPosition = newPosition;

			elapsedTime += Time.deltaTime;

			await Task.Yield();
		}

		_rectTransform.anchoredPosition = targetPosition;
		_currentPosition = targetPosition;

		_isRunning = false;
	}

	private async Task TweenRotationAsync(Vector3 finalRotation, TweenFormulas tweenFormula, float duration)
	{
		_isRotating = true;

		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMapping(_tweenDelegate, tweenFormula, elapsedTime / duration);
			Vector3 newRotation = _currentRotation + (finalRotation - _currentRotation) * tweenValue;

			Quaternion newQuaternion = Quaternion.Euler(newRotation);

			_rectTransform.rotation = newQuaternion;

			elapsedTime += Time.deltaTime;

			await Task.Yield();
		}

		_rectTransform.rotation = Quaternion.Euler(finalRotation);

		_isRotating = false;
	}
}

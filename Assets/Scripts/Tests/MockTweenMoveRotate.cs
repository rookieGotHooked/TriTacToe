using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MockTweenMoveRotate : MonoBehaviour
{
    private RectTransform _rectTransform;
	private Button _buttonComponent;
	//private bool _isTweening = false;
	private Image _imageComponent;
	private const float _duration = 1.5f;

	private Vector2 _originalPosition;
	private Vector3 _originalRotation;


	private void Awake()
	{
		if (!TryGetComponent(out _rectTransform))
        {
            throw new System.Exception("Need RectTransform!");
        }

		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception("Need Image!");
		}

		if (!gameObject.transform.parent.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception("Need Button!");
		}

		_originalPosition = _rectTransform.anchoredPosition;
		_originalRotation = _rectTransform.rotation.eulerAngles;

		_buttonComponent.onClick.AddListener(delegate  { _ = Tween(); });
	}

	async private void Start()
	{
		await MoveAndRotate();
	}

	async public Task Tween()
	{
		await ResetAll();
		await MoveAndRotate();
	}

	async public Task ResetAll()
	{
		_rectTransform.anchoredPosition = _originalPosition;
		_rectTransform.rotation = Quaternion.Euler(_originalRotation);
		_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, 0f);

		await Task.Delay(1000);
	}

	async public Task MoveAndRotate()
	{
		List<Task> tasks = new()
		{
			MoveUp(_originalPosition, new Vector2(_originalPosition.x, _originalPosition.y + 100f), _duration),
			RotateCounterClockwise(_originalRotation, new Vector3(_originalRotation.x, _originalRotation.y, _originalRotation.z - 180f), _duration),
			TransparentAppear(0f, 255f, _duration)
		};

		foreach (var task in tasks)
		{
			await task;
		}
	}

	async public Task MoveUp(Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float tweenValue = TweenMethods.EaseOutQuint(elapsedTime / duration);

            _rectTransform.anchoredPosition = start + (end - start) * tweenValue;

            elapsedTime += Time.deltaTime;

			await Task.Yield();
		}

        _rectTransform.anchoredPosition = end;
    }

	async public Task RotateCounterClockwise(Vector3 start, Vector3 end, float duration)
	{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenValue = TweenMethods.EaseOutElastic(elapsedTime / duration);

			Vector3 newRotationV3 = start + (end - start) * tweenValue;
			Quaternion newRotationQuat = Quaternion.Euler(newRotationV3);

			_rectTransform.rotation = newRotationQuat;

			elapsedTime += Time.deltaTime;

			await Task.Yield();
		}

		_rectTransform.rotation = Quaternion.Euler(end);
	}

	async public Task TransparentAppear(float start, float end, float duration)
	{
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float tweenedValue = TweenMethods.EaseInSine(elapsedTime / duration);
			float transparencyValue = start + (end - start) * tweenedValue;

			_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, transparencyValue);

			elapsedTime += Time.deltaTime;

			await Task.Yield();
		}

		_imageComponent.color = new Color(_imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, end);
	}
}

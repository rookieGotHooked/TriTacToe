using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectController : MonoBehaviour
{
    Button _chevronLeftButtonComponent;
	Button _chevronRightButtonComponent;

    Tweens2D _chevronLeftTweenComponent;
	Tweens2D _chevronRightTweenComponent;

	RectTransform _chevronLeftRectTransformComponent;
	RectTransform _chevronRightRectTransformComponent;

	RectTransform _rectTransformComponent;

	Vector2 _originalPosition;

	Vector2 _chevronLeftOriginalPosition;
	Vector2 _chevronRightOriginalPosition;

	Image _chevronLeftImageComponent;
	Image _chevronRightImageComponent;

	Image _backgroundImageComponent;
	Image _randomTextImageComponent;
	Image _normalTextImageComponent;
	Image _hardTextImageComponent;

	Tweens2D _randomTextTweenComponent;
    Tweens2D _normalTextTweenComponent;
    Tweens2D _hardTextTweenComponent;

	AIDifficulties _currentDifficulty = AIDifficulties.Random;
	public AIDifficulties CurrentDifficulty => _currentDifficulty;

	private void Awake()
	{
        GameObject chevronLeftButtonGameObject = transform.GetChild(1).gameObject;
		GameObject chevronRightButtonGameObject = transform.GetChild(2).gameObject;
		GameObject textMaskGameObject = transform.GetChild(3).gameObject;

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!chevronLeftButtonGameObject.TryGetComponent(out _chevronLeftButtonComponent))
        {
            throw new System.Exception($"{chevronLeftButtonGameObject.name} does not contains Button component");
		}

		_chevronLeftButtonComponent.onClick.AddListener(ChevronLeftOnClick);

		if (!chevronLeftButtonGameObject.TryGetComponent(out _chevronLeftTweenComponent))
		{
			throw new System.Exception($"{chevronLeftButtonGameObject.name} does not contains Tweens2D component");
		}

		if (!chevronLeftButtonGameObject.TryGetComponent(out _chevronLeftRectTransformComponent))
		{
			throw new System.Exception($"{chevronLeftButtonGameObject.name} does not contains RectTransform component");
		}

		_chevronLeftOriginalPosition = _chevronLeftRectTransformComponent.anchoredPosition;

		if (!chevronLeftButtonGameObject.TryGetComponent(out _chevronLeftImageComponent))
		{
			throw new System.Exception($"{chevronLeftButtonGameObject.name} does not contains Image component");
		}


		if (!chevronRightButtonGameObject.TryGetComponent(out _chevronRightButtonComponent))
		{
			throw new System.Exception($"{chevronRightButtonGameObject.name} does not contains Button component");
		}

		_chevronRightButtonComponent.onClick.AddListener(ChevronRightOnClick);

		if (!chevronRightButtonGameObject.TryGetComponent(out _chevronRightTweenComponent))
		{
			throw new System.Exception($"{chevronRightButtonGameObject.name} does not contains Tweens2D component");
		}

		if (!chevronRightButtonGameObject.TryGetComponent(out _chevronRightRectTransformComponent))
		{
			throw new System.Exception($"{chevronRightButtonGameObject.name} does not contains RectTransform component");
		}

		_chevronRightOriginalPosition = _chevronRightRectTransformComponent.anchoredPosition;

		if (!chevronRightButtonGameObject.TryGetComponent(out _chevronRightImageComponent))
		{
			throw new System.Exception($"{chevronLeftButtonGameObject.name} does not contains Image component");
		}

		if (!transform.GetChild(0).gameObject.TryGetComponent(out _backgroundImageComponent))
		{
			throw new System.Exception($"{transform.GetChild(0).gameObject} does not contains Image component");
		}


		if (!textMaskGameObject.transform.GetChild(0).gameObject.TryGetComponent(out _randomTextTweenComponent))
        {
            throw new System.Exception($"{textMaskGameObject.transform.GetChild(0).gameObject.name} does not contains Tweens2D component");
        }

		if (!textMaskGameObject.transform.GetChild(0).gameObject.TryGetComponent(out _randomTextImageComponent))
		{
			throw new System.Exception($"{textMaskGameObject.transform.GetChild(0).gameObject.name} does not contains Image component");
		}


		if (!textMaskGameObject.transform.GetChild(1).gameObject.TryGetComponent(out _normalTextTweenComponent))
		{
			throw new System.Exception($"{textMaskGameObject.transform.GetChild(1).gameObject.name} does not contains Tweens2D component");
		}

		if (!textMaskGameObject.transform.GetChild(1).gameObject.TryGetComponent(out _normalTextImageComponent))
		{
			throw new System.Exception($"{textMaskGameObject.transform.GetChild(1).gameObject.name} does not contains Image component");
		}


		if (!textMaskGameObject.transform.GetChild(2).gameObject.TryGetComponent(out _hardTextTweenComponent))
		{
			throw new System.Exception($"{textMaskGameObject.transform.GetChild(2).gameObject.name} does not contains Tweens2D component");
		}

		if (!textMaskGameObject.transform.GetChild(2).gameObject.TryGetComponent(out _hardTextImageComponent))
		{
			throw new System.Exception($"{textMaskGameObject.transform.GetChild(2).gameObject.name} does not contains Image component");
		}
	}

	async private void Start()
	{
		//Debug.Log("DifficultySelectController Start() called");

		_chevronRightImageComponent.color = new(
			_chevronRightImageComponent.color.r,
			_chevronRightImageComponent.color.g,
			_chevronRightImageComponent.color.b,
			255f);

		await _chevronRightTweenComponent.ExecuteTweenOrders("Move In");
		ChevronLeftResetPosition();
	}

	public void ChevronLeftResetPosition()
	{
		_chevronLeftRectTransformComponent.anchoredPosition = _chevronLeftOriginalPosition;

		_chevronLeftImageComponent.color = new (
			_chevronLeftImageComponent.color.r,
			_chevronLeftImageComponent.color.g,
			_chevronLeftImageComponent.color.b,
			0f);

		_chevronLeftButtonComponent.interactable = false;
	}

	public void ChevronRightResetPosition()
	{
		_chevronRightRectTransformComponent.anchoredPosition = _chevronRightOriginalPosition;

		_chevronRightImageComponent.color = new (
			_chevronRightImageComponent.color.r,
			_chevronRightImageComponent.color.g,
			_chevronRightImageComponent.color.b,
			0f);

		_chevronRightButtonComponent.interactable = false;
	}

	async public void ChevronLeftOnClick()
	{
		_chevronLeftButtonComponent.interactable = false;

		List<Task> tweenTasks = new();

		if (_currentDifficulty == AIDifficulties.Normal)
		{
			tweenTasks.Add(_chevronLeftTweenComponent.ExecuteTweenOrders("Move Out"));

			_currentDifficulty = AIDifficulties.Random;
		}
		else if (_currentDifficulty == AIDifficulties.Hard)
		{
			_chevronRightImageComponent.color = new (
				_chevronRightImageComponent.color.r,
				_chevronRightImageComponent.color.g,
				_chevronRightImageComponent.color.b,
				255f);

			_chevronRightButtonComponent.interactable = true;

			tweenTasks.Add(_chevronRightTweenComponent.ExecuteTweenOrders("Move In"));

			_currentDifficulty = AIDifficulties.Normal;
		}

		tweenTasks.Add(_randomTextTweenComponent.ExecuteTweenOrders("Move Left To Right"));
		tweenTasks.Add(_normalTextTweenComponent.ExecuteTweenOrders("Move Left To Right"));
		tweenTasks.Add(_hardTextTweenComponent.ExecuteTweenOrders("Move Left To Right"));

		foreach (var task in tweenTasks)
		{
			await task;
		}

		if (_currentDifficulty == AIDifficulties.Random)
		{
			ChevronLeftResetPosition();
		}

		//Debug.Log($"_currentDifficulty: {_currentDifficulty}");

		_chevronLeftButtonComponent.interactable = true;
	}

	async public void ChevronRightOnClick()
	{
		_chevronRightButtonComponent.interactable = false;

		List<Task> tweenTasks = new();

		if (_currentDifficulty == AIDifficulties.Normal)
		{
			tweenTasks.Add(_chevronRightTweenComponent.ExecuteTweenOrders("Move Out"));

			_currentDifficulty = AIDifficulties.Hard;
		}
		else if (_currentDifficulty == AIDifficulties.Random)
		{
			_chevronLeftImageComponent.color = new (
				_chevronLeftImageComponent.color.r,
				_chevronLeftImageComponent.color.g,
				_chevronLeftImageComponent.color.b,
				255f);

			_chevronLeftButtonComponent.interactable = true;
			tweenTasks.Add(_chevronLeftTweenComponent.ExecuteTweenOrders("Move In"));

			_currentDifficulty = AIDifficulties.Normal;
		}

		tweenTasks.Add(_randomTextTweenComponent.ExecuteTweenOrders("Move Right To Left"));
		tweenTasks.Add(_normalTextTweenComponent.ExecuteTweenOrders("Move Right To Left"));
		tweenTasks.Add(_hardTextTweenComponent.ExecuteTweenOrders("Move Right To Left"));

		foreach (var task in tweenTasks)
		{
			await task;
		}

		if (_currentDifficulty == AIDifficulties.Hard)
		{
			ChevronRightResetPosition();
		}

		//Debug.Log($"_currentDifficulty: {_currentDifficulty}");

		_chevronRightButtonComponent.interactable = true;
	}

	public void SetTransparencyAll(float value)
	{
		if (_currentDifficulty == AIDifficulties.Random)
		{
			_chevronLeftImageComponent.color = new(
				_chevronLeftImageComponent.color.r,
				_chevronLeftImageComponent.color.g,
				_chevronLeftImageComponent.color.b,
				0f);

			_chevronRightImageComponent.color = new(
				_chevronRightImageComponent.color.r,
				_chevronRightImageComponent.color.g,
				_chevronRightImageComponent.color.b,
				value);
		}
		else if (_currentDifficulty == AIDifficulties.Hard)
		{
			_chevronLeftImageComponent.color = new(
				_chevronLeftImageComponent.color.r,
				_chevronLeftImageComponent.color.g,
				_chevronLeftImageComponent.color.b,
				value);

			_chevronRightImageComponent.color = new(
				_chevronRightImageComponent.color.r,
				_chevronRightImageComponent.color.g,
				_chevronRightImageComponent.color.b,
				0f);
		}

		_backgroundImageComponent.color = new(
			_backgroundImageComponent.color.r,
			_backgroundImageComponent.color.g,
			_backgroundImageComponent.color.b,
			value);

		_normalTextImageComponent.color = new(
			_normalTextImageComponent.color.r,
			_normalTextImageComponent.color.g,
			_normalTextImageComponent.color.b,
			value);

		_hardTextImageComponent.color = new(
			_hardTextImageComponent.color.r,
			_hardTextImageComponent.color.g,
			_hardTextImageComponent.color.b,
			value);

		_randomTextImageComponent.color = new(
			_randomTextImageComponent.color.r,
			_randomTextImageComponent.color.g,
			_randomTextImageComponent.color.b,
			value);
	}
	public void ResetPosition()
	{
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}
}

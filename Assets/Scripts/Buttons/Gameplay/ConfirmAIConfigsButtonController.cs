using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmAIConfigsButtonController : MonoBehaviour
{
	RectTransform _rectTransformComponent;
	Button _buttonComponent;
	Image _imageComponent;

	Vector2 _originalPosition;

	private void Awake()
	{
		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		_buttonComponent.onClick.AddListener(SetAIConfigs);

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}
	}

	public void SetAIConfigs()
	{
		InitiateState initState = (InitiateState)GameManager.Instance.GameStateDict[GameStates.Initiate];

		initState.SetGameConfigs();
	}

	public void ResetPosition()
	{
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}

	public void SetTransparent(float value)
	{
		_imageComponent.color = new(
			_imageComponent.color.r,
			_imageComponent.color.g,
			_imageComponent.color.b,
			value);
	}
}

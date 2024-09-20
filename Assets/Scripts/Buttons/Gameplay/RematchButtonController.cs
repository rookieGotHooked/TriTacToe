using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RematchButtonController : MonoBehaviour
{
	GameManager _gameManager;

	private RectTransform _rectTransformComponent;
	private Tweens2D _tweenComponent;
	private Button _buttonComponent;
	private Image _imageComponent;

	private Vector2 _originalPosition;

	private void Awake()
	{
		_gameManager = GameManager.Instance;

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		if (!TryGetComponent(out _tweenComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}

		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}

		_buttonComponent.onClick.AddListener(HandleClick);
	}

	private void HandleClick()
	{
		EndGameState endState = (EndGameState)_gameManager.GameStateDict[GameStates.EndGame];
		endState.Rematch();
	}

	async public Task MoveIn()
	{
		_imageComponent.color = new Color(
			_imageComponent.color.r,
			_imageComponent.color.g,
			_imageComponent.color.b,
			255f);

		await _tweenComponent.ExecuteTweenOrders("Move In");
	}

	async public Task MoveOut()
	{
		await _tweenComponent.ExecuteTweenOrders("Move Out");

		_imageComponent.color = new Color(
			_imageComponent.color.r,
			_imageComponent.color.g,
			_imageComponent.color.b,
			0f);
	}

	public void ResetPosition()
	{
		SetTransparency(0f);
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}

	public void SetTransparency(float value)
	{
		_imageComponent.color = new Color(
			_imageComponent.color.r,
			_imageComponent.color.g,
			_imageComponent.color.b,
			value);
	}
}

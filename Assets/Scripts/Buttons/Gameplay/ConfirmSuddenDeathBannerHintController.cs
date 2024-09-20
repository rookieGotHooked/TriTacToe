using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSuddenDeathBannerHintController : MonoBehaviour
{
	GameManager _gameManager;

	private Tweens2D _tweenComponent;

	private Button _buttonComponent;

	private RectTransform _rectTransformComponent;
	Vector2 _originalPosition;

	private Image _imageComponent;

	private void Awake()
	{
		_gameManager = GameManager.Instance;

		if (!TryGetComponent(out _rectTransformComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains RectTransform component");
		}

		_originalPosition = _rectTransformComponent.anchoredPosition;

		if (!TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component");
		}

		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		if (!TryGetComponent(out _tweenComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Tweens2D component");
		}

		_buttonComponent.onClick.AddListener(OnClickHandling);
	}

	async private void OnClickHandling()
	{
		await _gameManager.HideSuddenDeathHint();
	}

	async public Task Move()
	{
		await _tweenComponent.ExecuteTweenOrders("Move Left To Right");
	}

	public void Show()
	{
		SetTransparency(_imageComponent, 255f);

		_buttonComponent.interactable = true;
	}
	public void Hide()
	{
		SetTransparency(_imageComponent, 0f);

		_buttonComponent.interactable = false;
	}

	public void SetTransparency(Image imageComponent, float transparencyValue)
	{
		imageComponent.color = new Color(
			imageComponent.color.r,
			imageComponent.color.g,
			imageComponent.color.b,
			transparencyValue);
	}

	public void ResetPosition()
	{
		_rectTransformComponent.anchoredPosition = _originalPosition;
	}
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class PressOrHoldGuideButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	private FSM_GuideManager _guideManager;
	private bool _isPressed = false;
	private Image _imageComponent;
	private bool _interactable = true;
	private Button _buttonComponent;

	private Image _screenTextImageComponent;

	private Tweens2D _screenTextTween;
	private Tweens2D _backTextTween;

	[Header("Interchangable Text Sprites")]
	[SerializeField]
	private Sprite _menuSprite;
	[SerializeField]
	private Sprite _gameSprite;

	[Header("Hold Config")]
	[SerializeField] 
	private float _pressToHoldDuration;
	[SerializeField] 
	private float _holdDuration;
	[SerializeField] 
	private GameObject _radialIndicator;
	[SerializeField] 
	private GameObject _textMask;

	[Header("Screen Transition Config")]
	[SerializeField]
	private ScreensEnum _screensEnum;
	[SerializeField]
	private ScreenTransitionCode _transitionCode;

	private void Awake()
	{
		_guideManager = FSM_GuideManager.Instance;

		GameObject screenTextGameObj = _textMask.transform.GetChild(0).gameObject;
		GameObject backTextGameObj = _textMask.transform.GetChild(1).gameObject;
		
		if (!_radialIndicator.TryGetComponent(out _imageComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Image component.");
		}

		if (_pressToHoldDuration >= _holdDuration)
		{
			throw new System.Exception("Time delay before handling hold button cannot be greater than the hold duration.");
		}

		if (!screenTextGameObj.TryGetComponent(out _screenTextTween))
		{
			throw new System.Exception($"{screenTextGameObj.name} does not contains Tweens2D component.");
		}

		if (!backTextGameObj.TryGetComponent(out _backTextTween))
		{
			throw new System.Exception($"{backTextGameObj.name} does not contains Tweens2D component.");
		}

		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		if (_menuSprite == null)
		{
			throw new System.Exception("_menuSprite is null");
		}

		if (_gameSprite == null)
		{
			throw new System.Exception("_gameSprite is null");
		}
		
		GameObject screenTextGameObject = gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
		
		if (!screenTextGameObject.TryGetComponent(out _screenTextImageComponent))
		{
			throw new System.Exception($"{screenTextGameObject.name} does not contains Image component");
		}
	}

	async public void OnPointerDown(PointerEventData eventData)
    {
		if (_interactable)
		{
			_isPressed = true;
			await HoldHandling();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isPressed = false;
	}

	async private Task HoldHandling()
	{
		float elapsedTime = 0f;

		float updateRate = 1f / (_holdDuration - _pressToHoldDuration);

		while (elapsedTime < _holdDuration)
		{
			if (_isPressed)
			{
				float time = Time.deltaTime;

				if (elapsedTime > _pressToHoldDuration)
				{
					_imageComponent.fillAmount += time * updateRate;
				}

				elapsedTime += time;
				await Task.Yield();
			}
			else
			{
				_imageComponent.fillAmount = 0f;
				await HandleNotFullHold();
				return;
			}
		}

		_imageComponent.fillAmount = 1f;
		HandleFullHold();
		_imageComponent.fillAmount = 0f;
	}
	private void RequestChangeScreen()
	{
		ScreenManager.Instance.SetNextScreen(_screensEnum, _transitionCode);
	}

	public void HandleFullHold()
	{
		RequestChangeScreen();
	}

	async public Task HandleNotFullHold()
	{
		if (_guideManager.CurrentPageKey.Equals(GuidePageEnum.GuidePage0))
		{
			RequestChangeScreen();
		}
		else
		{
			await FSM_GuideManager.Instance.ChangePreviousPage();
		}
	}

	async public Task MoveTextUp()
	{
		List<Task> tasks = new()
		{
			_screenTextTween.ExecuteTweenOrders("Move Bottom To Top"),
			_backTextTween.ExecuteTweenOrders("Move Bottom To Top")
		};

		foreach (var task in tasks)
		{
			await task;
		}
	}

	async public Task MoveTextDown()
	{
		List<Task> tasks = new()
		{
			_screenTextTween.ExecuteTweenOrders("Move Top To Bottom"),
			_backTextTween.ExecuteTweenOrders("Move Top To Bottom")
		};

		foreach (var task in tasks)
		{
			await task;
		}
	}

	public void SetInteractable(bool value)
	{
		_buttonComponent.interactable = value;
		_interactable = value;
	}

	public void SetTransition(ScreensEnum screensEnum)
	{
		switch (screensEnum)
		{
			case ScreensEnum.MainMenu:
				_screensEnum = ScreensEnum.MainMenu;
				_transitionCode = ScreenTransitionCode.GuideToMainMenu;

				_screenTextImageComponent.sprite = _menuSprite;
				break;
			case ScreensEnum.Gameplay:
				_screensEnum = ScreensEnum.Gameplay;
				_transitionCode = ScreenTransitionCode.GuideToGameplay;

				_screenTextImageComponent.sprite = _gameSprite;
				break;
			default:
				throw new System.Exception($"Unexpected ScreensEnum received: {screensEnum} at {gameObject.name}. Expected values are: ScreensEnum.MainMenu," +
					$"ScreensEnum.Gameplay");
		}
	}
}

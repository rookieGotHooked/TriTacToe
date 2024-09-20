using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitCancelButton : MonoBehaviour
{
	private enum State { Text, Icon }

	ScreenManager _screenManager;
	SettingsScreen _settingsScreen;
	Button _buttonComponent;

	Tweens2D _textTween;
    Tweens2D _iconTween;
	State _buttonState;

	private void Awake()
	{
		_screenManager = ScreenManager.Instance;
		_settingsScreen = (SettingsScreen)_screenManager.ScreenDict[ScreensEnum.Settings];

		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"{gameObject.name} does not contains Button component");
		}

		GameObject textGameObject = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
		GameObject iconGameObject = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;

		if (!textGameObject.TryGetComponent(out _textTween)) 
		{
			throw new System.Exception($"{textGameObject.name} does not contains Tweens2D component");
		}

		if (!iconGameObject.TryGetComponent(out _iconTween))
		{
			throw new System.Exception($"{iconGameObject.name} does not contains Tweens2D component");
		}

		_buttonComponent.onClick.AddListener(OnClickHandler);
	}

	async public Task TextMoveUp()
	{
		//Debug.Log("QuitCancelButton.TextMoveUp() called");

		List<Task> tasks = new()
		{
			_textTween.ExecuteTweenOrders("Move Bottom To Top"),
			_iconTween.ExecuteTweenOrders("Move Bottom To Top")
		};

		foreach (var task in tasks)
		{
			await task;
		}

		_buttonState = State.Icon;
	}

	async public Task TextMoveDown()
	{
		//Debug.Log("QuitCancelButton.TextMoveDown() called");

		List<Task> tasks = new()
		{
			_textTween.ExecuteTweenOrders("Move Top To Bottom"),
			_iconTween.ExecuteTweenOrders("Move Top To Bottom")
		};

		foreach (var task in tasks)
		{
			await task;
		}

		_buttonState = State.Text;
	}

	async public void OnClickHandler()
	{
		//Debug.Log("OnClickHandler called");

		if (_buttonState.Equals(State.Text))
		{
			await _settingsScreen.DisplayConfirmTextbox(true);
		}
		else if (_buttonState.Equals(State.Icon))
		{
			await _settingsScreen.DisplayConfirmTextbox(false);
		}
		else
		{
			throw new System.Exception($"Unexpected state detected at {gameObject.name} - value: {_buttonState}");
		}
	}

	public void SetInteractable(bool value)
	{
		_buttonComponent.interactable = value;
	}
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : BaseScreen<ScreensEnum>
{
	GameObject _confirmQuitTextbox;
	Tweens2D _confirmQuitTextboxTween;

	GameObject _quitCancelButton;
	QuitCancelButton _quitCancelButtonComponent;

	GameObject _gameConfirmButton;
	GameConfirmButton _gameConfirmButtonComponent;

	GameObject _menuButton;

	ScreenManager _screenManager;

	public SettingsScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
	{
	}

	public override void OnEnter()
	{
		if (IsInit)
		{
			DisplayButtons();
		}
	}

	async public override void OnUpdate()
	{
		if (!IsInit)
		{
			_screenManager = ScreenManager.Instance;

			await InstantiateObjects();
			SetInit();

			_confirmQuitTextbox = _staticSpritesGameObject[2];
			_menuButton = _buttonsGameObject[0];
			_quitCancelButton = _buttonsGameObject[1];
			_gameConfirmButton = _buttonsGameObject[2];


			if (!_confirmQuitTextbox.TryGetComponent(out _confirmQuitTextboxTween))
			{
				throw new Exception($"{_confirmQuitTextbox.name} does not contains Tweens2D component");
			}

			if (!_quitCancelButton.TryGetComponent(out _quitCancelButtonComponent))
			{
				throw new Exception($"{_quitCancelButton.name} does not contains Tweens2D component");
			}

			if (!_gameConfirmButton.TryGetComponent(out _gameConfirmButtonComponent))
			{
				throw new Exception($"{_gameConfirmButton.name} does not contains Tweens2D component");
			}

			DisplayButtons();
		}
	}

	public void DisplayButtons()
	{
		if (_screenManager.PreviousScreen.Equals(ScreensEnum.Gameplay))
		{
			_menuButton.SetActive(false);

			_quitCancelButton.SetActive(true);
			_gameConfirmButton.SetActive(true);
		}
		else if (_screenManager.PreviousScreen.Equals(ScreensEnum.MainMenu))
		{
			_menuButton.SetActive(true);

			_quitCancelButton.SetActive(false);
			_gameConfirmButton.SetActive(false);
		}
		else
		{
			throw new Exception($"Unexpected transition from {_screenManager.PreviousScreen} to {ScreenKey} detected. " +
				$"Expected values are: ScreensEnum.Gameplay, ScreensEnum.MainMenu");
		}
	}

	public override void OnExit()
	{
	}

	async public Task DisplayConfirmTextbox(bool value)
	{
		_gameConfirmButtonComponent.SetInteractable(false);
		_quitCancelButtonComponent.SetInteractable(false);

		if (value)
		{
			List<Task> tweenTasks = new()
			{
				_confirmQuitTextboxTween.ExecuteTweenOrders("Appear"),
				_quitCancelButtonComponent.TextMoveUp(),
				_gameConfirmButtonComponent.TextMoveUp()
			};

			foreach (var task in tweenTasks)
			{
				await task;
			}
		}
		else
		{
			await ResetButtons();
		}

		_gameConfirmButtonComponent.SetInteractable(true);
		_quitCancelButtonComponent.SetInteractable(true);
	}

	async public Task ResetButtons()
	{
		List<Task> tweenTasks = new()
		{
			_confirmQuitTextboxTween.ExecuteTweenOrders("Disappear"),
			_quitCancelButtonComponent.TextMoveDown(),
			_gameConfirmButtonComponent.TextMoveDown()
		};

		foreach (var task in tweenTasks)
		{
			await task;
		}
	}
}

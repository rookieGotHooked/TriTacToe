using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen<ScreensEnum>
{
	GameObject _confirmQuitTextbox;
	Tweens2D _confirmQuitTextboxTween;

	GameObject _quitCancelButton;
	QuitCancelButton _quitCancelButtonComponent;

	GameObject _gameConfirmButton;
	GameConfirmButton _gameConfirmButtonComponent;

	GameObject _sfxSliderGameObject;
	Slider _sfxSlider;

	GameObject _bgmSliderGameObject;
	Slider _bgmSlider;

	GameObject _menuButton;

	ScreenManager _screenManager;
	SoundController _soundController;

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
			_soundController = _screenManager.SoundController;

			await InstantiateObjects();
			SetInit();

			_sfxSliderGameObject = _slidersGameObject[0];
			_bgmSliderGameObject = _slidersGameObject[1];
			_confirmQuitTextbox = _staticSpritesGameObject[3];
			_menuButton = _buttonsGameObject[0];
			_quitCancelButton = _buttonsGameObject[1];
			_gameConfirmButton = _buttonsGameObject[2];

			if (!_sfxSliderGameObject.TryGetComponent(out _sfxSlider))
			{
				throw new Exception($"{_sfxSliderGameObject.name} does not contains Slider component");
			}
			_sfxSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });

			if (!_bgmSliderGameObject.TryGetComponent(out _bgmSlider))
			{
				throw new Exception($"{_bgmSliderGameObject.name} does not contains Slider component");
			}
			_bgmSlider.onValueChanged.AddListener(delegate { UpdateBGMVolume(); });

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

			AddAndUpdateAudioSources();
			SetAudioSliderValue();

			DisplayButtons();
		}

		//SetInteractableButtons(true);
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
		//SetInteractableButtons(false);
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

	public void AddAndUpdateAudioSources()
	{
		_soundController.AddAudioSource(AudioType.SFX, GetAllSFXSource());
		_soundController.UpdateAllSourcesVolume(AudioType.SFX);
	}

	public void SetAudioSliderValue()
	{
		_sfxSlider.value = _soundController.SFXVolume;
		_bgmSlider.value = _soundController.BGMVolume;
	}

	public void UpdateSFXVolume()
	{
		_soundController.SetAudioVolume(AudioType.SFX, _sfxSlider.value);
		_soundController.UpdateAllSourcesVolume(AudioType.SFX);
	}

	public void UpdateBGMVolume()
	{
		_soundController.SetAudioVolume(AudioType.BGM, _bgmSlider.value);
		_soundController.UpdateAllSourcesVolume(AudioType.BGM);
	}
	//publ
}

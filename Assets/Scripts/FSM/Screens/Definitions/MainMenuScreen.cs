using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class MainMenuScreen : BaseScreen<ScreensEnum>
{
	ScreenManager _screenManager;

	public MainMenuScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
	{
	}

	public override void OnEnter()
	{
		_screenManager = ScreenManager.Instance;
	}

	async public override void OnUpdate()
	{
		if (!IsInit)
		{
			await InstantiateObjects();
			SetInit();
			AddAndUpdateAudioSources();
		}

		//SetInteractableButtons(true);
	}

	public override void OnExit()
	{
		//SetInteractableButtons(false);
	}

	public void AddAndUpdateAudioSources()
	{
		_screenManager.SoundController.AddAudioSource(AudioType.SFX, GetAllSFXSource());
		_screenManager.SoundController.UpdateAllSourcesVolume(AudioType.SFX);
	}
}

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideScreen : BaseScreen<ScreensEnum>
{
	ScreenManager _screenManager;
	FSM_GuideManager _guideManager;

	public GuideScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
	{
	}

	public override void OnEnter()
	{
		if (IsInit)
		{
			if (_screenManager.PreviousScreen.Equals(ScreensEnum.MainMenu))
			{
				_guideManager.SetScreenTransition(ScreensEnum.MainMenu);
			}
			else if (_screenManager.PreviousScreen.Equals(ScreensEnum.Gameplay))
			{
				_guideManager.SetScreenTransition(ScreensEnum.Gameplay);
			}
			else
			{
				throw new Exception($"Unexpected transition: from {_screenManager.PreviousScreen} to GuideScreen");
			}
		}
	}

	async public override void OnUpdate()
	{
		if (!IsInit)
		{
			_screenManager = ScreenManager.Instance;
			_screenDefinition.ObjectGroupsList[1].Objects[0].Object.gameObject.GetComponent<FSM_GuideManager>().ScreenObject = ScreenObject;

			await InstantiateObjects();
			SetInit();
			AddAndUpdateAudioSources();

			_guideManager = _customGameObject[0].GetComponent<FSM_GuideManager>();

			if (_screenManager.PreviousScreen.Equals(ScreensEnum.MainMenu))
			{
				_guideManager.SetScreenTransition(ScreensEnum.MainMenu);
			}
			else if (_screenManager.PreviousScreen.Equals(ScreensEnum.Gameplay))
			{
				_guideManager.SetScreenTransition(ScreensEnum.Gameplay);
			}
			else
			{
				throw new Exception($"Unexpected transition: from {_screenManager.PreviousScreen} to GuideScreen");
			}
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

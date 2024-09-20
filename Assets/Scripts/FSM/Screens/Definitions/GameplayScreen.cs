using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayScreen : BaseScreen<ScreensEnum>
{
	ScreenManager _screenManager;

	public GameplayScreen(ScreenDefinition<ScreensEnum> screenDefinition) : base(screenDefinition)
	{
		if (screenDefinition != null)
		{
			ScreenKey = screenDefinition.ScreenEnum;
		}
		else
		{
			throw new Exception($"{GetType().Name} assets is null.");
		}
	}

	public override void OnEnter()
	{
		if (GameManager.Instance != null && ScreenManager.Instance.PreviousScreen == ScreensEnum.MainMenu)
		{
			GameManager.Instance.SetGameMode(ScreenManager.Instance.CurrentGameMode);
			GameManager.Instance.GameStateDict[GameStates.Initiate].OnEnter();
		}
	}

	async public override void OnUpdate()
	{
		if (!IsInit)
		{
			_screenManager = ScreenManager.Instance;

			_screenDefinition.ObjectGroupsList[1].Objects[0].Object.gameObject.GetComponent<GameManager>().ScreenObject = _screenObject;

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

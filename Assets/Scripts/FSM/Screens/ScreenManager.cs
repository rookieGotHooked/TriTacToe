using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class ScreenManager: FSM_ScreensManager<ScreensEnum>
{
	#region Base Singleton
	protected static ScreenManager _instance;
	public static ScreenManager Instance { get; private set; }

	private GameMode _currentGameMode;
	public GameMode CurrentGameMode => _currentGameMode;

	#endregion
	protected override void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
			return;
		}

		Instance = this;

		if (!TryGetComponent(out _soundController))
		{
			throw new Exception($"{gameObject.name} does not contains SoundController component.");
		}

		PopuplateAssetDict();

		_screensList = new List<BaseScreen<ScreensEnum>>() 
		{ 
			new MainMenuScreen(_screenDefinitionDict[ScreensEnum.MainMenu]),
			new GameplayScreen(_screenDefinitionDict[ScreensEnum.Gameplay]),
			new GuideScreen(_screenDefinitionDict[ScreensEnum.Guide]),
			new SettingsScreen(_screenDefinitionDict[ScreensEnum.Settings])
		};

		PopulateTransitionDict();
		AddScreens();

		_currentScreen = _screenDict[ScreensEnum.MainMenu];
		_nextScreen = _currentScreen.ScreenKey;
	}

	public void SetGameMode(GameMode mode)
	{
		_currentGameMode = mode;
	}
}

public enum GameMode
{
	Local,
	VersusAI
}

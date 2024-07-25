using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class ScreenManager: FSM_ScreensManager<ScreensEnum>
{
	protected override void Awake()
	{
		PopuplateAssetDict();

		_screensList = new List<BaseScreen<ScreensEnum>>() 
		{ 
			new MainMenuScreen(_screenDefinitionDict[ScreensEnum.MainMenu]),
			new MainMenuSettingsScreen(_screenDefinitionDict[ScreensEnum.MainMenuSettings]),
			new MainMenuGuideScreen(_screenDefinitionDict[ScreensEnum.MainMenuGuide]),

			//new GameplayScreen(_screenDefinitionDict[ScreensEnum.Gameplay]),
			//new GameplaySettingsScreen(_screenDefinitionDict[ScreensEnum.GameplaySettings]),
			//new GameplayGuideScreen(_screenDefinitionDict[ScreensEnum.GameplayGuide]),
		};

		AddScreens();

		_currentScreen = _screenDict[ScreensEnum.MainMenu];
		_nextScreen = _currentScreen.ScreenKey;
	}
}

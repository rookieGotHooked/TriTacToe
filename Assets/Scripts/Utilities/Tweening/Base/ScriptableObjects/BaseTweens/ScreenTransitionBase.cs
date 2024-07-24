using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
public class ScreenTransitionBase<EScreen> : ScriptableObject
    where EScreen: Enum
{
    public ScreenTransitionCode transitionCode;
    public List<ScreenTransitionGroup<EScreen>> transitionGroups;
}

public enum ScreenTransitionCode
{
    MainMenuToMainMenuSettings,
    MainMenuToMainMenuGuide,
    MainMenuToGameplay,

    GameplayToMainMenu,
    GameplayToGameplayGuide,
	GameplayToGameplaySettings,

    MainMenuSettingsToMainMenu,
    MainMenuGuideToMainMenu,

    GameplayGuideToGameplay,
    GameplaySettingsToGameplay,
    GameplaySettingsToMainMenu
}


[Serializable]
public class ScreenTransitionGroup<EScreen> 
    where EScreen: Enum
{
    public string transitionGroupName;
	public List<SingleTransition<EScreen>> transitionList;
}


[Serializable]
public class SingleTransition<EScreen>
	where EScreen : Enum
{
    public string transitionName;
    public EScreen screen;
    public DirectionalMovement directionalMovement;
}

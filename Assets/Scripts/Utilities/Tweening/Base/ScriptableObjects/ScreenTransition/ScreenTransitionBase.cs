using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

[CreateAssetMenu(fileName = "New Screen Transition", menuName = "Scriptable Object/Screen Transition")]
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
	public List<SingleTransition<EScreen>> transitionList;
}


[Serializable]
public class SingleTransition<EScreen>
	where EScreen : Enum
{
    public EScreen screen;
    public DirectionalMovement directionalMovement;
}

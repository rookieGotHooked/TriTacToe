using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Device;
public class ScreenTransitionBase<EScreen> : ScriptableObject
    where EScreen: Enum
{
    public ScreenTransitionCode transitionCode;
    public List<ScreenTransitionGroup<EScreen>> transitionGroups;
    //public float overridenTime;
}

public enum ScreenTransitionCode
{
    MainMenuToGuide,
    MainMenuToSettings,
    MainMenuToGameplay,

    GuideToMainMenu,
    GuideToGameplay,
    
    SettingsToGameplay,
    SettingsToMainMenu,

    GameplayToMainMenu,
    GameplayToSettings,
    GameplayToGuide,

    QuitToMainMenu
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
    [Tooltip("Override the transition time within the transition ScriptableObject;\n" +
        "If the value is larger or equal to 0, transition time in the DirectionalMovement ScriptableObject would be override; " +
        "otherwise, no overriden would occur.")]
    public float overridenTransitionTime;
}

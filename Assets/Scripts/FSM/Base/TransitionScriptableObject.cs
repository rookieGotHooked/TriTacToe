using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionScriptableObject : ScriptableObject
{
    public string movementGroupName;
    public List<ScreenMovement> screenMovements;
    public Tween2DScriptableObject movementData;
}

public enum MovementType
{
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown
}

public class ScreenMovement
{
	public ScreensEnum screen;
	public MovementType movementType;
}

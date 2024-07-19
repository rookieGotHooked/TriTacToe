using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tween 2D", menuName = "Scriptable Object/Tweens2D/Directional Movement")]
public class DirectionalMovement : BaseTween
{
	public MovementDirection direction;
	public float movementValue;
}

public enum MovementDirection
{
	MoveLeft,
	MoveRight,
	MoveUp,
	MoveDown
}

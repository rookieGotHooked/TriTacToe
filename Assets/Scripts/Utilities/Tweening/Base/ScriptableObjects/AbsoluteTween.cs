using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tween 2D", menuName = "Scriptable Object/Tweens2D/Absolute Tween")]
public class AbsoluteTween : BaseTween
{
	public TweenTypes tweenTypes;
	public Vector2 initialValue;
    public Vector2 finalValue;
}

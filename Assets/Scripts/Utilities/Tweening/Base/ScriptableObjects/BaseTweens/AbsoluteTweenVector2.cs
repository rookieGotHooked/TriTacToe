using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "AbsoluteTweenVector2_", menuName = "Scriptable Object/Tweens2D/Absolute Tween Vector 2")]
public class AbsoluteTweenVector2 : BaseTween
{
	public TweenTypes tweenType;
	public Vector2 initialValue;
    public Vector2 finalValue;
}

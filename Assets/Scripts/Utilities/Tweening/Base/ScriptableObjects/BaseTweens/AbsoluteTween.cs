using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "AbsoluteTween_", menuName = "Scriptable Object/Tweens2D/Absolute Tween")]
public class AbsoluteTween : BaseTween
{
	public TweenTypes tweenType;
	public Vector2 initialValue;
    public Vector2 finalValue;
}

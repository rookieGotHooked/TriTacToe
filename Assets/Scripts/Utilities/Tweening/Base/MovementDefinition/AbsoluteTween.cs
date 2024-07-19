using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tween 2D", menuName = "Scriptable Object/Tweens2D/Positional Movement")]
public class AbsoluteTween : BaseTween
{
    public Vector2 initialValue;
    public Vector2 finalValue;
}

using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tween 2D", menuName = "ScriptableObject/Tweens2D/SingleDefinitions")]
public class Tween2DScriptableObject : ScriptableObject
{
    public string tweenName;

    public Vector2 initialValue;
    public Vector2 finalValue;

    public TweenTypes tweenTypes;
    public TweenFormulas tweenFormula;
    public float duration;
}

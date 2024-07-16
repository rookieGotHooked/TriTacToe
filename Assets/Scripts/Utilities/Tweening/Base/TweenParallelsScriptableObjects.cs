using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tween Parallels", menuName = "ScriptableObject/Tweens2D/TweenParallels")]
public class TweenParallels : ScriptableObject
{
    public string tweenGroup;

    [Tooltip("Tweens that run at the same time on the same object")]
    public List<Tween2DScriptableObject> parallelTweens;
}

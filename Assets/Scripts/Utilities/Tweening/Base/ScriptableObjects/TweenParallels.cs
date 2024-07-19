using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tween Parallels", menuName = "Scriptable Object/Tweens2D/Tween Parallels")]
public class TweenParallels : ScriptableObject
{
    public string tweenGroup;

    [Tooltip("Tweens that run at the same time on the same object")]
    public List<BaseTween> parallelTweens;
}

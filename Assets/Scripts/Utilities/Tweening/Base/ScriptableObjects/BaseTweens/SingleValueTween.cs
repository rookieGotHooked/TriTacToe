using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class SingleValueTween<ValueType> : BaseTween
{
    public TweenTypes tweenType;
    public ValueType initialValue;
    public ValueType finalValue;
}

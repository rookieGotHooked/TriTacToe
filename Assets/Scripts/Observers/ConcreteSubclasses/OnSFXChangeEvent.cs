using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnSFXChangeEvent", menuName = "Scriptable Object/Game Events/OnSFXChangeEvent")]
public class OnSFXChangeEvent : GameEvent<FloatWrapper>
{
}

public class FloatWrapper
{
	public FloatWrapper(float value)
	{
		Value = value;
	}

	public float Value;
}

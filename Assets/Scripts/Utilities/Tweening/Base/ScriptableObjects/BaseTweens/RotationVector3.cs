using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RotationVector3_", menuName = "Scriptable Object/Tweens2D/Rotation Vector 3")]
public class RotationVector3: BaseTween
{
	public bool isXClockwise;
	public bool isYClockwise;
	public bool isZClockwise;
	public int xFullRotationTime;
	public int yFullRotationTime;
	public int zFullRotationTime;
	public Vector3 additionalRotationValue;
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Screen Definition", menuName = "Scriptable Object/Screen Definition")]
public class ScreenDefinition<EScreen>: ScriptableObject where EScreen : Enum
{
	[SerializeField] 
	private string _screenName;

	[SerializeField]
	private EScreen _screenEnum;
	public EScreen ScreenEnum { get => _screenEnum; }

	[SerializeField]
	private GameObject _screenObject;
	public GameObject ScreenObject { get => _screenObject; set => _screenObject = value; }

	[SerializeField] private List<ObjectGroup> _objectGroupsList;
	public List<ObjectGroup> ObjectGroupsList { get => _objectGroupsList; }
}

[Serializable]
public class ObjectGroup
{
	[SerializeField] private string _groupName;
	public string GroupName { get => _groupName; }

	[SerializeField] private List<ObjectAsset> _objects;
	public List<ObjectAsset> Objects { get => _objects; }
}

[Serializable]
public class ObjectAsset
{
	[SerializeField] private GameObject _object;
	public GameObject Object { get => _object; }

	[SerializeField] private AssetType _type;
	public AssetType Type { get => _type; }
}

public enum AssetType
{
	Button,
	StaticSprite,
	DynamicSprite,
	Slider,
	Custom
}

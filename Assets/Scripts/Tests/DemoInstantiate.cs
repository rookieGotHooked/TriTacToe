using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInstantiate : MonoBehaviour
{
    [SerializeField] GameObject prefab;

	private void Start()
	{
		Instantiate(prefab, gameObject.transform);
	}
}

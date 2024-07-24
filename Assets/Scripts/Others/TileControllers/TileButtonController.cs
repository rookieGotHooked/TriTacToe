using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileButtonController : MonoBehaviour
{
	// Gameplay FSM Instance

    Button _buttonComponent;

	private void Awake()
	{
		if (!TryGetComponent(out _buttonComponent))
		{
			throw new System.Exception($"GameObject {gameObject.name} does not contains Button component");
		}

		_buttonComponent.onClick.AddListener(RequestMarkSymbol);
	}

	private void RequestMarkSymbol()
	{
		
	}

	async public Task ExecuteMarkSymbol(Symbol symbol)
	{
		//await Task.Yield();
		throw new System.NotImplementedException();
	}
}

public enum Symbol
{
	X, O
}

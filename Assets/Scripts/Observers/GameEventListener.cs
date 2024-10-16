using UnityEngine;
using UnityEngine.Events;

public class GameEventListener<T> : MonoBehaviour where T: class
{
    [SerializeField] private GameEvent<T> _gameEvent;
    [SerializeField] private UnityEvent<T> _response;

	protected void OnEnable()
	{
		_gameEvent.RegisterListener(this);
	}

	protected void OnDisable()
	{
		_gameEvent.UnregisterListener(this);
	}

	public virtual void OnEventRaised(T item)
	{
		_response.Invoke(item);
	}
}

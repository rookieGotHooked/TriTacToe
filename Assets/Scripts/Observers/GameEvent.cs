using System.Collections.Generic;
using UnityEngine;

public class GameEvent<T> : ScriptableObject 
    where T: class
{
    private List<GameEventListener<T>> _listeners = new();

    public void Raise(T item)
    {
        foreach (var listener in _listeners)
        {
            listener.OnEventRaised(item);
        }
    }

    public void RegisterListener(GameEventListener<T> listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener<T> listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }
}

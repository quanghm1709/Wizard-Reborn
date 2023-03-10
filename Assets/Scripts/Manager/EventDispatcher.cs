using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EventID
{
	None = 0,
	OnEnemyDead,
	OnRoomClear,
	OnSkillUpgradeClick,
	OnSkillUpgrade,
	OnSkillUpgradeFailed,
}

public class EventDispatcher : MonoBehaviour
{
	#region Singleton
	static EventDispatcher s_instance;
	public static EventDispatcher Instance
	{
		get
		{
			if (s_instance == null)
			{
				GameObject singletonObject = new GameObject();
				s_instance = singletonObject.AddComponent<EventDispatcher>();
			}
			return s_instance;
		}
		private set { }
	}

	public static bool HasInstance()
	{
		return s_instance != null;
	}

	void Awake()
	{
		if (s_instance != null && s_instance.GetInstanceID() != this.GetInstanceID())
		{
			Destroy(gameObject);
		}
		else
		{
			s_instance = this as EventDispatcher;
		}
	}


	void OnDestroy()
	{
		if (s_instance == this)
		{
			ClearAllListener();
			s_instance = null;
		}
	}
	#endregion


	#region Fields
	/// Store all "listener"
	Dictionary<EventID, Action<object>> _listeners = new Dictionary<EventID, Action<object>>();
	#endregion


	#region Add Listeners, Post events, Remove listener

	/// <summary>
	/// Register to listen for eventID
	/// </summary>
	/// <param name="eventID">EventID that object want to listen</param>
	/// <param name="callback">Callback will be invoked when this eventID be raised</para	m>
	public void RegisterListener(EventID eventID, Action<object> callback)
	{
		if (_listeners.ContainsKey(eventID))
		{
			_listeners[eventID] += callback;
		}
		else
		{
			_listeners.Add(eventID, null);
			_listeners[eventID] += callback;
		}
	}

	/// <summary>
	/// Posts the event. This will notify all listener that register for this event
	/// </summary>
	/// <param name="eventID">EventID.</param>
	/// <param name="sender">Sender, in some case, the Listener will need to know who send this message.</param>
	/// <param name="param">Parameter. Can be anything (struct, class ...), Listener will make a cast to get the data</param>
	public void PostEvent(EventID eventID, object param = null)
	{
		if (!_listeners.ContainsKey(eventID))
		{
			return;
		}

		// posting event
		var callbacks = _listeners[eventID];
		// if there's no listener remain, then do nothing
		if (callbacks != null)
		{
			callbacks(param);
		}
		else
		{
			_listeners.Remove(eventID);
		}
	}

	/// <summary>
	/// Removes the listener. Use to Unregister listener
	/// </summary>
	/// <param name="eventID">EventID.</param>
	/// <param name="callback">Callback.</param>
	public void RemoveListener(EventID eventID, Action<object> callback)
	{

		if (_listeners.ContainsKey(eventID))
		{
			_listeners[eventID] -= callback;
		}
	}

	/// <summary>
	/// Clears all the listener.
	/// </summary>
	public void ClearAllListener()
	{
		_listeners.Clear();
	}
	#endregion
}


#region Extension class
/// <summary>
/// Delare some "shortcut" for using EventDispatcher easier
/// </summary>
public static class EventDispatcherExtension
{
	/// Use for registering with EventsManager
	public static void RegisterListener(this MonoBehaviour listener, EventID eventID, Action<object> callback)
	{
		EventDispatcher.Instance.RegisterListener(eventID, callback);
	}

	/// Post event with param
	public static void PostEvent(this MonoBehaviour listener, EventID eventID, object param)
	{
		EventDispatcher.Instance.PostEvent(eventID, param);
	}

	/// Post event with no param (param = null)
	public static void PostEvent(this MonoBehaviour sender, EventID eventID)
	{
		EventDispatcher.Instance.PostEvent(eventID, null);
	}
}
#endregion

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public enum EventID
//{
//	None = 0,
//	OnBallBack,
//	OnLastBallBack,
//}

//public class EventDispatcher : MonoBehaviour
//{
//	static EventDispatcher s_instance;
//	public static EventDispatcher Instance
//	{
//		get
//		{
//			// instance not exist, then create new one
//			if (s_instance == null)
//			{
//				// create new Gameobject, and add EventDispatcher component
//				GameObject singletonObject = new GameObject();
//				s_instance = singletonObject.AddComponent<EventDispatcher>();
//				singletonObject.name = "Singleton - EventDispatcher";

//			}
//			return s_instance;
//		}
//		private set { }
//	}

//	public static bool HasInstance()
//	{
//		return s_instance != null;
//	}

//	// Register to listen for eventID, callback will be invoke when event with eventID be raise
//	public void RegisterListener(EventID eventID, Action<object> callback) { }

//	// Post event, this will notify all listener which register to listen for eventID
//	public void PostEvent(EventID eventID, Component sender, object param = null) { }

//	// Use for Unregister, not listen for an event anymore.
//	public void RemoveListener(EventID eventID, Action<object> callback) { }
//}

///// An Extension class, declare some "shortcut" for using EventDispatcher
//public static class EventDispatcherExtension
//{
//	/// Use for registering with EventsManager
//	public static void RegisterListener(this MonoBehaviour listener, EventID eventID, Action<object> callback)
//	{
//		EventDispatcher.Instance.RegisterListener(eventID, callback);
//	}

//	/// Post event with param
//	public static void PostEvent(this MonoBehaviour listener, EventID eventID, object param)
//	{
//		EventDispatcher.Instance.PostEvent(eventID, param);
//	}

//	/// Post event with no param (param = null)
//	public static void PostEvent(this MonoBehaviour sender, EventID eventID)
//	{
//		EventDispatcher.Instance.PostEvent(eventID, null);
//	}
//}

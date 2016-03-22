using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class EventDispatcher
{
	//
	// Static Properties
	//
	static EventController mEventController = new EventController();
	public static EventController EventController {
		get {
			return mEventController;
		}
		set {
			mEventController = value;
		}
	}
	
	//
	// Static Methods
	//
	public static void AddEventListener<T0, T1> (string eventType, Action<T0, T1> handler)
	{
		EventController.AddEventListener<T0, T1> (eventType, handler);
	}
	
	public static void AddEventListener<T0, T1, T2> (string eventType, Action<T0, T1, T2> handler)
	{
		EventController.AddEventListener<T0, T1, T2> (eventType, handler);
	}
	
	public static void AddEventListener<T0, T1, T2, T3> (string eventType, Action<T0, T1, T2, T3> handler)
	{
		EventController.AddEventListener<T0, T1, T2, T3> (eventType, handler);
	}
	
	public static void AddEventListener<T> (string eventType, Action<T> handler)
	{
		EventController.AddEventListener<T> (eventType, handler);
	}
	
	public static void AddEventListener (string eventType, Action handler)
	{
		EventController.AddEventListener (eventType, handler);
	}
	
	public static void Cleanup ()
	{
		EventController.Cleanup ();
	}
	
	public static void MarkAsPermanent (string eventType)
	{
		EventController.MarkAsPermanent (eventType);
	}
	
	public static void RemoveEventListener<T0, T1, T2> (string eventType, Action<T0, T1, T2> handler)
	{
		EventController.RemoveEventListener<T0, T1, T2> (eventType, handler);
	}
	
	public static void RemoveEventListener<T0, T1, T2, T3> (string eventType, Action<T0, T1, T2, T3> handler)
	{
		EventController.RemoveEventListener<T0, T1, T2, T3> (eventType, handler);
	}
	
	public static void RemoveEventListener<T0, T1> (string eventType, Action<T0, T1> handler)
	{
		EventController.RemoveEventListener<T0, T1> (eventType, handler);
	}
	
	public static void RemoveEventListener (string eventType, Action handler)
	{
		EventController.RemoveEventListener (eventType, handler);
	}
	
	public static void RemoveEventListener<T> (string eventType, Action<T> handler)
	{
		EventController.RemoveEventListener<T> (eventType, handler);
	}
	
	public static void TriggerEvent<T0, T1, T2> (string eventType, T0 arg1, T1 arg2, T2 arg3)
	{
		EventController.TriggerEvent<T0, T1, T2> (eventType, arg1, arg2, arg3);
	}
	
	public static void TriggerEvent<T0, T1, T2, T3> (string eventType, T0 arg1, T1 arg2, T2 arg3, T3 arg4)
	{
		EventController.TriggerEvent<T0, T1, T2, T3> (eventType, arg1, arg2, arg3, arg4);
	}
	
	public static void TriggerEvent<T0, T1> (string eventType, T0 arg1, T1 arg2)
	{
		EventController.TriggerEvent<T0, T1> (eventType, arg1, arg2);
	}
	
	public static void TriggerEvent (string eventType)
	{
		EventController.TriggerEvent (eventType);
	}
	
	public static void TriggerEvent<T> (string eventType, T arg1)
	{
		EventController.TriggerEvent<T> (eventType, arg1);
	}
}

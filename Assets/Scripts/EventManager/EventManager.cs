using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager {

    private Dictionary<System.Type, UnityEventBase> eventDict;
    private static EventManager eventManager = null; // new EventManager();

    private EventManager() {
        if (eventDict == null) {
            eventDict = new Dictionary<System.Type, UnityEventBase>();
        }
    }

    private static EventManager Get() {
        if (eventManager == null) eventManager = new EventManager();
        return eventManager;
    }

    public static void StartListening<Tbase>(UnityAction listener) where Tbase : UnityEvent, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            ((Tbase) thisEvent).AddListener(listener);
        }
        else {
            Tbase e = new Tbase();
            e.AddListener(listener);
            eventManager.eventDict.Add(typeof(Tbase), e);
        }
    }

    public static void StartListening<Tbase, T0>(UnityAction<T0> listener) where Tbase : UnityEvent<T0>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            ((Tbase)thisEvent).AddListener(listener);
        }
        else {
            Tbase e = new Tbase();
            e.AddListener(listener);
            eventManager.eventDict.Add(typeof(Tbase), e);
        }
    }

    public static void StopListening<Tbase>(UnityAction listener) where Tbase : UnityEvent, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase) thisEvent;

            if (e != null) {
                e.RemoveListener(listener);
            }
        }
    }

    public static void StopListening<Tbase, T0>(UnityAction<T0> listener) where Tbase : UnityEvent<T0>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;

            if (e != null) {
                e.RemoveListener(listener);
            }
        }
    }

    public static void TriggerEvent<Tbase>() where Tbase : UnityEvent, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase) thisEvent;
            if (e != null) {
                e.Invoke();
            }
        }
    }

    public static void TriggerEvent<Tbase, T0>(T0 t0) where Tbase : UnityEvent<T0>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;
            if (e != null) {
                e.Invoke(t0);
            }
        }
    }
}

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

    public static void StartListening<Tbase, T0, T1>(UnityAction<T0, T1> listener) where Tbase : UnityEvent<T0, T1>, new() {
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

    public static void StartListening<Tbase, T0, T1, T2>(UnityAction<T0, T1, T2> listener) where Tbase : UnityEvent<T0, T1, T2>, new() {
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

    public static void StartListening<Tbase, T0, T1, T2, T3>(UnityAction<T0, T1, T2, T3> listener) where Tbase : UnityEvent<T0, T1, T2, T3>, new() {
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

    public static void StopListening<Tbase, T0, T1>(UnityAction<T0, T1> listener) where Tbase : UnityEvent<T0, T1>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;

            if (e != null) {
                e.RemoveListener(listener);
            }
        }
    }

    public static void StopListening<Tbase, T0, T1, T2>(UnityAction<T0, T1, T2> listener) where Tbase : UnityEvent<T0, T1, T2>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;

            if (e != null) {
                e.RemoveListener(listener);
            }
        }
    }

    public static void StopListening<Tbase, T0, T1, T2, T3>(UnityAction<T0, T1, T2, T3> listener) where Tbase : UnityEvent<T0, T1, T2, T3>, new() {
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

    public static void TriggerEvent<Tbase, T0, T1>(T0 t0, T1 t1) where Tbase : UnityEvent<T0, T1>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;
            if (e != null) {
                e.Invoke(t0, t1);
            }
        }
    }

    public static void TriggerEvent<Tbase, T0, T1, T2>(T0 t0, T1 t1, T2 t2) where Tbase : UnityEvent<T0, T1, T2>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;
            if (e != null) {
                e.Invoke(t0, t1, t2);
            }
        }
    }

    public static void TriggerEvent<Tbase, T0, T1, T2, T3>(T0 t0, T1 t1, T2 t2, T3 t3) where Tbase : UnityEvent<T0, T1, T2, T3>, new() {
        UnityEventBase thisEvent = null;
        if (Get().eventDict.TryGetValue(typeof(Tbase), out thisEvent)) {
            Tbase e = (Tbase)thisEvent;
            if (e != null) {
                e.Invoke(t0, t1, t2, t3);
            }
        }
    }
}

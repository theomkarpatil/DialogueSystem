// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;
using UnityEngine.Events;

namespace Sora.Events
{
    /// <summary>
    /// Interface for the Event.
    /// Any Event (with particular requirements, like custom parameters) needs to inherit from this</summary>
    ///  Events that inherit from this need to be Scriptable objects and NOT monobehaviours
    ///  </summary>
    public interface ISoraEvent
    {
        // The method that needs to be used in the calling component's method
        public void InvokeEvent(Component invoker = null, object data = null);
        public void RegisterObserver(ISoraObserver observer);
        public void UnregisterObserver(ISoraObserver observer);
        public void EmptyObserverData();
    }

    // Interface for the Observer for an event
    // Observers that inherit from this need to be monobehaviours and will be attached to objects as components
    public interface ISoraObserver
    {
        public void EventCallback(Component invoker, object data = null);
    }

    // A custom Event derived from UnityEvents that take a Component and an Object.
    // A general practice would be to have the Component set to "this" whenever the Event is being Invoked
    [System.Serializable]
    public class CustomSoraEvent : UnityEvent<Component, object> { }
}
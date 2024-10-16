// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;

namespace Sora.Events
{
    /// <summary>
    /// The SoraObserver is a component and needs to be added to GameObjects/Prefabs in the inspector 
    /// </summary>
    public class SoraObserver : MonoBehaviour, ISoraObserver
    {
        // The SoraEvent created as a scriptable object
        public SoraEvent SoraEvent;
        // This callback method needs to be added in the Editor
        // Whenever 'SoraEvent' is Invoked, this 'callback' method is called
        public CustomSoraEvent callback;

        private void OnEnable()
        {
            SoraEvent.RegisterObserver(this);
        }

        private void OnDisable()
        {
            SoraEvent.UnregisterObserver(this);
        }

        public void EventCallback(Component invoker, object data)
        {
            callback.Invoke(invoker, data);
        }
    }
}

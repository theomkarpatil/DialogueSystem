// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections.Generic;
using UnityEngine;

namespace Sora.Events
{
    /// <summary>
    /// This SoraEvent needs to created in the Editor
    /// and has to be referenced by the Observer components and by the Script that's invoking it
    /// </summary>
    [CreateAssetMenu(fileName = "NewSoraEvent", menuName = "Sora /Event System /Sora Event")]
    public class SoraEvent : ScriptableObject, ISoraEvent
    {
        // Using hashSet to avoid duplicates without having to check every time
        private readonly HashSet<ISoraObserver> observers = new HashSet<ISoraObserver>();

        public void InvokeEvent(Component invoker = null, object data = null)
        {
            foreach (ISoraObserver obs in observers)
            {
                if (obs != null)
                    obs.EventCallback(invoker, data);
            }
        }

        public void RegisterObserver(ISoraObserver observer) => observers.Add(observer);

        public void UnregisterObserver(ISoraObserver observer) => observers.Remove(observer);

        public void EmptyObserverData() => observers.Clear();
    }
}

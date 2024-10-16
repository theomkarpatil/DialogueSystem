using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora.Events;

namespace Sora.DialogueSystem
{
    [CreateAssetMenu(fileName = "NewSoraEvent", menuName = "Sora /Event System /Dialogue Event")]
    public class DialogueEvent : SoraEvent, ISoraEvent
    {
        private readonly HashSet<ISoraObserver> observers = new HashSet<ISoraObserver>();

        void ISoraEvent.InvokeEvent(Component invoker, object data)
        {
            foreach (ISoraObserver obs in observers)
            {
                if (obs != null)
                    obs.EventCallback(invoker, data);
            }
        }
    }
}

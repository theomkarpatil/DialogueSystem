using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora.Variables;
using Sora.Events;

namespace Sora.DialogueSystem
{
    public class DialogueSystem : Managers.Singleton<DialogueSystem>
    {
        public bool conversing;
        public SerializedDictionary<DialogueTriggerWorld, DialogueTriggerWorld> linkedConversations;
    }
}
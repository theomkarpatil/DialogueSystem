using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora.Variables;
using Sora.Events;

namespace Sora.DialogueSystem
{
    public class DialogueSystemWorld : Managers.Singleton<DialogueSystemWorld>
    {
        public bool conversing;
        public SerializedDictionary<DialogueTriggerWorld, DialogueTriggerWorld> linkedConversations;
    }
}
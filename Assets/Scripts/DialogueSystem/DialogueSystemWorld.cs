using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora.Variables;
using Sora.Events;

namespace Sora.DialogueSystem
{
    public class DialogueSystemWorld : Managers.Singleton<DialogueSystemWorld>
    {
        [HideInInspector] public bool conversing;

        [Tooltip("if two Conversations are linked, the \"Value\" will only play once the \"Key\"s primary conversation completes")]
        public SerializedDictionary<DialogueTriggerWorld, DialogueTriggerWorld> linkedConversations;

        public bool CanPlayConversation(DialogueTriggerWorld dTrigger)
        {
            if (conversing)
                return false;

            if (!linkedConversations.ContainsKey(dTrigger))
                return true;

            if (linkedConversations[dTrigger].conversationComplete)
                return true;

            return false;
        }
    }
}
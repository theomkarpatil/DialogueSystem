using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sora.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Character Variables")]
        public string characterName;
        public Sprite characterPortrait;

        // These sentences will be played one after the another when the "Next" button is pressed
        [SerializeField] private List<string> dialogues;
        // Whether the dialogues will be played again when the trigger is visited again
        public bool replayOnRevisit;
        // The cooldown after which these dialogues will be played again when visited
        [ShowIf("replayOnRevisit", true)]
        [SerializeField] private float dialogueReplayCD;
        // The dialogues will only be triggered a second time if the player skipped the conversation
        [ShowIf("replayOnRevisit", true)]
        public bool replayOnlyOnSkip;

        [Space]
        // If set true, these dialogues from the secondary list will be played
        [ShowIf("replayOnRevisit", true)]
        [SerializeField] private bool playSecondaryDialogues;
        // The way in which the secondary dialogues will be played        
        [ShowIf("playSecondaryDialogues", true)]
        [SerializeField] private ESecondaryDialogue secondaryDialogueType;
        // The dialogues that will be played on revisit
        [ShowIf("secondaryDialogueType", ESecondaryDialogue.LINEAR)]
        [SerializeField] private List<string> secondaryDialogues;
        [ShowIf("secondaryDialogueType", ESecondaryDialogue.ONE_BY_ONE)]
        [SerializeField] private List<string> secondaryOBODialogues;
        [ShowIf("secondaryDialogueType", ESecondaryDialogue.RANDOM)]
        [SerializeField] private List<string> secondaryRandomDialogues;

        [Space]
        // Weather an event needs to be fired when the conversation is over
        // Event will be auto created through code
        public bool fireEventOnCompletion;
        [ShowIf("fireEventOnCompletion", true)]
        public DialogueEvent dialogueEndEvent;

        [HideInInspector] public bool conversationComplete;

        private bool visited;
        [HideInInspector] public bool skipped;
        private Coroutine resetDialogueCoroutine;
        [HideInInspector] public int dialogueCompletionIndex;
        [HideInInspector] public int oneByOneIndex;

        private string assetPath;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!replayOnRevisit)
            {
                playSecondaryDialogues = false;
                secondaryDialogueType = ESecondaryDialogue.PLEASE_SELECT_ONE;
            }

            if (fireEventOnCompletion)
            {
                if (dialogueEndEvent != null)
                    return;
                dialogueEndEvent = ScriptableObject.CreateInstance<DialogueEvent>();

                UnityEditor.AssetDatabase.CreateAsset(dialogueEndEvent, "Assets/ScriptableObjects/DialogueEvents/" + gameObject.name + "DialogueEndEvent.asset");
                assetPath = "Assets/ScriptableObjects/DialogueEvents/" + gameObject.name + "DialogueEndEvent.asset";
            }
            else
            {
                if (dialogueEndEvent != null)
                {
                    UnityEditor.AssetDatabase.DeleteAsset(assetPath);
                }
            }
#endif
        }

        private void Awake()
        {
            dialogueCompletionIndex = 0;            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckForDialogues();
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForDialogues();
        }

        private void CheckForDialogues()
        {
            if (DialogueSystem.instance.CanPlayConversation(this) && !visited)
            {
                DialogueSystemWorld.instance.conversing = true;                

                visited = true;
                List<string> _dialogues = new List<string>();

                if (dialogueCompletionIndex > 0 && playSecondaryDialogues)
                {
                    switch (secondaryDialogueType)
                    {
                        case ESecondaryDialogue.LINEAR:
                            {
                                _dialogues = secondaryDialogues;
                                DialogueSystem.instance.ShowDialogue(_dialogues, this);
                            }
                            break;
                        case ESecondaryDialogue.RANDOM:
                            {
                                _dialogues = secondaryRandomDialogues;
                                DialogueSystem.instance.ShowDialoguesOneByOne(_dialogues, this, true);
                            }
                            break;
                        case ESecondaryDialogue.ONE_BY_ONE:
                            {
                                _dialogues = secondaryOBODialogues;
                                DialogueSystem.instance.ShowDialoguesOneByOne(_dialogues, this, false);
                            }
                            break;
                        case ESecondaryDialogue.PLEASE_SELECT_ONE:
                            {
                                _dialogues = secondaryDialogues;
                                DialogueSystem.instance.ShowDialogue(_dialogues, this);
                            }
                            break;
                    }
                }
                else
                {
                    _dialogues = dialogues;
                    DialogueSystem.instance.ShowDialogue(_dialogues, this);
                }
            }
        }

        public void ResetDialogueCooldown()
        {
            resetDialogueCoroutine = StartCoroutine(ResetDialogueCD());
        }

        private IEnumerator ResetDialogueCD()
        {
            yield return new WaitForSecondsRealtime(dialogueReplayCD);

            if (replayOnlyOnSkip && !skipped)
            {
                yield return null;
                StopCoroutine(resetDialogueCoroutine);
            }
            visited = false;
        }
    }

}
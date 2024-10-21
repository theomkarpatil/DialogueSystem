// Developed by Sora
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sora.Events;
using UnityEngine.InputSystem;

namespace Sora.DialogueSystem
{
    public enum ESecondaryDialogue
    {
        SELECT_ONE,
        [Tooltip("plays the secondary dialogues once similar to the primary dialogues")]
        LINEAR,
        [Tooltip("plays secondary dialogues 1 by 1 in order")]
        ONE_BY_ONE,
        [Tooltip("Instead of playing all secondary dialogues at once, this plays one of these dialogues randomly")]
        RANDOM
    }

    public class DialogueTriggerWorld : MonoBehaviour
    {
        // These sentences will be played one after the another when the "Next" button is pressed
        [SerializeField] private List<string> dialogues;
        // The speed at which a dialogue is displayed on the screen
        [SerializeField] private float dialogueTypeSpeed;
        // Whether the dialogues will be played again when the trigger is visited again
        [SerializeField] private bool replayOnRevisit;
        // The cooldown after which these dialogues will be played again when visited
        [ShowIf("replayOnRevisit", true)]
        [SerializeField] private float dialogueReplayCD;
        // The dialogues will only be triggered a second time if the player skipped the conversation
        [ShowIf("replayOnRevisit", true)]
        [SerializeField] private bool replayOnlyOnSkip;

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
        [SerializeField] private bool fireEventOnCompletion;
        [ShowIf("fireEventOnCompletion", true)]
        [SerializeField] private DialogueEvent dialogueEndEvent;

        [Space]
        [SerializeField] private GameObject dialogueCanvas;
        [SerializeField] private TMP_Text dialogueText;

        [HideInInspector] public bool conversationComplete;

        private bool visited;

        private bool next;
        private bool skipped;
        private Coroutine dialogueCoroutine;
        private Coroutine resetDialogueCoroutine;
        private int dialogueCompletionIndex;
        private int oneByOneIndex;
        
        private string assetPath;
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!replayOnRevisit)
            {
                playSecondaryDialogues = false;
                secondaryDialogueType = ESecondaryDialogue.SELECT_ONE;
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
            oneByOneIndex = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!visited)
            {
                visited = true;
                List<string> _dialogues = new List<string>();
                _dialogues = dialogues;
                dialogueCoroutine = StartCoroutine(ShowDialogue(_dialogues));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (DialogueSystemWorld.instance.CanPlayConversation(this) && !visited)
            {
                DialogueSystemWorld.instance.conversing = true;
                
                InputSystem.PlayerInputManager.instance.inputReader.nextPerformedEvent += OnNext;
                InputSystem.PlayerInputManager.instance.inputReader.skipPerformedEvent += OnSkip;

                visited = true;
                List<string> _dialogues = new List<string>();

                if (dialogueCompletionIndex > 0 && playSecondaryDialogues)
                {
                    switch (secondaryDialogueType)
                    {
                        case ESecondaryDialogue.LINEAR:
                            {
                                _dialogues = secondaryDialogues;
                                dialogueCoroutine = StartCoroutine(ShowDialogue(_dialogues));
                            }
                            break;
                        case ESecondaryDialogue.RANDOM:
                            {
                                _dialogues = secondaryRandomDialogues;
                                dialogueCoroutine = StartCoroutine(ShowDialogueOneByOne(_dialogues, true));
                            }
                            break;
                        case ESecondaryDialogue.ONE_BY_ONE:
                            {
                                _dialogues = secondaryOBODialogues;
                                dialogueCoroutine = StartCoroutine(ShowDialogueOneByOne(_dialogues, false));
                            }
                            break;
                        case ESecondaryDialogue.SELECT_ONE:
                            {
                                _dialogues = secondaryDialogues;
                                dialogueCoroutine = StartCoroutine(ShowDialogue(_dialogues));
                            }
                            break;
                    }
                }
                else
                {
                    _dialogues = dialogues;
                    dialogueCoroutine = StartCoroutine(ShowDialogue(_dialogues));
                }
            }
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

        private IEnumerator ShowDialogue(List<string> aDialogues)
        {
            dialogueCanvas.SetActive(true);

            foreach (string dialogue in aDialogues)
            {
                next = false;
                dialogueText.text = "";
                for (int i = 0; i < dialogue.Length; ++i)
                {
                    dialogueText.text += dialogue[i].ToString();

                    yield return new WaitForSecondsRealtime(dialogueTypeSpeed);
                }

                yield return new WaitUntil(() => next);
            }

            conversationComplete = true;
            dialogueCompletionIndex++;
            ExitDialogue(false);
        }

        private IEnumerator ShowDialogueOneByOne(List<string> aDialogues, bool atRandom)
        {
            dialogueCanvas.SetActive(true);
                        
            next = false;
            dialogueText.text = "";


            if (atRandom)
            {
                Random.InitState((int)Time.time);
                int index = Random.Range(0, aDialogues.Count);
                for (int i = 0; i < aDialogues[index].Length; ++i)
                {
                    dialogueText.text += aDialogues[index][i].ToString();

                    yield return new WaitForSecondsRealtime(dialogueTypeSpeed);
                }
                yield return new WaitUntil(() => next);

                dialogueCompletionIndex++;
                ExitDialogue(false);
            }
            else
            {
                for (int i = 0; i < aDialogues[oneByOneIndex].Length; ++i)
                {
                    dialogueText.text += aDialogues[oneByOneIndex][i].ToString();

                    yield return new WaitForSecondsRealtime(dialogueTypeSpeed);
                }

                yield return new WaitUntil(() => next);
                oneByOneIndex++;
            }
            
            if(oneByOneIndex < aDialogues.Count)
            {
                dialogueCompletionIndex++;
                conversationComplete = true;

                ExitDialogue(false);
            }
            else
            {
                conversationComplete = true;
                ExitDialogue(true);
            }
        }


        private void ExitDialogue(bool exitWithoutReset)
        {
            if (!exitWithoutReset && replayOnRevisit && !replayOnlyOnSkip)
                resetDialogueCoroutine = StartCoroutine(ResetDialogueCD());
            
            dialogueCanvas.SetActive(false);
            InputSystem.PlayerInputManager.instance.inputReader.nextPerformedEvent -= OnNext;
            InputSystem.PlayerInputManager.instance.inputReader.skipPerformedEvent -= OnSkip;

            if (fireEventOnCompletion)
                dialogueEndEvent.InvokeEvent();

            DialogueSystemWorld.instance.conversing = false;
        }

        void OnNext()
        {
            next = true;
        }

        void OnSkip()
        {
            if (dialogueCanvas && dialogueCanvas.activeSelf)
            {

                StopCoroutine(dialogueCoroutine);
                dialogueCanvas.SetActive(false);
                if (fireEventOnCompletion)
                    dialogueEndEvent.InvokeEvent();

                if (replayOnlyOnSkip)
                    skipped = true;
                if(replayOnRevisit)
                    resetDialogueCoroutine = StartCoroutine(ResetDialogueCD());
            }
        }
    }
}
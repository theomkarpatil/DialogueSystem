// Developed by Pluto
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
    public class DialogueTriggerWorld : MonoBehaviour
    {
        [SerializeField] private List<string> dialogues;
        [SerializeField] private float dialogueReplayCD;
        
        [SerializeField] private bool fireEventOnCompletion;
        [ShowIf("fireEventOnCompletion", true)]
        private DialogueEvent dialogueEndEvent;

        private bool next;
        private Coroutine dialogueCoroutine;

        [Space]
        [SerializeField] private GameObject dialogueCanvas;
        [SerializeField] private TMP_Text dialogueText;

        public bool visited;

        private void OnEnable()
        {

        }
        
        private void OnValidate()
        {
            if(fireEventOnCompletion)
            {
                dialogueEndEvent = ScriptableObject.CreateInstance<DialogueEvent>();

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(dialogueEndEvent, "Assets/ScriptableObjects/DialogueEvents/" + gameObject.name + "DialogueEndEvent.asset");
#endif
            }
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
            if (!DialogueSystemWorld.instance.conversing && !visited)
            {
                visited = true;
                List<string> _dialogues = new List<string>();
                _dialogues = dialogues;
                dialogueCoroutine = StartCoroutine(ShowDialogue(_dialogues));
            }
        }

        private IEnumerator ResetDialogueCD()
        {
            yield return new WaitForSecondsRealtime(dialogueReplayCD);
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

                    yield return new WaitForSecondsRealtime(0.03f);
                }

                yield return new WaitUntil(() => next);
            }

            dialogueCanvas.SetActive(false);
            if (fireEventOnCompletion)
                dialogueEndEvent.InvokeEvent();
        }

        void OnNext(InputAction.CallbackContext context)
        {
            next = true;
        }

        void OnSkip(InputAction.CallbackContext context)
        {
            if (dialogueCanvas && dialogueCanvas.activeSelf)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueCanvas.SetActive(false);
                if (fireEventOnCompletion)
                    dialogueEndEvent.InvokeEvent();

                StartCoroutine(ResetDialogueCD());
            }
        }
    }
}
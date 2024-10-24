using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sora.DialogueSystem
{
    public class DialogueSystem : Managers.Singleton<DialogueSystem>
    {
        [HideInInspector] public bool conversing;
        [Tooltip("if two Conversations are linked, the \"Value\" will only play once the \"Key\"s primary conversation completes")] public SerializedDictionary<DialogueTrigger, DialogueTrigger> linkedConversations;

        [Header("UI Variables")]
        [SerializeField] private GameObject dialogueUI;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text characterName;
        [SerializeField] private Image characterNameBG;
        [SerializeField] private Image characterPortrait;
        // The speed at which a dialogue is displayed on the screen
        [SerializeField] private float dialogueTypeSpeed;

        private bool next;
        private Coroutine dialogueCoroutine;
        private DialogueTrigger currentDialogueTrigger;

        private string assetPath;

        public bool CanPlayConversation(DialogueTrigger dTrigger)
        {
            if (conversing)
                return false;

            if (!linkedConversations.ContainsKey(dTrigger))
                return true;

            if (linkedConversations[dTrigger].conversationComplete)
                return true;

            return false;
        }

        public void ShowDialogue(List<string> aDialogues, DialogueTrigger dialogueTrigger)
        {
            currentDialogueTrigger = dialogueTrigger;

            // setup UI
            dialogueUI.SetActive(true);
            characterName.text = dialogueTrigger.characterName;
            if(characterName.text.Length > 5)
            {
                characterNameBG.rectTransform.sizeDelta = new Vector2(200.0f + (characterName.text.Length - 5) * 20.0f, characterNameBG.rectTransform.sizeDelta.y);
            }
            characterPortrait.sprite = dialogueTrigger.characterPortrait;

            dialogueCoroutine = StartCoroutine(StartShowingDialogue(aDialogues));
        }

        private IEnumerator StartShowingDialogue(List<string> aDialogues)
        {
            InputSystem.PlayerInputManager.instance.inputReader.nextPerformedEvent += OnNext;
            InputSystem.PlayerInputManager.instance.inputReader.skipPerformedEvent += OnSkip;

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

            currentDialogueTrigger.conversationComplete = true;
            currentDialogueTrigger.dialogueCompletionIndex++;
            ExitDialogue(false);
        }

        public void ShowDialoguesOneByOne(List<string> aDialogues, DialogueTrigger dialogueTrigger, bool atRandom)
        {
            InputSystem.PlayerInputManager.instance.inputReader.nextPerformedEvent += OnNext;
            InputSystem.PlayerInputManager.instance.inputReader.skipPerformedEvent += OnSkip;

            currentDialogueTrigger = dialogueTrigger;

            // setup UI
            dialogueUI.SetActive(true);
            characterName.text = dialogueTrigger.characterName;
            if (characterName.text.Length > 5)
            {
                characterNameBG.rectTransform.sizeDelta = new Vector2(200.0f + (characterName.text.Length - 5) * 20.0f, characterNameBG.rectTransform.sizeDelta.y);
            }
            characterPortrait.sprite = dialogueTrigger.characterPortrait;

            dialogueCoroutine = StartCoroutine(StartShowingDialogueOneByOne(aDialogues, atRandom));
        }

        private IEnumerator StartShowingDialogueOneByOne(List<string> aDialogues, bool atRandom)
        {
            dialogueUI.SetActive(true);

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

                currentDialogueTrigger.dialogueCompletionIndex++;
                ExitDialogue(false);
            }
            else
            {
                for (int i = 0; i < aDialogues[currentDialogueTrigger.oneByOneIndex].Length; ++i)
                {
                    dialogueText.text += aDialogues[currentDialogueTrigger.oneByOneIndex][i].ToString();

                    yield return new WaitForSecondsRealtime(dialogueTypeSpeed);
                }

                yield return new WaitUntil(() => next);
                currentDialogueTrigger.oneByOneIndex++;
            }

            if (currentDialogueTrigger.oneByOneIndex < aDialogues.Count)
            {
                currentDialogueTrigger.dialogueCompletionIndex++;
                currentDialogueTrigger.conversationComplete = true;

                ExitDialogue(false);
            }
            else
            {
                currentDialogueTrigger.conversationComplete = true;
                ExitDialogue(true);
            }
        }

        void OnNext()
        {
            next = true;
        }

        void OnSkip()
        {
            if (dialogueUI && dialogueUI.activeSelf)
            {
                StopCoroutine(dialogueCoroutine);
                dialogueUI.SetActive(false);
                if (currentDialogueTrigger.fireEventOnCompletion)
                    currentDialogueTrigger.dialogueEndEvent.InvokeEvent();

                if (currentDialogueTrigger.replayOnlyOnSkip)
                    currentDialogueTrigger.skipped = true;
                if (currentDialogueTrigger.replayOnRevisit)
                    currentDialogueTrigger.ResetDialogueCooldown();

                conversing = false;
            }
        }

        private void ExitDialogue(bool exitWithoutReset)
        {
            if (!exitWithoutReset && currentDialogueTrigger.replayOnRevisit && !currentDialogueTrigger.replayOnlyOnSkip)
                currentDialogueTrigger.ResetDialogueCooldown();

            dialogueUI.SetActive(false);
            InputSystem.PlayerInputManager.instance.inputReader.nextPerformedEvent -= OnNext;
            InputSystem.PlayerInputManager.instance.inputReader.skipPerformedEvent -= OnSkip;

            if (currentDialogueTrigger.fireEventOnCompletion)
                currentDialogueTrigger.dialogueEndEvent.InvokeEvent();

            DialogueSystemWorld.instance.conversing = false;
        }
    }
}
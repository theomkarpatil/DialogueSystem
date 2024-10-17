using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Sora.InputSystem
{
    [CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Sora/Input System/PlayerInputReader")]

    public class PlayerInputReader : ScriptableObject, PlayerKeyBindings.IPlayerInteractionsActions, PlayerKeyBindings.IPlayerMovementActions
    {
        // interaction events
        public event UnityAction nextPerformedEvent;
        public event UnityAction skipPerformedEvent;

        // movement events
        public event UnityAction<Vector2> moveEvent;
        public event UnityAction<Vector2> moveCanceledEvent;

        public void Enable()
        {
            PlayerInputManager.instance.pkb.PlayerInteractions.Enable();
            PlayerInputManager.instance.pkb.PlayerInteractions.SetCallbacks(this);


            PlayerInputManager.instance.pkb.PlayerMovement.Enable();
            PlayerInputManager.instance.pkb.PlayerMovement.SetCallbacks(this);
        }

        public void OnNextDialogue(InputAction.CallbackContext context)
        {
            if (nextPerformedEvent != null && context.phase == InputActionPhase.Performed)
                nextPerformedEvent.Invoke();
        }

        public void OnSkipDialogue(InputAction.CallbackContext context)
        {
            if (skipPerformedEvent != null && context.phase == InputActionPhase.Performed)
                skipPerformedEvent.Invoke();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            if (moveEvent != null && context.phase == InputActionPhase.Performed)
                moveEvent.Invoke(context.ReadValue<Vector2>());

            if (moveCanceledEvent != null && context.phase == InputActionPhase.Canceled)
                moveCanceledEvent.Invoke(context.ReadValue<Vector2>());

        }
    }

}
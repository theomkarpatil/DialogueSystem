// Developed by Sora
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sora.InputSystem;
using Sora.Events;

namespace Sora.Player
{ 
    /// You may delete all of the stuff inside here. 
    /// Just remember to stick to the formating

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        private Vector3 moveDirection;

        private void OnEnable()
        {
            PlayerInputManager.instance.EnablePlayerInput();
            PlayerInputManager.instance.inputReader.moveEvent += OnMovement;
            PlayerInputManager.instance.inputReader.moveCanceledEvent += OnMovement;
        }

        private void OnMovement(Vector2 moveDir)
        {
            moveDirection = new Vector3(moveDir.x, 0.0f, moveDir.y);
        }

        private void FixedUpdate()
        {
            transform.position += movementSpeed * Time.deltaTime * moveDirection;
        }
    }
}
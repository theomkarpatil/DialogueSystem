// Developed by Sora
//
// Copyright(c) Sora 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;

namespace Sora.Variables
{
    /// <summary>
    /// A scriptable object base to create a string value that exists in the project
    /// </summary>
    [CreateAssetMenu(fileName = "SO_NewStringVariable", menuName = "Sora/Variables/String Variable")]
    public class StringVariable : ScriptableObject
    {
        public string value;
        [TextArea(2, 5)]
        [Tooltip("A short description of this variable if not obvious.")]
        [SerializeField] private string description;
    }
}
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
    /// A scriptable object base to create a boolean that exists in the project
    /// </summary>
    [CreateAssetMenu(fileName = "SO_NewBoolVariable", menuName = "Sora/Variables/Bool Variable")]
    public class BoolVariable : ScriptableObject
    {
        public bool value;
        [TextArea(2, 5)]
        [Tooltip("A short description of this variable if not obvious.")]
        [SerializeField] private string description;

        [Space]
        [Space]
        // If resetToDefault is set to true, value = defaultValue when the project stops running
        [SerializeField] private bool resetToDefault;
        [SerializeField] private bool defaultValue;

        private void OnEnable()
        {
            if (resetToDefault)
                value = defaultValue;
        }

        public void SetValue(bool value)
        {
            this.value = value;
        }
    }

}
// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;

namespace Sora.Utility
{
    /// <summary>
    /// A Variable of type `T` that invokes an events whenever the value of the variable changes
    /// In order to change the variable, be sure to use VolatileVariable.value as the "variable to be changed"
    /// Methods that need to be called when the value changes have to be Subscribed to VolatileVariable.OnVariableChange.
    /// --e.g.
    /// class foo : Monobehavior
    /// {
    ///     VolatileVariable<float> test;
    ///     void CallThisEvent()
    ///     { // do something }
    /// 
    ///         void Start()
    ///         {
    ///             test.OnVariableChange += CallThisEvent;
    ///         }
    ///         void OnTriggerEnter()
    ///         {
    ///             test.value += 50.0f; // CallThisEvent will be called after this.
    ///         }
    ///     }    
    /// --end e.g.
    /// 
    /// if T is Custom Struct or Class, value first needs to be assigned manually and shouldn't be null
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VolatileVariable<T>
    {
        private T _value;
        public T value
        {
            get
            {
                if (_value != null)
                    return _value;
                else
                {
                    Debug.Log("VolatileVariable is a custom class and its 'Value' needs to be assigned first. Class will just return null for now");
                    return _value;
                }
            }
            set
            {
                if (this._value == (dynamic)value)
                    return;

                this._value = value;

                if (OnVariableChange != null)
                    OnVariableChange(this._value);
            }
        }

        public VolatileVariable() { }

        public VolatileVariable(T t)
        {
            _value = t;
        }

        public static bool operator ==(VolatileVariable<T> lhs, VolatileVariable<T> rhs)
        {
            if (Equals(lhs, rhs))
                return true;

            return false;
        }

        public static bool operator !=(VolatileVariable<T> lhs, VolatileVariable<T> rhs)
        {
            if (!Equals(lhs, rhs))
                return true;

            return false;
        }

        public static bool Equals(T lhs, T rhs)
        {
            return (dynamic)lhs == (dynamic)rhs;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public delegate void OnVariableChangedDelegate(T t);
        public event OnVariableChangedDelegate OnVariableChange;
    }
}
// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System;

namespace Sora.Utility
{
    public struct DelayedMethod
    {
        public UnityEngine.Component invoker;
        public Action method;
        public float delay;
        public double startTime;
        public DateTime realStartTime;
        public bool executed;

        public DelayedMethod(UnityEngine.Component _invoker, Action _method, float _delay)
        {
            invoker = _invoker;
            method = _method;
            delay = _delay;
            startTime = SoraClock.instance.currentTime;
            realStartTime = DateTime.Now;
            executed = false;
        }

        public static bool operator ==(DelayedMethod lhs, DelayedMethod rhs)
        {
            if (lhs.delay == rhs.delay && lhs.method == rhs.method && lhs.realStartTime == rhs.realStartTime)
                return true;

            return false;
        }

        public static bool operator !=(DelayedMethod lhs, DelayedMethod rhs)
        {
            if (lhs.delay != rhs.delay || lhs.method != rhs.method || lhs.realStartTime != rhs.realStartTime)
                return true;

            return false;
        }

        public void SetExecuted()
        {
            executed = true;
        }

        #region required stuff to eliminate warnings
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion
    }
}

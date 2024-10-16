// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Sora.Utility
{
    public class SoraClock : Managers.Singleton<SoraClock>
    {
        /// <summary>
        /// time since play button is pressed
        /// updates once per frame
        /// </summary>
        public double currentTime { private set; get; }

        /// <summary>
        /// The list containing all the methods that wil be executed after delay
        /// </summary>
        private List<DelayedMethod> methodList;

        private void OnEnable()
        {
            currentTime = 0;

            methodList = new List<DelayedMethod>();

            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            currentTime += Time.unscaledDeltaTime;

            CheckDelayedMethods();
        }

        /// <summary>
        /// Loops through all methods in method list that need to execute after delay
        /// If the delay duration is reached, the method is invoked
        /// </summary>
        private void CheckDelayedMethods()
        {
            if (methodList.Count > 0)
                for (int i = 0; i < methodList.Count; ++i)
                {
                    if (currentTime - methodList[i].startTime > methodList[i].delay)
                    {
                        methodList[i].method.Invoke();
                        methodList[i].SetExecuted();
                        methodList.RemoveAt(i);
                    }
                }
        }

        /// <summary>
        /// The Sora Method used to call a method with a delay of "duration" seconds
        /// e.g.:
        /// SoraClock.Instance.ExecuteWithDelay(this, () => ExampleMethod(arg1, arg2,...), 4.0f);
        /// Make sure to call the Action using "() =>"
        /// </summary>
        /// <param name="invoker"> should usually default to "this" </param>
        /// <param name="method"> the method to be called with a delay </param>
        /// <param name="duration"> the delay in seconds </param>
        /// <returns> Returns the method along with all it's details in the DelayedMethod form </returns>
        public DelayedMethod ExecuteWithDelay(Component invoker, Action method, float duration)
        {
            DelayedMethod dm = new DelayedMethod(invoker, method, duration);
            methodList.Add(dm);

            return dm;
        }

        /// <summary>
        /// Removes the DelayedMethod "method" from the list, and will not be considered for execution
        /// Create a DelayedMethod dm = ExecuteWithDelay(...) reference to use in this method later
        /// </summary>
        /// <param name="method"></param>
        public void StopDelayedMethod(DelayedMethod method)
        {
            if (methodList.Contains(method))
            {
                methodList.Remove(method);
            }
        }

        /// <summary>
        /// Stops all the delayed method that have been called by that particular instance of the method
        /// </summary>
        /// <param name="invoker"></param>
        public void StopAllDelayedMethodsInObject(Component invoker)
        {
            var list = methodList;

            if (list.Count > 0)
                for (int i = 0; i < list.Count; ++i)
                {
                    if (methodList[i].invoker == invoker)
                        methodList.Remove(methodList[i]);
                }
        }

        /// <summary>
        /// Stops all the delayed methods currently in queue to be executed later
        /// </summary>
        public void StopAllDelayedMethods()
        {
            methodList.Clear();
        }
    }
}

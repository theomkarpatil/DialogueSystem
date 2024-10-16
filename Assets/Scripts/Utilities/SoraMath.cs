// Developed by Sora Arts
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using UnityEngine;

namespace Sora.Utility 
{ 
    public static class SoraMath
    {
        /// <summary>
        /// returns a value rescaled from initial range to a value between 0 and 1.
        /// </summary>
        /// <param name="min"> minimum value of initial range</param>
        /// <param name="max"> maximum value of initial range</param>
        /// <param name="value"> value to be rescaled between 0 and 1 </param>
        /// <returns> returns the value rescaled between 0 and 1</returns>
        public static float Rescale(float min, float max, float value)
        {
            if (value == min)
                return 0;
            if (value == max)
                return 1;

            return (value - min) / (max - min);
        }

        /// <summary>
        /// returns a value rescaled from initial range to targetRange
        /// </summary>
        /// <param name="min"> minimum value of initial range </param>
        /// <param name="max"> maximum value of initial range </param>
        /// <param name="value"> value to be rescaled between targetMin and targetMax </param>
        /// <param name="targetMin"> minimum value of rescaled range </param>
        /// <param name="targetMax"> maximum value of rescaled range </param>
        /// <returns>returns a value rescaled from initial range to targetRange</returns>
        public static float RescaleRange(float min, float max, float value, float targetMin, float targetMax)
        {
            // the ratio between the two ranges
            float _ratio = (targetMax - targetMin) / (max - min);
            // the reach of the new value in terms of the ratio
            float _reach = _ratio * Mathf.Abs(value - min);

            // how far from target min is the reach
            return targetMin + _reach;
        }

        /// <summary>
        /// snaps the given value to the nearest snapTo value's multiple
        /// </summary>
        /// <param name="value"> value to be snapped </param>
        /// <param name="snapTo"> value to snap to. works with decimal values as well </param>
        public static void Snap(ref float value, float snapTo)
        {
            float _rem = value % snapTo;

            if (_rem > snapTo / 2)
                value += (snapTo - _rem);
            else
                value -= _rem;
        }

        /// <summary>
        /// snaps the given vector value to the nearest snapTo value's multiple
        /// </summary>
        /// <param name="vec"> Vector to be snapped </param>
        /// <param name="snap"> Values each vector component needs to be snapped to </param>
        public static void SnapVector(ref UnityEngine.Vector3 vec, float snap)
        {
            Snap(ref vec.x, snap);
            Snap(ref vec.y, snap);
            Snap(ref vec.z, snap);
        }

        /// <summary>
        ///  snaps the given vector value to the nearest snapTo vector value's multiple
        /// </summary>
        /// <param name="vec"> Vector to be snapped </param>
        /// <param name="snap"> Vector values each vector component needs to be snapped to </param>
        public static void SnapVectorByVector(ref UnityEngine.Vector3 vec, UnityEngine.Vector3 snap)
        {
            Snap(ref vec.x, snap.x);
            Snap(ref vec.y, snap.y);
            Snap(ref vec.z, snap.z);
        }

        /// <summary>
        /// Used to check if a value is withing a miniscule range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="epsilon"></param>
        /// <returns> true if value is wihin range false if outside range</returns>
        public static bool WithinEpsilon(float value, float epsilon)
        {
            if (value <= epsilon && value >= -epsilon)
                return true;
            return false;
        }
    }
}
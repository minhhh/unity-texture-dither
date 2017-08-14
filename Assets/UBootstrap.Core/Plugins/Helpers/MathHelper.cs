using UnityEngine;
using System.Collections;

namespace UBootstrap
{
    static public class MathHelper
    {
        /// <summary>
        /// Round to nearest multiples of steps. For instance, 3 rounded to nearest 5 results in 5,
        /// 2 rounded to nearest 5 results in 0
        /// </summary>
        /// <returns>The to nearest.</returns>
        /// <param name="value">Value.</param>
        /// <param name="step">Round to nearest.</param>
        public static float RoundToNearest (float value, float step)
        {
            return Mathf.Round (value / step) * step;
        }
    }

}
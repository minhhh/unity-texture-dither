using UnityEngine;
using System.Collections;

using System;
using System.Collections;

namespace UBootstrap
{
    static public class VectorHelper
    {
        public static Vector3 WithX (this Vector3 vec, float x)
        {
            return new Vector3(x, vec.y, vec.z);
        }

        public static Vector2 WithX (this Vector2 vec, float x)
        {
            return new Vector2(x, vec.y);
        }

        public static Vector3 WithY (this Vector3 vec, float y)
        {
            return new Vector3(vec.x, y, vec.z);
        }

        public static Vector2 WithY (this Vector2 vec, float y)
        {
            return new Vector2(vec.x, y);
        }

        public static Vector3 WithZ (this Vector3 vec, float z)
        {
            return new Vector3(vec.x, vec.y, z);
        }
    }
}

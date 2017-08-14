using System;

namespace UBootstrap
{
    static public class EnumHelper
    {
        public static int EnumToInt (Enum e)
        {
            return (int)((object)e);
        }

        public static bool EnumEqual (Enum a, Enum b)
        {
            return EnumToInt (a) == EnumToInt (b);
        }

    }
}

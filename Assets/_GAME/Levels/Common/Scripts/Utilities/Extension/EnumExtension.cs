using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class EnumExtension
    {
        public static bool IsOneOf<T>(this T self, params T[] values) where T : System.Enum
        {
            foreach (var value in values)
            {
                if (self.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsNotOf<T>(this T self, params T[] values) where T : System.Enum
        {
            foreach (var value in values)
            {
                if (self.Equals(value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
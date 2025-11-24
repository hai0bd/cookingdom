using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class StringExtension
    {
        private static string[] RomanLetters = {
            "M",
            "CM",
            "D",
            "CD",
            "C",
            "XC",
            "L",
            "XL",
            "X",
            "IX",
            "V",
            "IV",
            "I"
        };

        private static int[] RomanNumbers = {
            1000,
            900,
            500,
            400,
            100,
            90,
            50,
            40,
            10,
            9,
            5,
            4,
            1
        };

        public static string IntToRoman(int num)
        {
            if (num == 0) return "0";

            int sign = num < 0 ? -1 : 1;
            if (num < 0) num = -num;

            string romanResult = string.Empty;
            int i = 0;
            while (num != 0)
            {
                if (num >= RomanNumbers[i])
                {
                    num -= RomanNumbers[i];
                    romanResult += RomanLetters[i];
                }
                else
                {
                    i++;
                }
            }
            return romanResult;
        }
    }
}
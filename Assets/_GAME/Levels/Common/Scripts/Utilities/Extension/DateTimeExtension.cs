using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class DateTimeExtension
    {
        public static bool IsSameDay(System.DateTime date1, System.DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
        }
    }
}
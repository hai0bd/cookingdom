using System;
using System.Collections;
using UnityEngine;

namespace Utilities.DateTime
{
    [System.Serializable]
    public struct DateStruct : IComparable<DateStruct>
    {
        public int day;
        public int month;
        public int year;

        public System.DateTime ToDateTime()
        {
            return new System.DateTime(year, month, day);
        }

        /// <summary>
        /// Time ratio is from 0 to 1. Zero mean start of the day. One mean end of the day.
        /// </summary>
        public System.DateTime ToDateTime(float timeRatio)
        {
            return new System.DateTime(year, month, day, (int)(23 * timeRatio), (int)(59 * timeRatio), (int)(59 * timeRatio));
        }

        public override string ToString()
        {
            return $"{year}/{month}/{day}";
        }

        public int CompareTo(DateStruct other)
        {
            if (year != other.year)
            {
                return year.CompareTo(other.year);
            }
            if (month != other.month)
            {
                return month.CompareTo(other.month);
            }
            return day.CompareTo(other.day);
        }

        public static bool operator >(DateStruct operand1, DateStruct operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }
        public static bool operator <(DateStruct operand1, DateStruct operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }
        public static bool operator >=(DateStruct operand1, DateStruct operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }
        public static bool operator <=(DateStruct operand1, DateStruct operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
        public static bool operator ==(DateStruct operand1, DateStruct operand2)
        {
            return operand1.CompareTo(operand2) == 0;
        }
        public static bool operator !=(DateStruct operand1, DateStruct operand2)
        {
            return operand1.CompareTo(operand2) != 0;
        }
    }

    [System.Serializable]
    public struct TimeStruct
    {
        public int hour;
        public int minute;
        public int second;
        public override string ToString()
        {
            return $"{hour}:{minute}:{second}";
        }
    }

    [System.Serializable]
    public struct DateTimeStruct
    {
        public DateStruct date;
        public TimeStruct time;

        public DateTimeStruct(DateStruct date, TimeStruct time)
        {
            this.date = date;
            this.time = time;
        }

        public DateTimeStruct(int day, int month, int year, int hour, int minute, int second)
        {
            date = new DateStruct()
            {
                day = day,
                month = month,
                year = year
            };
            time = new TimeStruct()
            {
                hour = hour,
                minute = minute,
                second = second
            };
        }

        public System.DateTime ToDateTime()
        {
            return new System.DateTime(date.year, date.month, date.day, time.hour, time.minute, time.second);
        }

        public override string ToString()
        {
            return $"{date.year}/{date.month}/{date.day} {time.hour}:{time.minute}:{time.second}";
        }
    }

    [System.Serializable]
    public struct DateSpan
    {
        public DateStruct fromDate;
        public DateStruct toDate;

        public DateSpan(DateStruct fromDate, DateStruct toDate)
        {
            this.fromDate = fromDate;
            this.toDate = toDate;
        }

        public DateSpan(int fromDay, int fromMonth, int fromYear, int toDay, int toMonth, int toYear)
        {
            fromDate = new DateStruct()
            {
                day = fromDay,
                month = fromMonth,
                year = fromYear
            };
            toDate = new DateStruct()
            {
                day = toDay,
                month = toMonth,
                year = toYear
            };
        }

        public override string ToString()
        {
            return $"{fromDate.ToString()} - {toDate.ToString()}";
        }

        public System.TimeSpan CalculateTimeSpan() => toDate.ToDateTime(1) - fromDate.ToDateTime(0);

        public bool CheckIsInSpan(System.DateTime dateTime)
        {
            return fromDate.ToDateTime(0) <= dateTime && dateTime <= toDate.ToDateTime(1);
        }
        public bool CheckIsPassed(System.DateTime dateTime)
        {
            return dateTime > toDate.ToDateTime(1);
        }

        public System.TimeSpan GetTimeLeft(System.DateTime dateTime)
        {
            return toDate.ToDateTime(1) - dateTime;
        }
    }

    [System.Serializable]
    public struct DateTimeSpan
    {
        public DateTimeStruct fromDateTime;
        public DateTimeStruct toDateTime;

        public DateTimeSpan(DateTimeStruct fromDateTime, DateTimeStruct toDateTime)
        {
            this.fromDateTime = fromDateTime;
            this.toDateTime = toDateTime;
        }

        public DateTimeSpan(DateStruct fromDate, TimeStruct fromTime, DateStruct toDate, TimeStruct toTime)
        {
            fromDateTime = new DateTimeStruct(fromDate, fromTime);
            toDateTime = new DateTimeStruct(toDate, toTime);
        }

        public DateTimeSpan(int fromDay, int fromMonth, int fromYear, int fromHour, int fromMinute, int fromSecond, int toDay, int toMonth, int toYear, int toHour, int toMinute, int toSecond)
        {
            fromDateTime = new DateTimeStruct(fromDay, fromMonth, fromYear, fromHour, fromMinute, fromSecond);
            toDateTime = new DateTimeStruct(toDay, toMonth, toYear, toHour, toMinute, toSecond);
        }

        public bool CheckIsInSpan(System.DateTime dateTime)
        {
            return dateTime >= fromDateTime.ToDateTime() && dateTime <= toDateTime.ToDateTime();
        }

        public override string ToString()
        {
            return $"{fromDateTime.ToString()} - {toDateTime.ToString()}";
        }
    }
}
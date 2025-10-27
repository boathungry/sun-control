using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace SunControl
{
    [Serializable]
    public class StartingDate
    {
        public int day = 11;
        public int month = 10;
        public int year = 1996;
    }
    [Serializable]
    public class StartingTime
    {
        public int hour = 12;
        public int minute = 30;
    }
    [Serializable]
    public class SunRotationOffset
    {
        public GameObject compass;
        public Direction sunriseDirection = Direction.PositiveX;
        [Range(0, 360)]
        public float rotationOffset = 0;
    }

    public enum Direction
    {
        PositiveX,
        PositiveZ,
        NegativeX,
        NegativeZ
    }

    public class SunControl : MonoBehaviour
    {
        public InputActionReference moveSunAction;
        public float latitude;
        public float longitude;
        public StartingDate date;
        public StartingTime time;
        public GameObject sun;
        public float sunSpeed = 100;
        public SunRotationOffset offset;

        private DateTime dateTime;
        private SunCalculations sunCalc;
        private float sunMoveValue;

        /// <summary>
        /// Sets the new date and time, then updates the sun's position based on it.
        /// </summary>
        /// <param name="newDateTime"></param>
        void SetDateTime(DateTime newDateTime)
        {
            dateTime = newDateTime;
            SetSunPosition(dateTime);
        }

        public DateTime GetDateTime()
        {
            return dateTime;
        }

        public void SetDateFromString(string dateString)
        {
            char[] separators = new char[] { ' ', '.', '/', '-' };
            string[] separatedDate = dateString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int day, month, year;
            day = int.Parse(separatedDate[0]);
            month = int.Parse(separatedDate[1]);
            year = int.Parse(separatedDate[2]);
            SetDateTime(new DateTime(year, month, day, time.hour, time.minute, 0));
        }

        private float GetRotationOffset()
        {
            if (offset.compass)
            {
                return offset.compass.transform.rotation.eulerAngles.y;
            }
            else if (offset.sunriseDirection != Direction.PositiveX)
            {
                float rot;
                switch (offset.sunriseDirection)
                {
                    case Direction.PositiveZ:
                        rot = -90;
                        break;
                    case Direction.NegativeZ:
                        rot = 90;
                        break;
                    case Direction.NegativeX:
                        rot = 180;
                        break;
                    default:
                        rot = 0;
                        break;
                }
                return rot + offset.rotationOffset;
            }
            else
            {
                return offset.rotationOffset;
            }
        }

        /// <summary>
        /// Linearly interpolates the time of day between 00:00 and 23:59 based on the fraction provided.
        /// Updates sun position.
        /// Accepts values from 0 to 1.
        /// </summary>
        /// <param name="frac">A value between 0 and 1 (inclusive).</param>
        public void SetDayFraction(float frac)
        {
            float startMinutes = 0;
            float endMinutes = 60 * 23 + 59;
            float currMinutesTotal = (long)Mathf.Lerp(startMinutes, endMinutes, frac);
            float currMinutes = currMinutesTotal % 60;
            int currHours = Mathf.RoundToInt((currMinutesTotal - currMinutes) / 60);
            SetDateTime(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, currHours, Mathf.RoundToInt(currMinutes), 0));
        }

        /// <summary>
        /// Update the sun's position based on the date and time.
        /// </summary>
        /// <param name="dateTime"></param>
        void SetSunPosition(DateTime dateTime)
        {
            Vector2 sunPos = sunCalc.GetSunPosition(dateTime, latitude, longitude);
            sun.transform.eulerAngles = new Vector3(sunPos.y, sunPos.x + GetRotationOffset());
            // In Unity, the azimuth translates to rotating around the Y axis
            // and the altitude translates to rotating around the X axis
        }

        void Start()
        {
            dateTime = new DateTime(date.year, date.month, date.day, time.hour, time.minute, 0);
            sunCalc = gameObject.AddComponent<SunCalculations>();
            SetSunPosition(dateTime);

            moveSunAction.action.Enable();
            moveSunAction.action.performed += context => sunMoveValue = context.ReadValue<float>();
        }

        void Update()
        {
            if (sunMoveValue != 0)
            {
                SetDateTime(dateTime.AddMinutes(sunMoveValue * Time.deltaTime * sunSpeed));
            }
        }
    }
}

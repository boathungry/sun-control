using UnityEngine;
using System;

/* 
 * NOTE: This script is essentially just a part of the SunCalc javascript library adapted into C#, which is based on formulas from Astronomy Answers.
 * Links to both sources included below.
 * 
 * Astronomy Answers: http://aa.quae.nl/en/reken/zonpositie.html
 * SunCalc: https://github.com/mourner/suncalc
 * 
 * Copyright notice and license for SunCalc:
 * Copyright (c) 2014, Vladimir Agafonkin
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
 * TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace SunControl
{
    public class SunCalculations : MonoBehaviour
    {
        // Constants
        readonly double DAYMS = 1000 * 60 * 60 * 24;    // Number of milliseconds in a day
        readonly double J1970 = 2440588;                // 12:00 UTC Jan 1st, 1970 as a Julian Date
        readonly double J2000 = 2451545;                // 12:00 UTC Jan 1st, 2000 as a Julian Date
        readonly float RAD = Mathf.Deg2Rad;             // Radian conversion constant
        readonly float E = Mathf.Deg2Rad * 23.4397f;    // Obliquity of the Earth

        // Conversion functions
        double ToJulian(DateTime date) { return (date.ToUniversalTime() - DateTime.UnixEpoch).TotalMilliseconds / DAYMS - 0.5 + J1970; }
        DateTime FromJulian(float j) { return new DateTime(Convert.ToInt32((j + 0.5 - J1970) * DAYMS)); }
        double ToDays(DateTime date) { return ToJulian(date) - J2000; }

        // Position calculations
        float RightAscension(float l, float b) { return Mathf.Atan2(Mathf.Sin(l) * Mathf.Cos(E) - Mathf.Tan(b) * Mathf.Sin(E), Mathf.Cos(l)); }
        float Declination(float l, float b) { return Mathf.Asin(Mathf.Sin(b) * Mathf.Cos(E) + Mathf.Cos(b) * Mathf.Sin(E) * Mathf.Sin(l)); }
        float Altitude(float H, float phi, float dec) { return Mathf.Asin(Mathf.Sin(phi) * Mathf.Sin(dec) + Mathf.Cos(phi) * Mathf.Cos(dec) * Mathf.Cos(H)); }
        float Azimuth(float H, float phi, float dec) { return Mathf.Atan2(Mathf.Sin(H), Mathf.Cos(H) * Mathf.Sin(phi) - Mathf.Tan(dec) * Mathf.Cos(phi)); }

        float SiderealTime(float d, float lw) { return RAD * (280.16f + 360.9856235f * d) - lw; }

        // Sun calculations
        float SolarMeanAnomaly(float d) { return RAD * (357.5291f + 0.98560028f * d); }
        float EclipticLongitude(float M)
        {
            float C = RAD * (1.9148f * Mathf.Sin(M) + 0.02f * Mathf.Sin(2 * M) + 0.0003f * Mathf.Sin(3 * M));   // equation of center
            float P = RAD * 102.9372f;  // perihelion of the Earth

            return M + C + P + Mathf.PI;
        }

        /// <summary>
        /// Calculates sun coordinates.
        /// </summary>
        /// <param name="d">Number of days since the year 2000.</param>
        /// <returns>A vector where x represents the Declination and y represents the Right Ascension.</returns>
        Vector2 SunCoords(float d)
        {
            float M = SolarMeanAnomaly(d);
            float L = EclipticLongitude(M);

            return new Vector2(Declination(L, 0), RightAscension(L, 0));
        }

        /// <summary>
        /// Calculates the sun's position.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns>A vector where x represents the Azimuth and y represents the Altitude.</returns>
        public Vector2 GetSunPosition(DateTime date, float lat, float lng)
        {
            float lw = RAD * -lng;
            float phi = RAD * lat;
            float d = (float)ToDays(date);

            Vector2 c = SunCoords(d);
            float H = SiderealTime(d, lw) - c.y;

            //Debug.Log("Azimuth: " + Azimuth(H, phi, c.x) * Mathf.Rad2Deg);
            //Debug.Log("Altitude: " + Altitude(H, phi, c.x) * Mathf.Rad2Deg);

            // TODO: Make sure the sun is rising and setting in the right position (i.e. that it's not flipped)
            return new Vector2(Azimuth(H, phi, c.x) * Mathf.Rad2Deg, Altitude(H, phi, c.x) * Mathf.Rad2Deg);
        }
    }
}

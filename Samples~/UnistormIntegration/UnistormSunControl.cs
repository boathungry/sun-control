using UnityEngine;
using UniStorm;
using SunControl;
using System;

public class UnistormSunControl : MonoBehaviour
{
    public UniStormSystem unistormInstance;

    private SunControl.SunControl sunControl;
    private DateTime dateTime;
    private float timeFloat = 0f;

    void Start()
    {
        sunControl = gameObject.GetComponent<SunControl.SunControl>();
        UpdateDateTime();
    }

    void UpdateDateTime()
    {
        dateTime = sunControl.GetDateTime();
        unistormInstance.SunRevolution = Convert.ToInt32(sunControl.azimuth);
        timeFloat = (sunControl.altitude + 100) / 360;
        unistormInstance.m_TimeFloat = timeFloat;
    }

    void Update()
    {
        if (dateTime != sunControl.GetDateTime() || Math.Abs(timeFloat - unistormInstance.m_TimeFloat) > 0.05)
        {
            UpdateDateTime();
        }
    }
}

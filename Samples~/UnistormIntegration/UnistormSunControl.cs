using UnityEngine;
using UniStorm;
using SunControl;
using System;

public class UnistormSunControl : MonoBehaviour
{
    public UniStormSystem unistormInstance;

    private SunControl.SunControl sunControl;
    private DateTime dateTime;

    void Start()
    {
        sunControl = gameObject.GetComponent<SunControl.SunControl>();
        UpdateDateTime();
    }

    void UpdateDateTime()
    {
        dateTime = sunControl.GetDateTime();
        unistormInstance.SunRevolution = Convert.ToInt32(sunControl.azimuth);
        unistormInstance.m_TimeFloat = (sunControl.altitude + 100) / 360;
    }

    void Update()
    {
        if (dateTime != sunControl.GetDateTime())
        {
            UpdateDateTime();
        }
    }
}

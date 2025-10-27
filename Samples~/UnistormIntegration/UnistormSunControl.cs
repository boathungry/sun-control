using UnityEngine;
using UniStorm;
using SunControl;
using System;

public class UnistormSunControl : MonoBehaviour
{
    public UniStormManager unistormManager;

    private SunControl.SunControl sunControl;
    private DateTime dateTime;

    void Start()
    {
        if (!unistormManager)
        {
            unistormManager = FindFirstObjectByType<UniStormManager>();
        }
        sunControl = gameObject.GetComponent<SunControl.SunControl>();
        UpdateDateTime();
    }

    void UpdateDateTime()
    {
        dateTime = sunControl.GetDateTime();
    }

    void Update()
    {
        if (dateTime != sunControl.GetDateTime())
        {
            UpdateDateTime();
        }
    }
}

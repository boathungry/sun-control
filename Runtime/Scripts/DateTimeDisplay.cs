using SunControl;
using System;
using UnityEngine;

public class DateTimeDisplay : MonoBehaviour
{
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int fontSize = 30;
    public SunControl.SunControl sunControl;
    private DateTime dateTime;

    void Start()
    {
        
    }

    void Update()
    {
        dateTime = sunControl.GetDateTime();
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect timeRect = new Rect(5, h - 80, w, h);
        Rect dateRect = new Rect(5, h - 50, w, h);

        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        
        string timeText = (dateTime != null) ? $"{dateTime.ToShortTimeString()}" : "--:--";
        string dateText = (dateTime != null) ? $"{dateTime.ToShortDateString()}" : "--:--";

        GUI.Label(timeRect, timeText, style);
        GUI.Label(dateRect, dateText, style);
    }
}

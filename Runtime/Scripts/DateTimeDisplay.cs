using SunControl;
using System;
using UnityEngine;
using TMPro;

public class DateTimeDisplay : MonoBehaviour
{
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int fontSize = 30;
    public SunControl.SunControl sunControl;
    public TextMeshProUGUI dateDisplay;
    public TextMeshProUGUI timeDisplay;
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
        string timeText = (dateTime != null) ? $"{dateTime.ToShortTimeString()}" : "--:--";
        string dateText = (dateTime != null) ? $"{dateTime.ToShortDateString()}" : "--/--/----";

        if (dateDisplay != null && timeDisplay != null)
        {
            dateDisplay.SetText(dateText);
            timeDisplay.SetText(timeText);
        }
        else
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect timeRect = new Rect(5, h - 80, w, h);
            Rect dateRect = new Rect(5, h - 50, w, h);

            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = fontSize;
            style.normal.textColor = textColor;

            GUI.Label(timeRect, timeText, style);
            GUI.Label(dateRect, dateText, style);
        }
    }
}

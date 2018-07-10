using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    TimeSpan quater_time;

    void Start()
    {
        quater_time = new TimeSpan(0, 15, 0);
    }

    void DecreaseTime()
    {
        quater_time -= new TimeSpan(0, 0, 30);

        CheckQuarter();
        UpdateTimeDisplay();
    }

    void UpdateTimeDisplay()
    {
        transform.Find("Time").GetComponent<Text>().text = quater_time.Minutes.ToString() + ":" + quater_time.Seconds.ToString();
    }

    void CheckQuarter()
    {
        if (quater_time.TotalSeconds <= 0)
        {
            NextQuarter();
        }
    }

    void NextQuarter()
    {
        Text quarter_field = transform.Find("Quarter").GetComponent<Text>();
        switch (quarter_field.text)
        {
            case "1st":
                quarter_field.text = "2nd";
                break;
            case "2nd":
                quarter_field.text = "3rd";
                break;
            case "3rd":
                quarter_field.text = "4th";
                break;
            case "4th":
                quater_time = new TimeSpan(0, 0, 0);
                quarter_field.text = "End";
                return;
        }

        quater_time = new TimeSpan(0, 15, 0);
    }
}

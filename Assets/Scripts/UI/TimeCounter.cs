using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    TimeSpan current_quarter_time;
    TimeSpan max_quarter_time;

    void Start()
    {
        max_quarter_time = new TimeSpan(0, 10, 0);

        current_quarter_time = max_quarter_time;
        UpdateTimeDisplay();
    }

    public void DecreaseTime()
    {
        current_quarter_time -= new TimeSpan(0, 0, 30);

        CheckQuarter();
        UpdateTimeDisplay();
    }

    void UpdateTimeDisplay()
    {
        transform.Find("Time").GetComponent<Text>().text = current_quarter_time.Minutes.ToString() + ":" + current_quarter_time.Seconds.ToString("D2");
    }

    void CheckQuarter()
    {
        if (current_quarter_time.TotalSeconds <= 0)
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
                current_quarter_time = new TimeSpan(0, 0, 0);
                quarter_field.text = "End";
                FindObjectOfType<PhaseController>().GameOver();
                return;
        }

        current_quarter_time = max_quarter_time;
    }
}

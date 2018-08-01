using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotClock : MonoBehaviour
{
    public int starting_value;
    int current_value;

    void Start()
    {
        current_value = starting_value;
    }

    public void Restart()
    {
        current_value = starting_value;
        Display();
    }

    public void DecreaseTime()
    {
        current_value--;
        Display();
        if (current_value == 0)
        {
            GetComponent<AudioSource>().Play();
            Invoke("DelayChange", 1f);
        }
    }

    void Display()
    {
        GetComponent<Text>().text = current_value.ToString();
    }

    void DelayChange()
    {
        FindObjectOfType<PhaseController>().ChangeSides(true);
    }
}

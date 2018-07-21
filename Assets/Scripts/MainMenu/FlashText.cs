using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashText : MonoBehaviour
{ 
    int counter = 0;
    public float switch_time;

    void FixedUpdate()
    {
        counter++;
        if (counter > switch_time)
        {
            if (GetComponent<Text>().color.a == 0)
            {
                Color text_color = GetComponent<Text>().color;
                text_color.a = 1;
                GetComponent<Text>().color = text_color;
            }
            else
            {
                Color text_color = GetComponent<Text>().color;
                text_color.a = 0;
                GetComponent<Text>().color = text_color;
            }
            counter = 0;
        }
    }
}

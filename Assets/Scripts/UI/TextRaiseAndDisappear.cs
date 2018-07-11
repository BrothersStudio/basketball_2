using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextRaiseAndDisappear : MonoBehaviour
{
    float birth_time;
    float lifetime = 1.5f;
    float speed = 100f;

    void Start()
    {
        birth_time = Time.timeSinceLevelLoad;
    }

    void FixedUpdate()
    {
        // Die after "lifetime" seconds
        if (Time.timeSinceLevelLoad > birth_time + lifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            // Slowly move up
            Vector3 anchored_position = GetComponent<RectTransform>().anchoredPosition;
            anchored_position.y += speed * Time.deltaTime;
            GetComponent<RectTransform>().anchoredPosition = anchored_position;

            // Slowly disappear
            Color current_color = GetComponent<Text>().color;
            current_color.a = (1 - (Time.timeSinceLevelLoad - birth_time) / lifetime);
            GetComponent<Text>().color = current_color;
        }
    }
}
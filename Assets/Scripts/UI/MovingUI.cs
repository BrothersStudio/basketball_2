using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingUI : MonoBehaviour
{
    bool moving = false;
    float speed = 20;
    Vector3 default_position;

    void Start()
    {
        default_position = GetComponent<RectTransform>().position;
    }

    public void StartMoving()
    {
        moving = true;
        GetComponent<RectTransform>().position = default_position;
    }

    void Update ()
    {
        if (moving)
        {
            GetComponent<RectTransform>().position =
                new Vector3(
                    GetComponent<RectTransform>().position.x + speed,
                    GetComponent<RectTransform>().position.y,
                    GetComponent<RectTransform>().position.z);

            if (GetComponent<RectTransform>().position.x > 3000)
            {
                moving = false;
            }
        }
	}
}

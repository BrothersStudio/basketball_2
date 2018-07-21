using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopAnimation : MonoBehaviour
{
    int counter = 0;
    public Sprite hoop_1;
    public Sprite hoop_2;
    public float switch_time;

	void FixedUpdate ()
    {
        counter++;
        if (counter > switch_time)
        {
            if (GetComponent<SpriteRenderer>().sprite == hoop_1)
            {
                GetComponent<SpriteRenderer>().sprite = hoop_2;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = hoop_1;
            }
            counter = 0;
        }
	}
}

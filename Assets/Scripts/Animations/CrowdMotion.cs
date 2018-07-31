using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMotion : MonoBehaviour
{
    int flip_speed = 0;

    int current_ind = 0;
    public Sprite[] crowd_sprite;

	void FixedUpdate ()
    {
        flip_speed++;
        if (flip_speed > 10)
        {
            flip_speed = 0;

            current_ind++;
            if (current_ind == crowd_sprite.Length)
            {
                current_ind = 0;
            }

            GetComponent<SpriteRenderer>().sprite = crowd_sprite[current_ind];
        }
	}
}

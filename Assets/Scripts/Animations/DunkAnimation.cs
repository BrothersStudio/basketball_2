using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DunkAnimation : MonoBehaviour
{
    public Sprite dunk_1;
    public Sprite dunk_2;
    public Sprite dunk_3;

    int time_in_frame = 0;
    int animation_frame = 0;

	void FixedUpdate ()
    {
        time_in_frame++;
        if (time_in_frame > 15)
        {
            time_in_frame = 0;
            SwitchSprite();
        }
	}

    void SwitchSprite()
    {
        if (animation_frame == 0)
        {
            animation_frame = 1;
            GetComponent<Image>().sprite = dunk_2;
        }
        else if (animation_frame == 1)
        {
            animation_frame = 2;
            GetComponent<Image>().sprite = dunk_3;
        }
        else if (animation_frame == 2)
        {
            animation_frame = 0;
            GetComponent<Image>().sprite = dunk_1;
        }
    }
}

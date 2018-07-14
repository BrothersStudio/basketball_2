using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool passing = false;

    Player pass_destination;

    float ball_speed = 0.2f;
    Vector3 ball_catch_visual_offset;

    void Start()
    {
        ball_catch_visual_offset = transform.localPosition;
    }

    public void Pass(Player new_player)
    {
        passing = true;
        pass_destination = new_player;
    }
	
    public void SetCaught()
    {
        transform.localPosition = ball_catch_visual_offset;
    }

	void Update ()
    {
        if (passing)
        {
            float current_distance = Vector2.Distance(transform.position, pass_destination.transform.position);
            if (current_distance < 0.1f)
            {
                passing = false;
                SetCaught();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, pass_destination.transform.position, ball_speed);
        }
	}
}

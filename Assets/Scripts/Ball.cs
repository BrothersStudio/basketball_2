using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool shooting = false;
    bool passing = false;

    float starting_distance_to_hoop;
    Vector2 hoop_location;

    Player pass_destination;

    public float ball_speed;
    public Vector3 ball_visual_offset;

    void Start()
    {
        hoop_location = GameObject.FindGameObjectWithTag("Basket").transform.position;

        ball_visual_offset = transform.localPosition;
    }

    public void Shoot()
    {
        shooting = true;
        starting_distance_to_hoop = Vector2.Distance(transform.position, hoop_location);
    }

    public void Pass(Player new_player)
    {
        passing = true;
        pass_destination = new_player;
    }
	
	void Update ()
    {
		if (shooting)
        {
            float current_distance = Vector2.Distance(transform.position, hoop_location);
            if (current_distance < 0.1f)
            {
                Destroy(gameObject);
                return;
            }

            float derived_angle = Mathf.PI * ((starting_distance_to_hoop - current_distance) / starting_distance_to_hoop);
            float parabolic_variance = 0.1f * Mathf.Sin(derived_angle);
            transform.position = Vector2.MoveTowards(transform.position, hoop_location, ball_speed) + new Vector2(0, parabolic_variance);
        }
        else if (passing)
        {
            float current_distance = Vector2.Distance(transform.position, pass_destination.transform.position);
            if (current_distance < 0.1f)
            {
                passing = false;
                transform.localPosition = ball_visual_offset;
            }

            transform.position = Vector2.MoveTowards(transform.position, pass_destination.transform.position, ball_speed);
        }
	}
}

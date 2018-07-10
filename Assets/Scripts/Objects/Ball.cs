using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool shooting = false;
    bool passing = false;

    bool rebound_next = false;
    bool rebounding = false;
    Tile rebound_tile;

    float starting_distance_to_hoop;
    Vector2 hoop_location;

    Player pass_destination;

    public float ball_speed;
    Vector3 ball_catch_visual_offset;

    void Start()
    {
        hoop_location = GameObject.FindGameObjectWithTag("Basket").transform.position;

        ball_catch_visual_offset = transform.localPosition;
    }

    public void Shoot()
    {
        rebounding = false;
        rebound_next = false;

        shooting = true;
        starting_distance_to_hoop = Vector2.Distance(transform.position, hoop_location);
    }

    public void ShootRebound(Tile rebound_tile)
    {
        Shoot();

        this.rebound_tile = rebound_tile;
        rebound_tile.SetBall();
        rebound_next = true;
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
		if (shooting)
        {
            float current_distance = Vector2.Distance(transform.position, hoop_location);
            if (current_distance < 0.1f)
            {
                if (rebound_next)
                {
                    rebounding = true;

                    rebound_next = false;
                    shooting = false;
                }
                return;
            }

            float derived_angle = (Mathf.PI - 0.7f) * ((starting_distance_to_hoop - current_distance) / starting_distance_to_hoop) + 0.7f;
            float parabolic_variance = 0.1f * Mathf.Sin(derived_angle);
            transform.position = Vector2.MoveTowards(transform.position, hoop_location, ball_speed) + new Vector2(0, parabolic_variance);
        }
        else if (passing)
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
        else if (rebounding)
        {
            float current_distance = Vector2.Distance(transform.position, rebound_tile.transform.position);
            if (current_distance < 0.2f)
            {
                rebounding = false;
                transform.localPosition = new Vector3(0, 0.06f, -0.25f);
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, rebound_tile.transform.position, ball_speed);
        }
	}
}

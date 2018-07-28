using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool dribble_down = true;

    bool passing = false;

    Player pass_destination;

    float orig_ball_speed = 0.3f;
    float current_ball_speed;

    public GameObject sweat_particle_prefab;

    Vector3 current_location;

    public void Pass(Player new_player)
    {
        current_ball_speed = orig_ball_speed;

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = current_location;
        dribble_down = true;

        passing = true;
        pass_destination = new_player;

        Instantiate(sweat_particle_prefab, transform.position, Quaternion.identity);
    }
	
    public void DribbleSound()
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
        GetComponent<AudioSource>().Play();
    }

	void Update ()
    {
        if (passing)
        {
            float current_distance = Vector2.Distance(transform.position, pass_destination.transform.position);
            if (current_distance < 0.1f)
            {
                passing = false;
                transform.rotation = Quaternion.identity;

                FixPositionFacing(pass_destination);
                Instantiate(sweat_particle_prefab, transform.position, Quaternion.identity);
                return;
            }

            current_ball_speed = Mathf.Clamp(0.95f * current_ball_speed, orig_ball_speed * 0.2f, orig_ball_speed);
            transform.position = Vector2.MoveTowards(transform.position, pass_destination.transform.position, current_ball_speed);
            transform.Rotate(new Vector3(0, 0, 20f));
        }
        else
        {
            if (dribble_down && transform.localPosition.y > -0.13f)
            {
                transform.Translate(new Vector3(0, -0.03f, 0));
            }
            else if (dribble_down && transform.localPosition.y <= -0.13f)
            {
                transform.localScale = new Vector3(1.3f, 0.73f, 1);
                DribbleSound();
                dribble_down = false;
            }
            else if (!dribble_down && transform.localPosition.y < -0.05f)
            {
                ScaleTowardOne();
                transform.Translate(new Vector3(0, 0.03f, 0));
            }
            else if (!dribble_down && transform.localPosition.y >= -0.05f)
            {
                dribble_down = true;
            }
        }
	}

    void ScaleTowardOne()
    {
        if (transform.localScale.x >= 1)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.05f, transform.localScale.y + 0.05f, 1);
        }
    }

    public void FixPositionFacing(Player player)
    {
        switch (player.facing)
        {
            case SpriteFacing.SE:
                current_location = new Vector3(0.06f, -0.01f, -0.25f);
                transform.localPosition = new Vector3(0.06f, -0.01f, -0.25f);
                break;
            case SpriteFacing.SW:
                current_location = new Vector3(-0.06f, -0.01f, -0.25f);
                transform.localPosition = new Vector3(-0.06f, -0.01f, -0.25f);
                break;
            case SpriteFacing.NE:
                current_location = new Vector3(0.06f, -0.01f, 0.25f);
                transform.localPosition = new Vector3(0.06f, -0.01f, 0.25f);
                break;
            case SpriteFacing.NW:
                current_location = new Vector3(-0.06f, -0.01f, 0.25f);
                transform.localPosition = new Vector3(-0.06f, -0.01f, 0.25f);
                break;
        }
    }
}

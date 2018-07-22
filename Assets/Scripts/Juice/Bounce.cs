using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public bool stop = false;

    bool tallen = true;
    bool shorten = false;

    public float x_growth = 0;
    float max_x = 0.04f;
    float min_x = -0.04f;

    float y_growth = 0;

    float grow_speed = 0.005f;

    bool values_set = false;
    Vector3 original_position;
    Vector3 original_scale;

    Team team;
    PhaseController phase;

    void Start()
    {
        team = GetComponent<Player>().team;
        phase = FindObjectOfType<PhaseController>();

        original_scale = transform.localScale;

        x_growth = Random.Range(min_x, max_x);
        y_growth = -x_growth;
    }

	void Update ()
    {
        CheckPhase();
        if (!stop && GetComponent<FallIntoPlace>().done)
        {
            if (tallen && x_growth > min_x)
            {
                x_growth -= grow_speed;
                y_growth += grow_speed;
            }
            else if (tallen)
            {
                tallen = false;
                shorten = true;
            }
            else if (shorten && x_growth < max_x)
            {
                x_growth += grow_speed;
                y_growth -= grow_speed;
            }
            else if (shorten)
            {
                tallen = true;
                shorten = false;
            }

            //transform.localPosition = original_position + new Vector3(0, y_growth / 10f, 0);
            transform.localScale = new Vector3(original_scale.x + x_growth, original_scale.y + y_growth, 1);
        }
	}

    void CheckPhase()
    {
        if (GetComponent<Player>().IsDone())
        {
            stop = true;
        }
        else if (phase.current_phase == Phase.TeamAAct && team == Team.A)
        {
            stop = false;
        }
        else if (phase.current_phase == Phase.TeamAAct && team == Team.B)
        {
            stop = true;
        }
        else if (phase.current_phase == Phase.TeamBAct && team == Team.B)
        {
            stop = false;
        }
        else if (phase.current_phase == Phase.TeamBAct && team == Team.A)
        {
            stop = true;
        }
    }
}

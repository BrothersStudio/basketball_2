using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    bool shooting = false;
    float starting_distance_to_hoop;
    Vector2 hoop_location;
    public float ball_speed;

    void Start()
    {
        hoop_location = GameObject.FindGameObjectWithTag("Basket").transform.position;
    }

    public void Shoot()
    {
        shooting = true;
        starting_distance_to_hoop = Vector2.Distance(transform.position, hoop_location);
    }
	
	// Update is called once per frame
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
            Debug.Log(derived_angle);
            float parabolic_variance = 0.1f * Mathf.Sin(derived_angle);
            transform.position = Vector2.MoveTowards(transform.position, hoop_location, ball_speed) + new Vector2(0, parabolic_variance);
        }
	}
}

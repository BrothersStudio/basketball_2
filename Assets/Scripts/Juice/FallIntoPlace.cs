using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallIntoPlace : MonoBehaviour
{
    public bool done = false;
    bool falling = false;
    Vector3 final_position;

    public AudioClip impact_sound;

    public void SetFinalPosition(Vector3 final_position)
    {
        this.final_position = final_position;
    }

    public void FallSoon(bool long_time = false)
    {
        if (!long_time)
        {
            Invoke("SetFallTime", Random.Range(0, 0.3f));
        }
        else
        {
            Invoke("SetFallTime", Random.Range(0.3f, 0.7f));
        }
    }

    void SetFallTime()
    {
        falling = true;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, final_position) > 0.1f && falling)
        {
            Vector3 position = transform.position;
            position.y -= 1;
            transform.position = position;

        }
        else if (falling)  // Done falling
        {
            done = true;
            falling = false;

            GetComponent<AudioSource>().clip = impact_sound;
            GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1f);
            GetComponent<AudioSource>().Play();

            if (transform.childCount > 0)
            {
                if (transform.GetChild(0).GetComponent<FallIntoPlace>() != null)
                {
                    transform.GetChild(0).GetComponent<FallIntoPlace>().FallSoon(true);
                }
            }
        }
    }
}

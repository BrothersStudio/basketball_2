using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSounds : MonoBehaviour
{
    public AudioClip move_click;
    public AudioClip select_click;

    public void Move()
    {
        GetComponent<AudioSource>().clip = move_click;
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        GetComponent<AudioSource>().Play();
    }

    public void Select()
    {
        GetComponent<AudioSource>().clip = select_click;
        GetComponent<AudioSource>().pitch = 1;
        GetComponent<AudioSource>().Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelector : MonoBehaviour
{
    public AudioClip battle_song_1;
    public AudioClip battle_song_2;

	void Awake ()
    {
		switch (Progression.level)
        {
            case 0:
                GetComponent<AudioSource>().clip = battle_song_2;
                break;
            case 1:
                GetComponent<AudioSource>().clip = battle_song_1;
                break;
            case 2:
                GetComponent<AudioSource>().clip = battle_song_2;
                break;
            case 3:
                GetComponent<AudioSource>().clip = battle_song_1;
                break;
        }
        GetComponent<AudioSource>().Play();
	}
}

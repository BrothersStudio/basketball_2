using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndResults : MonoBehaviour
{
    float quit_game_timer;

    public Sprite win_sprite;
    public Sprite lose_sprite;

	void Start ()
    {
        quit_game_timer = Time.timeSinceLevelLoad;

        StartCoroutine(DisplayResults());
    }
	
	void Update ()
    {
        if (Input.GetKey("escape"))
        {
            if (Time.timeSinceLevelLoad > quit_game_timer + 1f)
            {
                Debug.Log("QUIT");
                Application.Quit();
            }
        }
        else
        {
            quit_game_timer = Time.timeSinceLevelLoad;
        }
    }

    IEnumerator DisplayResults()
    {
        yield return new WaitForSeconds(0.3f);
        transform.Find("Game 1").gameObject.SetActive(true);
        if (Progression.game_1_victory)
        {
            transform.Find("Icon 1").GetComponent<Image>().sprite = win_sprite;
            transform.Find("Win Lose 1").GetComponent<Text>().text = "Win!";
        }
        else
        {
            transform.Find("Icon 1").GetComponent<Image>().sprite = lose_sprite;
            transform.Find("Win Lose 1").GetComponent<Text>().text = "Loss!";
        }
        transform.Find("Icon 1").gameObject.SetActive(true);
        transform.Find("Win Lose 1").gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        transform.Find("Game 2").gameObject.SetActive(true);
        if (Progression.game_2_victory)
        {
            transform.Find("Icon 2").GetComponent<Image>().sprite = win_sprite;
            transform.Find("Win Lose 2").GetComponent<Text>().text = "Win!";
        }
        else
        {
            transform.Find("Icon 2").GetComponent<Image>().sprite = lose_sprite;
            transform.Find("Win Lose 2").GetComponent<Text>().text = "Loss!";
        }
        transform.Find("Icon 2").gameObject.SetActive(true);
        transform.Find("Win Lose 2").gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        transform.Find("Game 3").gameObject.SetActive(true);
        if (Progression.game_3_victory)
        {
            transform.Find("Icon 3").GetComponent<Image>().sprite = win_sprite;
            transform.Find("Win Lose 3").GetComponent<Text>().text = "Win!";
        }
        else
        {
            transform.Find("Icon 3").GetComponent<Image>().sprite = lose_sprite;
            transform.Find("Win Lose 3").GetComponent<Text>().text = "Loss!";
        }
        transform.Find("Icon 3").gameObject.SetActive(true);
        transform.Find("Win Lose 3").gameObject.SetActive(true);
    }
}

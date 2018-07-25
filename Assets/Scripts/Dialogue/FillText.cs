using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillText : MonoBehaviour
{
    Player focus_player;
    bool play_text = false;

    public void AnimateText()
    {
        Player[] all_players = FindObjectsOfType<Player>();
        focus_player = all_players[Random.Range(0, all_players.Length)];


        string level = Progression.level.ToString();
        Debug.Log(string.Format("Level {0}", level));

        string str = InterviewController.GetInterview("tag1", level, "222222222").message;
        Debug.Log(str);
        gameObject.SetActive(true);
        StartCoroutine(AnimateTextRoutine(str));
    }

    IEnumerator AnimateTextRoutine(string strComplete)
    {
        play_text = true;
        string slow_string = "";
        int i = 0;
        while (i < strComplete.Length)
        {
            //source.Play();  // Use this for playing "animal crossing" sound effects on each new character

            slow_string += strComplete[i++];
            GetComponentInChildren<Text>().text = slow_string;
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(1.00f);
        gameObject.SetActive(false);
        play_text = false;
    }

    void Update()
    {
        if (play_text)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(focus_player.transform.position);
            transform.position = pos + new Vector3(100, 100, 0);
        }
    }
}

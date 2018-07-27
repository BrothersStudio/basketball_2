using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillText : MonoBehaviour
{
    Player focus_player;
    bool play_text = false;

    public void AnimateText(string tag)
    {
        string str = InterviewUtils.GetMessageByTagLevel(tag);
        Debug.Log(string.Format("Displaying interview: {0}", str));
        if (str.Length == 0)
            return;

        Player[] all_players = FindObjectsOfType<Player>();
        focus_player = all_players[Random.Range(0, all_players.Length)];
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
        yield return new WaitForSeconds(3.00f);
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

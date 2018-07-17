using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillText : MonoBehaviour
{
    Player focus_player;

    public void AnimateText()
    {
        Player[] all_players = FindObjectsOfType<Player>();
        focus_player = all_players[Random.Range(0, all_players.Length)];

        string str = "Arbitrary test text here!";
        gameObject.SetActive(true);
        StartCoroutine(AnimateTextRoutine(str));
    }

    IEnumerator AnimateTextRoutine(string strComplete)
    {
        string slow_string = "";
        int i = 0;
        while (i < strComplete.Length)
        {
            //source.Play();  // Use this for playing "animal crossing" sound effects on each new character

            slow_string += strComplete[i++];
            GetComponentInChildren<Text>().text = slow_string;
            yield return new WaitForSeconds(0.03f);
        }
    }

    void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(focus_player.transform.position);
        transform.position = pos + new Vector3(100, 100, 0);
    }
}

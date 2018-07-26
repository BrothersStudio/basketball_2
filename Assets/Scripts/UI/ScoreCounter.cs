using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    int team_A_score = 0;
    int team_B_score = 0;

    public List<Sprite> number_sprites;

    public void PlayerScored(Player player)
    {
        if (player.team == Team.A)
        {
            TeamAScores(2);
        }
        else
        {
            TeamBScores(2);
        }
    }

    void TeamAScores(int points)
    {
        team_A_score += points;
        ConvertToSpriteNumbers(team_A_score, transform.Find("Home Score").gameObject);
        //GameObject.Find("Canvas").transform.Find("Box").GetComponent<FillText>().AnimateText();
    }

    void TeamBScores(int points)
    {
        team_B_score += points;
        ConvertToSpriteNumbers(team_B_score, transform.Find("Away Score").gameObject);
        //GameObject.Find("Canvas").transform.Find("Box").GetComponent<FillText>().AnimateText();
    }

    void ConvertToSpriteNumbers(int score, GameObject panel)
    {
        string string_score = score.ToString("00");

        char tens = string_score[0];
        char ones = string_score[1];

        panel.transform.Find("Tens").GetComponent<Image>().sprite = number_sprites[int.Parse(tens.ToString())];
        panel.transform.Find("Ones").GetComponent<Image>().sprite = number_sprites[int.Parse(ones.ToString())];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverStatsWindow : MonoBehaviour
{
    Player current_player = null;

    public void Populate(Player player)
    {
        CancelInvoke();
        gameObject.SetActive(true);

        if (player != current_player)
        {
            AddTextAndColor("Shoot Number", player.Normal_Shoot_Skill, player.Shoot_Skill);
            AddTextAndColor("Control Number", player.Normal_Control_Skill, player.Control_Skill);
            AddTextAndColor("Block Number", player.Normal_Block_Skill, player.Block_Skill);
            AddTextAndColor("Steal Number", player.Normal_Steal_Skill, player.Steal_Skill);
            AddTextAndColor("Move Number", player.Normal_Move_Skill, player.Move_Skill);
        }

        Invoke("Disappear", 0.25f);
    }

    void AddTextAndColor(string text_name, int nominal_skill, int current_skill)
    {
        Text number_text = transform.Find(text_name).GetComponent<Text>();
        number_text.text = current_skill.ToString();
        if (current_skill > nominal_skill)
        {
            number_text.color = Color.green;
        }
        else if (current_skill < nominal_skill)
        {
            number_text.color = Color.red;
        }
        else
        {
            number_text.color = Color.white;
        }
    }

    void Disappear()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearAndFade : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DisplayText());
    }

    IEnumerator DisplayText()
    {
        // 0.3 seconds per word

        GetComponent<Text>().text = "IN THE YEAR OF 20XX,\nBASKETBALL WAS FINISHED";
        yield return StartCoroutine(Appear());
        yield return new WaitForSeconds(2.4f);
        yield return StartCoroutine(Fade());

        GetComponent<Text>().text = "THE FANS WERE BORED OF SHOTS\nPREFERRING INSTEAD DUNKS AND SHOVING";
        yield return StartCoroutine(Appear());
        yield return new WaitForSeconds(3.3f);
        yield return StartCoroutine(Fade());

        GetComponent<Text>().text = "SO THE COMMISSIONER BOWED\nTO THE WILL OF THE FANS";
        yield return StartCoroutine(Appear());
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(Fade());

        GetComponent<Text>().text = "INSTATING A NEW LEAGUE\nWITH A NEW, RADICAL SPORT";
        yield return StartCoroutine(Appear());
        yield return new WaitForSeconds(2.7f);
        yield return StartCoroutine(Fade());

        FindObjectOfType<MainMenuScroll>().StartMoving();
    }

    IEnumerator Appear()
    {
        for (float i = 0; i < 1.1; i += 0.05f)
        {
            Color current_color = GetComponent<Text>().color;
            current_color.a = i;
            GetComponent<Text>().color = current_color;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Fade()
    {
        for (float i = 1; i >= -0.1; i -= 0.05f)
        {
            Color current_color = GetComponent<Text>().color;
            current_color.a = i;
            GetComponent<Text>().color = current_color;
            yield return new WaitForEndOfFrame();
        }
    }
}

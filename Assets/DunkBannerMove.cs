using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DunkBannerMove : MonoBehaviour
{
    GameObject banner;
    GameObject hoop;
    GameObject player;

    Image flash;

    public List<AudioClip> crowd_cheers;

    public ParticleSystem score_particles;

    void Start()
    {
        banner = transform.Find("Dunk Banner").gameObject;
        flash = transform.parent.Find("White Flash").GetComponent<Image>();
        hoop = banner.transform.Find("Hoop Sprite").gameObject;
        player = banner.transform.Find("Dunk Sprite").gameObject;

        ResetBanners();
        Dunk();
    }

    void ResetBanners()
    {
        banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000, 0);
        hoop.GetComponent<RectTransform>().anchoredPosition = new Vector2(2000, -123);
        player.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1100, 0.42f);
    }

    public void Dunk()
    {
        ResetBanners();
        StartCoroutine(Flash());
        StartCoroutine(MoveBanner());
        StartCoroutine(MovePlayer());
        StartCoroutine(MoveHoop());

        GetComponent<AudioSource>().clip = crowd_cheers[Random.Range(0, crowd_cheers.Count)];
        GetComponent<AudioSource>().Play();
    }

    IEnumerator Flash()
    {
        for (int i = 0; i < 20; i++)
        {
            float new_alpha = flash.color.a + 0.05f;
            flash.color = new Color(flash.color.r, flash.color.g, flash.color.b, new_alpha);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < 20; i++)
        {
            float new_alpha = flash.color.a - 0.05f;
            flash.color = new Color(flash.color.r, flash.color.g, flash.color.b, new_alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator MoveBanner()
    {
        score_particles.Play();
        banner.SetActive(true);
        for (int i = -2000; i < 0; i += 80)
        {
            banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            yield return new WaitForEndOfFrame();
        }        
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < 2000; i += 60)
        {
            banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            yield return new WaitForEndOfFrame();
        }
        score_particles.Stop();
    }

    IEnumerator MovePlayer()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = -1000; i <= 100; i += 20)
        {
            player.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0.42f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator MoveHoop()
    {
        yield return new WaitForSeconds(0.7f);
        for (int i = 1000; i > 200; i -= 20)
        {
            hoop.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, -123);
            yield return new WaitForEndOfFrame();
        }
    }
}

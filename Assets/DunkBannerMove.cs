using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DunkBannerMove : MonoBehaviour
{
    GameObject banner;
    GameObject dunk_image;
    Image flash;

    public List<AudioClip> crowd_cheers;

    public ParticleSystem score_particles;

    void Start()
    {
        banner = transform.Find("Dunk Banner").gameObject;
        dunk_image = transform.Find("Dunk Background").gameObject;
        flash = transform.parent.Find("White Flash").GetComponent<Image>();

        ResetBanners();
    }

    void ResetBanners()
    {
        banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000, 0);
        dunk_image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1400, 0);
    }

    public void Dunk()
    {
        ResetBanners();
        StartCoroutine(Flash());
        StartCoroutine(MoveBanner());
        StartCoroutine(MoveImage());

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
        banner.SetActive(true);
        for (int i = -2000; i < 0; i += 30)
        {
            banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            yield return new WaitForEndOfFrame();
        }
        score_particles.Play();
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 2000; i += 60)
        {
            banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            yield return new WaitForEndOfFrame();
        }
        score_particles.Stop();
    }

    IEnumerator MoveImage()
    {
        dunk_image.SetActive(true);
        yield return new WaitForSeconds(1);
        for (int i = -1400; i < 0; i += 60)
        {
            dunk_image.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 1400; i += 30)
        {
            dunk_image.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            yield return new WaitForEndOfFrame();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRiseAndFall : MonoBehaviour
{
    Vector3 original_position = new Vector3(0, 0.5f, 0);

    void OnEnable()
    {
        transform.localPosition = original_position;
        StartCoroutine(RiseAndFall());
    }

    IEnumerator RiseAndFall()
    {
        float x_val = 0;
        while (true)
        {
            x_val += 0.1f;
            transform.localPosition = original_position + new Vector3(0, 0.01f * Mathf.Sin(x_val));
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}

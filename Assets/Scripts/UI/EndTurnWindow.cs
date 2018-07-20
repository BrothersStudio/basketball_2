using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnWindow : MonoBehaviour
{
    public Transform arrow;

    Button current_button;
    public Button confirm_button;
    public Button cancel_button;

    [HideInInspector]
    public Highlight highlight;

    void OnEnable()
    {
        highlight.InMenu();

        current_button = confirm_button;
        arrow.SetParent(current_button.transform, false);
    }

    void Update()
    {
        if (Input.GetKeyDown("up"))
        {
            Switch();
        }
        else if (Input.GetKeyDown("down"))
        {
            Switch();
        }
        else if (Input.GetKeyDown("space") || Input.GetKeyDown("return"))
        {
            HandleEnter();
        }
        else if (Input.GetKeyDown("escape"))
        {
            Cancel();
        }
    }

    void Switch()
    {
        if (current_button == confirm_button)
        {
            current_button = cancel_button;
        }
        else if (current_button == cancel_button)
        {
            current_button = confirm_button;
        }
        arrow.SetParent(current_button.transform, false);
    }

    void HandleEnter()
    {
        current_button.onClick.Invoke();
    }

    public void Cancel()
    {
        transform.parent.gameObject.SetActive(false);
        highlight.Reset();
    }
}

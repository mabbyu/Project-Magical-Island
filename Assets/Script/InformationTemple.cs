using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationTemple : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject beforeButton;

    private int selectedTemple;

    public GameObject temple1;
    public GameObject temple2;
    public GameObject temple3;

    void Start()
    {
        for (int i = 0; i < 0; i++)
        {
            selectedTemple = i;
            UpdateResLabel();
        }

        beforeButton.SetActive(false);
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown("d"))
            Next();

        if (Input.GetKeyDown("a"))
            Before();
    }
    */
    public void Next()
    {
        selectedTemple++;
        UpdateResLabel();
    }

    public void Before()
    {
        selectedTemple--;
        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        if (selectedTemple == 0)
        {
            temple1.SetActive(true);
            beforeButton.SetActive(false);
        }
        else
        {
            temple1.SetActive(false);
            beforeButton.SetActive(true);
        }

        if (selectedTemple == 1)
        {
            temple2.SetActive(true);
        }
        else
        {
            temple2.SetActive(false);
        }

        if (selectedTemple == 2)
        {
            temple3.SetActive(true);
            nextButton.SetActive(false);
        }
        else
        {
            temple3.SetActive(false);
            nextButton.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeTempleCamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject templeCamera;

    private void Start()
    {
        templeCamera.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D cld_trggr)
    {
        if (cld_trggr.gameObject.name.Equals("Player"))
        {
            mainCamera.SetActive(false);
            templeCamera.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D cld_trggr)
    {
        if (cld_trggr.gameObject.name.Equals("Player"))
        {
            mainCamera.SetActive(true);
            templeCamera.SetActive(false);
        }
    }
}

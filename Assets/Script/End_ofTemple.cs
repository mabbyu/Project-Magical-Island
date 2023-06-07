using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End_ofTemple : MonoBehaviour
{
    public GameObject wall;

    public void Start()
    {
        wall.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
            wall.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHuggins : MonoBehaviour
{
    public static SpawnHuggins instance;

    public GameObject beforeHuggins;
    public GameObject nextHuggins;

    // Start is called before the first frame update
    void Start()
    {
        nextHuggins.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveAfterCandi : MonoBehaviour
{
    public GameObject dialogPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            DialogueDisplay.instance.objectiveDone = true;

            dialogPanel.gameObject.SetActive(true);
            DialogueDisplay.instance.Initialize();

            Destroy(this.gameObject);
        }
    }
}

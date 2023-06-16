using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationTrigger : MonoBehaviour
{
    //public Text interactText;
    public GameObject dialogPanel;

    //public bool interactAllowed;

    public void Start()
    {
        //interactText.gameObject.SetActive(false);
        dialogPanel.gameObject.SetActive(false);
    }

    public void Update()
    {
        /*
        if (!DialogueDisplay.instance.isDialogue)
        {
            if (interactAllowed && Input.GetKeyDown(KeyCode.E))
                Interact();
        

        if (DialogueDisplay.instance.isDialogue)
            interactText.gameObject.SetActive(false);
        }*/
    }

    public void TriggerDialogue()
    {
        DialogueDisplay.instance.Initialize();

        //interactText.gameObject.SetActive(false);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Interact();
            //interactText.gameObject.SetActive(true);
            //interactAllowed = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            //interactText.gameObject.SetActive(false);
            //interactAllowed = false;
        }
    }

    public void Interact()
    {
        dialogPanel.gameObject.SetActive(true);
        TriggerDialogue();
    }
}

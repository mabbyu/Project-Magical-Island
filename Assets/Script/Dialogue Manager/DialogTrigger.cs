using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Text interactText;
    public GameObject interactPanel;

    public bool interactAllowed;

    private void Start()
    {
        interactText.gameObject.SetActive(false);
        interactPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
       

        if (!DialogueManager.instance.isDialogue)
        {
            if (interactAllowed && Input.GetKeyDown(KeyCode.E))
                Interact();
        }  
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            interactText.gameObject.SetActive(true);
            interactAllowed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            interactText.gameObject.SetActive(false);
            interactAllowed = false;
        }
    }

    public void Interact()
    {
        interactPanel.gameObject.SetActive(true);
        interactText.gameObject.SetActive(false);
        //FindObjectOfType<MenuManager>().SetTimeScale(0);
        TriggerDialogue();
    }
}

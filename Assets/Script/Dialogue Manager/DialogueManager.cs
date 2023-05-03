using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text npcNameText;
    public Text playerNameText;
    public Text dialogueText;
    public bool isDialogue;
    public bool canDialogue;
    private Queue<string> sentences;

    public float typeSpeed;
    float ts;

    public static DialogueManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        sentences = new Queue<string>();
        canDialogue = true;
    }
    
    private void Update()
    {
        ts -= Time.deltaTime;
    }
    
    public void StartDialogue(Dialogue dialogue)
    {
        //MenuManager.instance.SetTimeScale(0);
        isDialogue = true;
        npcNameText.gameObject.SetActive(true);
        playerNameText.gameObject.SetActive(false);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
            sentences.Enqueue(sentence);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count % 2 == 0)
        {
            npcNameText.gameObject.SetActive(true);
            playerNameText.gameObject.SetActive(false);
        }
        else
        {
            playerNameText.gameObject.SetActive(true);
            npcNameText.gameObject.SetActive(false);
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        MenuManager.instance.buttonDialoguePanel.gameObject.SetActive(false);
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        MenuManager.instance.buttonDialoguePanel.gameObject.SetActive(true);
    }

    public void EndDialogue()
    {
        FindObjectOfType<DialogTrigger>().interactPanel.gameObject.SetActive(false);
        //MenuManager.instance.SetTimeScale(1);
        isDialogue = false;
        canDialogue = false;
    }
}

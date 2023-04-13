using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NextDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public string name;
    public Queue<string> sentences;
        
    public GameObject pnlFlash;

    public TextMesh dialogText;
    float CurrentTime;


    public GameObject Player;
    public GameObject loadBlack;

    public int flashNum;


    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        Player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void Update()
    {
        
        if(CurrentTime > 0)
        {
            CurrentTime -= 1 * Time.deltaTime;
        }
        else if(CurrentTime < 0)
        {
            CurrentTime += 0;
            DisplayNextDialog();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            DisplayNextDialog();
        }

        //Debug.Log("Status Dialog " + Object.statusDialog);
        //Debug.Log("Waktu " + CurrentTime);

    }

    public void StartDialog(Dialog dialog)
    {
        //Player.GetComponent<PlayerController>().canMove = false;
        //Player.GetComponent<PlayerController>().horizontalMove = 0;

        CurrentTime = 3f;

        Debug.Log("dialog");
        sentences.Clear();
       

        foreach(string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextDialog();
    }

     public void DisplayNextDialog()
    {
        CurrentTime = 3;
        if(sentences.Count == 0)
        {
            EndDialog();
            return;
        }
         if (sentences.Count == flashNum)
        {
            if(pnlFlash)
                pnlFlash.SetActive(true);
            
        }


        string sentence = sentences.Dequeue();
        //Debug.Log(sentence);


        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }


    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return null;
        }
    }

    void EndDialog()
    {
        CurrentTime = 0;

        if(pnlFlash)
            pnlFlash.SetActive(false);
        pnlFlash = null;

        //Object.statusDialog = false;
        transform.GetChild(0).gameObject.SetActive(false);
        //Player.GetComponent<PlayerController>().canMove = true;

        if(loadBlack)
        {
            loadBlack.SetActive(true);
        }
    }
}

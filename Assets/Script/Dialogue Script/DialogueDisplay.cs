using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    public bool isDialogue = false;
    public bool canDialogue;
    public bool hugginsFlying;

    public GameObject theDialogue;

    public static DialogueDisplay instance;

    public Conversation[] conversation;
    Conversation nextConversation;
    int nextConversationIndex;

    public GameObject speakerLeft;
    public GameObject speakerRight;

    private SpeakerUI speakerUILeft;
    private SpeakerUI speakerUIRight;

    private int activeLineIndex = 0;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void Start()
    {
        canDialogue = true;

        theDialogue.SetActive(false);

        speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUI>();

        nextConversation = conversation[0];

        if (speakerUILeft)
            speakerUILeft.Speaker = conversation[nextConversationIndex].speakerLeft;
        else
            speakerUIRight.Speaker = conversation[nextConversationIndex].speakerRight;
    }

    public void Update()
    {
        
    }

    public void EndConversation()
    {
        nextConversationIndex++;
        nextConversation = conversation[nextConversationIndex];

        speakerUILeft.Hide();
        speakerUIRight.Hide();
        isDialogue = false;
        hugginsFlying = true;
        canDialogue = true;
        theDialogue.SetActive(false);
        Huggins.instance.nextPosIndex++;
    }

    public void Initialize()
    {
        if (!canDialogue)
            return;

        isDialogue = true;
        canDialogue = false;
        theDialogue.SetActive(true);
        activeLineIndex = 0;
        speakerUILeft.Speaker = conversation[nextConversationIndex].speakerLeft;
        speakerUIRight.Speaker = conversation[nextConversationIndex].speakerRight;

        AdvanceLine();
    }

    public void AdvanceLine()
    {
        if (conversation == null || activeLineIndex > conversation[nextConversationIndex].lines.Length)
            return;

        if (!isDialogue)
            Initialize();

        StopAllCoroutines();

        if (activeLineIndex < conversation[nextConversationIndex].lines.Length)
            DisplayLine();
        else
            EndConversation();
    }

    public void DisplayLine()
    {

        Line line = conversation[nextConversationIndex].lines[activeLineIndex];
        Character character = line.character;

        if (speakerUILeft.SpeakerIs(character))
            SetDialog(speakerUILeft, speakerUIRight, line);
        else
            SetDialog(speakerUIRight, speakerUILeft, line);

        activeLineIndex += 1;
    }

    public void SetDialog(SpeakerUI activeSpeakerUI, SpeakerUI inactiveSpeakerUI, Line line)
    {
        activeSpeakerUI.Show();
        inactiveSpeakerUI.Hide();

        activeSpeakerUI.Dialog = ""; StartCoroutine(EffectTypewriter(line.text, activeSpeakerUI));
    }

    private IEnumerator EffectTypewriter(string text, SpeakerUI controller)
    {
        foreach (char character in text.ToCharArray())
        {
            controller.Dialog += character;
            yield return new WaitForSeconds(0.05f);
            // yield return null;
        }
    }
}

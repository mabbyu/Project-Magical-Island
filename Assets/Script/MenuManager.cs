using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    /*-------------------------------------------------------------*/
    [Header("Main Menu")]
    [SerializeField] private bool isMenu;
    /*-------------------------------------------------------------*/
    [Header("Game Pause")]
    public GameObject pauseMenu;
    public GameObject buttonPause;
    public bool isPaused;
    /*-------------------------------------------------------------*/
    [Header("Dead Panel")]
    public GameObject deadPanel;
    public GameObject buttonDeadPanel;
    /*-------------------------------------------------------------*/
    [Header("Done Panel")]
    public GameObject donePanel;
    public GameObject buttonDonePanel;
    /*-------------------------------------------------------------*/
    [Header("Dialogue Panel")]
    public GameObject dialoguePanel;
    public GameObject buttonDialoguePanel;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!GameManager.instance)
            SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMenu)
        {

        }
        else
        {

            if (GameManager.instance.playerAlive == false)
            {
                if (!deadPanel.activeSelf)
                    DeadPanel();
            }
            else
            {
                //if (!donePanel.activeSelf)
                //    DonePanel();

                if (dialoguePanel.activeSelf)
                {
                    DialoguePanel();
                }

                if (Input.GetButtonDown("Pause"))
                {
                    if (isPaused)
                        GameUnPause();
                    else
                        GamePause();
                }
            }
        }
    }
    
    public void GameLoadLevel(string name)
    {
        SetTimeScale(1);
        GameManager.instance.GameLoadLevel(name);
    }

    //timescale
    public void SetTimeScale(int value)
    {
        Time.timeScale = value;
    }

    //Paused
    public void GamePause()
    {
        isPaused = true;

        if (pauseMenu)
            pauseMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(buttonPause);
        SetTimeScale(0);
    }

    public void GameUnPause()
    {
        isPaused = false;

        if (pauseMenu)
            pauseMenu.SetActive(false);

        SetTimeScale(1);
    }

    public void DeadPanel()
    {
        GameUnPause();

        if (deadPanel)
            deadPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(buttonDeadPanel);
        SetTimeScale(0);
    }

    public void DonePanel()
    {
        GameUnPause();

        if (donePanel)
            donePanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(buttonDonePanel);
        SetTimeScale(0);
    }

    public void DialoguePanel()
    {
        GameUnPause();

        if (dialoguePanel)
            dialoguePanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(buttonDialoguePanel);
        
    }

    //application
    public void GameQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
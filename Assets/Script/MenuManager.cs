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
    [SerializeField] private bool isMainMenu;
    public GameObject mainPanel;
    /*-------------------------------------------------------------*/
    [Header("Credit Information")]
    public GameObject informationPanel;
    /*-------------------------------------------------------------*/
    [Header("Credit Panel")]
    public GameObject creditPanel;
    /*-------------------------------------------------------------*/
    [Header("Options Panel")]
    public GameObject optionsPanel;
    /*-------------------------------------------------------------*/
    [Header("Game Pause")]
    public GameObject pauseMenu;
    public GameObject pauseButtons;
    public GameObject buttonPause;
    public bool isPaused;
    public bool isOptionsInGame;
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

    [Header("SFX Panel")]
    public GameObject[] allSfx;
    //public bool isDialog;

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
            if (Input.GetButtonDown("Pause"))
            {
                if (isMainMenu == false)
                    TheMainMenu();
            }
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
                    DialoguePanel();

                if (Input.GetButtonDown("Pause"))
                {
                    if (isPaused && isOptionsInGame)
                    {
                        isOptionsInGame = false;
                        pauseButtons.SetActive(true);
                        optionsPanel.SetActive(false);
                    }
                    else if (isPaused)
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

    //Main Menu
    public void NotMainMenu()
    {
        isMainMenu = false;

        if (mainPanel)
            mainPanel.SetActive(false);
    }

    public void TheMainMenu()
    {
        isMainMenu = true;

        if (mainPanel)
        {
            mainPanel.SetActive(true);

            informationPanel.SetActive(false);
            creditPanel.SetActive(false);
            optionsPanel.SetActive(false);
        }
    }
    public void ButtonInformation()
    {
        NotMainMenu();

        if (informationPanel)
            informationPanel.SetActive(true);
    }

    public void ButtonCredit()
    {
        NotMainMenu();

        if (creditPanel)
            creditPanel.SetActive(true);
    }

    public void ButtonOptions()
    {
        NotMainMenu();

        if (optionsPanel)
            optionsPanel.SetActive(true);

    }


    //Paused in Game
    public void GamePause()
    {
        isPaused = true;
        isOptionsInGame = false;

        if (pauseMenu)
            pauseMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(buttonPause);
        SetTimeScale(0);
    }

    public void GameUnPause()
    {
        isPaused = false;
        isOptionsInGame = false;

        if (pauseMenu)
            pauseMenu.SetActive(false);

        SetTimeScale(1);

        if (deadPanel || donePanel)
            SetTimeScale(0);
    }

    public void NotPauseMenu()
    {
        if (pauseButtons)
            pauseButtons.SetActive(false);
    }

    public void OptionInGame()
    {
        isOptionsInGame = true;

        NotPauseMenu();

        if (optionsPanel)
            optionsPanel.SetActive(true);
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
        if (dialoguePanel)
            dialoguePanel.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(buttonDialoguePanel);
        SetTimeScale(1);

        if (isPaused)
            GamePause();
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool playerAlive;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (GameManager.instance != this)
            Destroy(gameObject);
    }

    void Update()
    {
        
    }

    public void GameLoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}

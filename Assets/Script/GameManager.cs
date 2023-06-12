using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool playerAlive;

    public Animator anim;

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

    public void GameLoadLevel(string lvl)
    {
        anim.SetTrigger("FadeOut");
        StartCoroutine(LoadingLevel(lvl));
    }

   IEnumerator LoadingLevel(string lvl)
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(lvl); 
        anim.SetTrigger("FadeIn");
    }
}

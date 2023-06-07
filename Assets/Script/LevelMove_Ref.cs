using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class LevelMove_Ref : MonoBehaviour
{
    //public int sceneBuildIndex;
    public string nameLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            GameManager.instance.GameLoadLevel(nameLevel);
            //SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }
}
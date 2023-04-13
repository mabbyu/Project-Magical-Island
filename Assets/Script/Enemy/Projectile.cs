using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.SendMessage("GetDamage", SendMessageOptions.DontRequireReceiver);
        //Destroy(gameObject);

        if (collision.gameObject.name.Equals("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}

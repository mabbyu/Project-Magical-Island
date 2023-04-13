using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum AttackMode
{
    chasing, idle, attack, backToPos
}

public class EnemyAI : MonoBehaviour
{
    [HideInInspector]
    public Seeker seeker;
    [HideInInspector]
    public Rigidbody2D rb;
    /*-------------------------------------------------------------*/
    [Header("Move Info")]
    public float speed = 5;
    //public int facingDirection = 1;
    //public bool facingRight = true;
    public Transform enemyGFX;
    /*-------------------------------------------------------------*/
    [Header("A star Pathfinding")]
    public Transform target;
    public float nextWaypointDistance = 3f;
    [HideInInspector]
    public Path path;
    [HideInInspector]
    public int currentWaypoint = 0;
    [HideInInspector] 
    public bool reachedEndOfPath = false;
    /*-------------------------------------------------------------*/
    [Header("Distance Player")]
    public float maximunDistanceToPlayer;
    /*-------------------------------------------------------------*/
    [Header("After Attack")]
    public Transform returnPos;
    
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePath", 0f, .5f);

        Physics2D.IgnoreLayerCollision(8, 7, true);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    
    void FixedUpdate()
    {
        //Path
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
            reachedEndOfPath = false;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force =  direction * speed * Time.deltaTime;

        rb.AddForce(force);

        //transform.position = Vector2.MoveTowards(transform.position, ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized, speed * Time.deltaTime);

        //rb.transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;

        //EnemyGFX
        if (force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        }

        /*
        //EnemyGFX
        if (transform.position.x - transform.position.x < 0 && facingRight)
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        else if (transform.position.x - transform.position.x > 0 && !facingRight)
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        */
    }
    /*
    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    */
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, maximunDistanceToPlayer);
    } 
}

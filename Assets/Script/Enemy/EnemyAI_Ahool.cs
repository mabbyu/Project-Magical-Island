using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI_Ahool : EnemyAI
{
    [Header("NPC Info")]
    public AttackMode currentMode;

    public float attackTimeWait;
    //public float stuckAttackTime;

    float aTime;
    //float saTime;

    Transform lastAttackPos;
    public LayerMask layerMaskChasingStuck;
    public LayerMask layerMaskToFly;
    public float flyHigh, flyForce;

    [Header("SFX")]
    public GameObject moveAudio;
    public GameObject attackAudio;

    private void StartSFX()
    {
        moveAudio.SetActive(false);
        attackAudio.SetActive(false);
    }

    void Start()
    {
        StartSFX();

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);

        Physics2D.IgnoreLayerCollision(8, 7, true);

        returnPos.transform.position = transform.position;
        aTime = attackTimeWait;
        //saTime = stuckAttackTime;

        lastAttackPos = new GameObject("AttackPos").transform ;
    }

    private void Update()
    {
        var dist = Vector2.Distance(transform.position, target.position);
        /*
        if (isAstar)
            speed = 450;
        else
            speed = 5;
        */
        if(target == returnPos)
        {
            currentMode = AttackMode.backToPos;
            //isAstar = true;
            speed = 25;

            attackAudio.SetActive(false);

            if (dist < 1)
            {
                currentMode = AttackMode.chasing;
                target = GameObject.FindGameObjectWithTag("Player").transform;
                Debug.Log("chasing");
            }
        }
        else
        {
            if (currentMode == AttackMode.chasing)
            {
                //isAstar = true;
                speed = 10;

                moveAudio.SetActive(true);

                target = GameObject.FindGameObjectWithTag("Player").transform;

                if (dist <= maximunDistanceToPlayer)
                {
                    currentMode = AttackMode.idle;
                    Debug.Log("idle");
                }
            }
            else if (currentMode == AttackMode.idle)
            {
                //isAstar = false;

                target = GameObject.FindGameObjectWithTag("Player").transform;
                
                rb.velocity = Vector2.zero;

                var dist2 = Vector2.Distance(transform.position, returnPos.position);
                
                if(dist2 > maximunDistanceToPlayer)
                    returnPos.transform.position = transform.position;
                
                aTime -= Time.deltaTime;

                lastAttackPos.transform.position = target.transform.position;

                if (aTime <= 0)
                {
                    speed = 50;
                    currentMode = AttackMode.attack;
                    aTime = attackTimeWait;
                    Debug.Log("attack");
                }
            }
            else if (currentMode == AttackMode.attack)
            {
                //isAstar = false;

                attackAudio.SetActive(true);
                moveAudio.SetActive(false);

                target = lastAttackPos.transform;

                speed = 25;

                //saTime -= Time.deltaTime;

                if (dist <= 0.75 /*|| saTime == 0*/)
                {
                    target = returnPos;
                    //saTime = stuckAttackTime;
                    Debug.Log("back pos");
                }
            }
        }
    }

    void FixedUpdate()
    {
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
        Vector2 force = direction * speed;

        //if (isAstar)
            rb.AddForce(force);
        //else
            //transform.position = Vector2.MoveTowards(transform.position, path.vectorPath[currentWaypoint], speed * Time.deltaTime);

            //transform.position = Vector2.MoveTowards(transform.position, ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized, speed * Time.deltaTime);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;

        //EnemyGFX
        if (force.x >= 0.01f)
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        else if (force.x <= -0.01f)
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, flyHigh, layerMaskToFly);
        
        if (currentMode == AttackMode.chasing || currentMode == AttackMode.idle || currentMode == AttackMode.backToPos)
        {
            if (hit.collider)
            {
                if (hit.transform.tag == "Ground")
                    rb.AddForce(Vector2.up * flyForce);
            }
        }
    }
}


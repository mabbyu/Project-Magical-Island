using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI_Ahool : EnemyAI
{
    [Header("NPC Info")]
    public AttackMode currentMode;

    public float attackTimeWait;
    public float stuckAttackTime;
    
    float aTime;
    float sTime;

    Transform lastAttackPos;
    public LayerMask layerMaskToFly;
    public float flyHigh, flyForce;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);

        Physics2D.IgnoreLayerCollision(8, 7, true);

        returnPos.transform.position = transform.position;
        aTime = attackTimeWait;
        sTime = stuckAttackTime;

        lastAttackPos = new GameObject("AttackPos").transform ;
    }

    private void Update()
    {
        var dist = Vector2.Distance(transform.position, target.position);
        
        if(target == returnPos)
        {
            currentMode = AttackMode.backToPos;

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
                target = GameObject.FindGameObjectWithTag("Player").transform;
                speed = 450;

                if (dist <= maximunDistanceToPlayer)
                {
                    currentMode = AttackMode.idle;
                    Debug.Log("idle");
                }
            }
            else if (currentMode == AttackMode.idle)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
                
                rb.velocity = Vector2.zero;

                var dist2 = Vector2.Distance(transform.position, returnPos.position);
                
                if(dist2 > maximunDistanceToPlayer)
                    returnPos.transform.position = transform.position;
                
                aTime -= Time.deltaTime;

                lastAttackPos.transform.position = target.transform.position;

                if (aTime <= 0)
                {
                    speed = 1250;
                    currentMode = AttackMode.attack;
                    aTime = attackTimeWait;
                    Debug.Log("attack");
                }
            }
            else if (currentMode == AttackMode.attack)
            {
                target = lastAttackPos.transform;
                speed = 1250;

                sTime -= Time.deltaTime;
                
                if (dist <= 2 || sTime <= 0)
                    StartCoroutine(BackPosCd());
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
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

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

    IEnumerator BackPosCd()
    {
        yield return new WaitForSeconds(0.1f);
        target = returnPos;
        sTime = stuckAttackTime;
        Debug.Log("returnPos");
    }
}


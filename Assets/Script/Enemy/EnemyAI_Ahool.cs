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
    public float stuckBackposTime;

    float aTime;
    float saTime;
    float sbTime;

    Transform lastAttackPos;
    public LayerMask layerMaskChasingStuck;
    public LayerMask layerMaskToFly;
    public float flyHigh, flyForce;

    [Header("SFX")]
    public GameObject moveAudio;
    public GameObject attackAudio;

    [Header("Animation")]
    private Animator anim;

    private void StartSFX()
    {
        moveAudio.SetActive(false);
        attackAudio.SetActive(false);
    }

    void Start()
    {
        StartSFX();

        anim = GetComponent<Animator>();

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);

        Physics2D.IgnoreLayerCollision(8, 7, true);

        returnPos.transform.position = transform.position;
        aTime = attackTimeWait;
        saTime = stuckAttackTime;
        sbTime = stuckBackposTime;

        lastAttackPos = new GameObject("AttackPos").transform ;

        if (GameObject.FindGameObjectWithTag("Player"))
            target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        var dist = Vector2.Distance(transform.position, target.position);

        if (target == returnPos)
        {
            currentMode = AttackMode.backToPos;
            attackAudio.SetActive(false);
            sbTime -= Time.deltaTime;

            if (dist < 0.5 || sbTime <= 0)
            {
                currentMode = AttackMode.chasing;
                target = GameObject.FindGameObjectWithTag("Player").transform;
                Debug.Log("NPC Ahool Mengejar Pemain");
            }
        }
        else
        {
            if (currentMode == AttackMode.chasing)
            {
                speed = 8;

                moveAudio.SetActive(true);

                target = GameObject.FindGameObjectWithTag("Player").transform;
                sbTime = stuckBackposTime;

                if (dist <= maximunDistanceToPlayer)
                {
                    currentMode = AttackMode.idle;
                    Debug.Log("NPC Ahool Bersiap Untuk Menyerang");
                }
            }
            else if (currentMode == AttackMode.idle)
            {
                speed = 20;

                if (GameObject.FindGameObjectWithTag("Player"))
                    target = GameObject.FindGameObjectWithTag("Player").transform;

                rb.velocity = Vector2.zero;

                var dist2 = Vector2.Distance(transform.position, returnPos.position);
                
                if(dist2 > maximunDistanceToPlayer)
                    returnPos.transform.position = transform.position;
                
                aTime -= Time.deltaTime;

                lastAttackPos.transform.position = target.transform.position;

                if (aTime <= 0)
                {
                    currentMode = AttackMode.attack;
                    aTime = attackTimeWait;
                    Debug.Log("NPC Ahool Menyerang Pemain");
                }
            }
            else if (currentMode == AttackMode.attack)
            {
                attackAudio.SetActive(true);
                moveAudio.SetActive(false);

                target = lastAttackPos.transform;

                saTime -= Time.deltaTime;

                if (dist <= 1.25 || saTime <= 0)
                {

                    target = returnPos;
                    saTime = stuckAttackTime;
                    Debug.Log("NPC Ahool Kembali Ke Posisi Sebelumnya");
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
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI_Leak : EnemyAI
{
    [Header("NPC Info")]
    public AttackMode currentMode;

    public float attackTimeWait;
    public float stuckAttackTime;

    float aTime;
    float saTime;

    Transform lastAttackPos;
    public LayerMask layerMaskToFly;
    public float flyHigh, flyForce;

    public float distTp;
    public List<Transform> tpPoint = new List<Transform>();

    [Header("SFX")]
    public GameObject moveAudio;
    public GameObject attackAudio;
    public GameObject tpAudio;

    [Header("Animation")]
    private Animator anim;
    public bool isTp;


    private void StartSFX()
    {
        moveAudio.SetActive(false);
        attackAudio.SetActive(false);
        tpAudio.SetActive(false);
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

        lastAttackPos = new GameObject("AttackPos").transform;
    }

    private void Update()
    {
        AnimatorContoller();

        var dist = Vector2.Distance(transform.position, target.position);

        if (target == returnPos)
        {
            currentMode = AttackMode.backToPos;
            attackAudio.SetActive(false);
            tpAudio.SetActive(false);

            if (dist < 0.5)
            {
                currentMode = AttackMode.chasing;
                target = GameObject.FindGameObjectWithTag("Player").transform;
                Debug.Log("chasing");
            }
        }
        else
        {
            if (dist >= distTp)
            {
                isTp = true;

                tpAudio.SetActive(true);
                moveAudio.SetActive(false);
                transform.position = GetClosestSpawn().position;
                StartCoroutine(TpSoundCd());
            }
            if (currentMode == AttackMode.chasing)
            {
                moveAudio.SetActive(true);

                target = GameObject.FindGameObjectWithTag("Player").transform;
                speed = 5;

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

                if (dist2 > maximunDistanceToPlayer)
                    returnPos.transform.position = transform.position;

                aTime -= Time.deltaTime;

                lastAttackPos.transform.position = target.transform.position;
                speed = 25;

                if (aTime <= 0)
                {
                    currentMode = AttackMode.attack;
                    aTime = attackTimeWait;
                    Debug.Log("attack");
                }
            }
            else if (currentMode == AttackMode.attack)
            {
                attackAudio.SetActive(true);
                moveAudio.SetActive(false);
                tpAudio.SetActive(false);


                target = lastAttackPos.transform;

                saTime -= Time.deltaTime;

                if (dist <= 1.25 || saTime <= 0)
                {
                    target = returnPos;
                    saTime = stuckAttackTime;
                }
            }
        }
    }
    
    private void AnimatorContoller()
    {
        anim.SetBool("isMove", currentMode == AttackMode.chasing || currentMode == AttackMode.backToPos);
        anim.SetBool("isAttack", currentMode == AttackMode.attack);
        anim.SetBool("isIdle", currentMode == AttackMode.idle);
        anim.SetBool("isTp", isTp && currentMode == AttackMode.chasing || isTp && currentMode == AttackMode.idle);
    }

    IEnumerator TpSoundCd()
    {
        yield return new WaitForSeconds(2.1f);
        tpAudio.SetActive(false);
        isTp = false;
    }
    Transform GetClosestSpawn()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = target.position;
        foreach (Transform t in tpPoint)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
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

        //transform.position = Vector2.MoveTowards(transform.position, path.vectorPath[currentWaypoint], speed * Time.deltaTime);

        //transform.position = Vector2.MoveTowards(transform.position, ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized, speed * Time.deltaTime);

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, flyHigh, layerMaskToFly);

        if (currentMode == AttackMode.chasing || currentMode == AttackMode.idle || currentMode == AttackMode.backToPos)
        {
            if (hit.collider)
            {
                if (hit.transform.tag == "Ground")
                {
                    rb.AddForce(Vector2.up * flyForce);
                }
            }
        }
    }
}
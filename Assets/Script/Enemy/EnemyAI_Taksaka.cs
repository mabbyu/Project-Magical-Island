using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI_Taksaka : EnemyAI
{
    [Header("NPC Info")]
    public AttackMode currentMode;

    public LayerMask layerMaskToFly;
    public float flyHigh, flyForce;

    public float timeToShoot = 3;
    public float stuckChasingTime;

    float tShoot;
    float scTime;

    public int amountBurst = 3;
    public float timeBetweenBurst;
    float tbShoot;

    public GameObject projectile;
    public float shootSpeed;
    public Transform shootSpawn;
    


    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);

        Physics2D.IgnoreLayerCollision(8, 7, true);

        scTime = stuckChasingTime;
    }

    private void Update()
    {
        var dist = Vector2.Distance(transform.position, target.position);
        target = GameObject.FindGameObjectWithTag("Player").transform;

        tbShoot += Time.deltaTime;

        if (currentMode == AttackMode.chasing)
        {
            speed = 450;

            if (dist <= maximunDistanceToPlayer)
            {
                currentMode = AttackMode.attack;
            }
        }
        else if (currentMode == AttackMode.attack)
        {
            tShoot -= Time.deltaTime;
            if (tShoot <= 0)
            {
                Shoot();
            }
            //speed = 0;
            rb.velocity = Vector2.zero;

            if (dist > maximunDistanceToPlayer)
            {
                currentMode = AttackMode.chasing;
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

    void Shoot()
    {
        tShoot = timeToShoot;
        StartCoroutine(TimerBurst());
    }

    IEnumerator TimerBurst()
    {
        var am = 0;
        while(am < amountBurst)
        {
            if (tbShoot >= timeBetweenBurst)
            {
                Vector2 direction = ((Vector2)target.position - rb.position).normalized;
                var bullet = Instantiate(projectile, shootSpawn.position, Quaternion.identity);
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
                bullet.GetComponent<Rigidbody2D>().AddForce(direction * shootSpeed);

                tbShoot = 0;
            }
            yield return new WaitForSeconds(timeBetweenBurst);
            am++;
        }
    }
}
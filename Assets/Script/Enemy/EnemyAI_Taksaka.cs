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

    float tShoot;

    public int amountBurst = 3;
    public float timeBetweenBurst;
    float tbShoot;

    public GameObject projectile;
    public float shootSpeed;
    public Transform shootSpawn;

    [Header("SFX")]
    public GameObject moveAudio;
    public GameObject attackAudio;

    [Header("Animation")]
    public Animator anim;
    public Animator bodyAnim;
    public bool isShoot;
    public bool isVertical;

    public bool isRight;
    public bool isLeft;

    Vector2 moceDirection;
    Vector2 moveForce;

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

        if (GameObject.FindGameObjectWithTag("Player"))
            target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (path == null)
            return;
        AnimatorContoller();
        
        var dist = Vector2.Distance(transform.position, target.position);

        tbShoot += Time.deltaTime;

        if (currentMode == AttackMode.chasing)
        {
            moveAudio.SetActive(true);

            if (dist <= maximunDistanceToPlayer)
            {
                currentMode = AttackMode.attack;
                Debug.Log("NPC Taksaka Menyerang Pemain");
            }
        }
        else if (currentMode == AttackMode.attack)
        {
            tShoot -= Time.deltaTime;
            moveAudio.SetActive(false);

            if (tShoot <= 0)
                Shoot();

           

            if (dist > maximunDistanceToPlayer)
            {
                currentMode = AttackMode.chasing;
                Debug.Log("NPC Taksaka Mengejar Pemain");
            }
        }


    }
    private void AnimatorContoller()
    {
        anim.SetBool("isMove", currentMode == AttackMode.chasing);
        anim.SetBool("isAttack", isShoot);
        bodyAnim.SetBool("isMoveVertical", isVertical);
    }
    
    private void OnTriggerEnter2D(Collider2D cld_trggr)
    {
        if (cld_trggr.gameObject.name.Equals("Vertical"))
            isVertical = true;
    }

    private void OnTriggerExit2D(Collider2D cld_trggr)
    {
        if (cld_trggr.gameObject.name.Equals("Vertical"))
            isVertical = false;
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

         moceDirection = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
         moveForce = moceDirection * speed;

        if (currentMode == AttackMode.chasing)
            rb.AddForce(moveForce);
        else
           rb.velocity = Vector2.zero;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, flyHigh, layerMaskToFly);

        if (currentMode == AttackMode.chasing || currentMode == AttackMode.idle || currentMode == AttackMode.backToPos)
        {
            if (hit.collider)
            {
                if (hit.transform.tag == "Ground")
                    rb.AddForce(Vector2.up * flyForce);
            }
        }
        //EnemyGFX
        if((transform.position.x - target.transform.position.x) < 0)
        {
            isRight = true;
            isLeft = false;
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            isRight = false;
            isLeft = true;
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
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
                isShoot = true;
                attackAudio.SetActive(true);
                Vector2 direction = ((Vector2)target.position - rb.position).normalized;
                var bullet = Instantiate(projectile, shootSpawn.position, Quaternion.identity);
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
                bullet.GetComponent<Rigidbody2D>().AddForce(direction * shootSpeed);

                tbShoot = 0;
            }
            yield return new WaitForSeconds(timeBetweenBurst);
            isShoot = false;
            attackAudio.SetActive(false);
            am++;
        }
    }
}
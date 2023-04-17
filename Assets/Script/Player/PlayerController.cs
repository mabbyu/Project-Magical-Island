using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    /*-------------------------------------------------------------*/
    [Header("Move info")]
    [SerializeField] public float speed = 5;
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private float ropingSpeed = 4;
    private int facingDirection = 1;
    public float movingInput;
    public float verticalInput;
    private bool facingRight = true;
    public bool canMove = true;
    /*-------------------------------------------------------------*/
    [Header("Animation")]
    public bool isMoving;
    /*-------------------------------------------------------------*/
    [Header("Wall Slide")]
    private bool canWallSlide;
    private bool isWallSliding;
    [SerializeField] private Vector2 wallJumpDirection;
    /*-------------------------------------------------------------*/
    [Header("Rope")]
    [SerializeField] private Vector2 ropeJumpDirection;
    private bool canRope;
    private bool isRope;
    /*-------------------------------------------------------------*/
    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float ropeCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsRope;
    private bool isGrounded;
    private bool wasGrounded = false;
    private bool isWallDetected;
    private bool isRopeDetected;
    /*-------------------------------------------------------------*/
    [Header("Before Falling")]
    public Transform beforeFalling;
    private bool isHurt;
    /*-------------------------------------------------------------*/
    [Header("Falling")]
    //[SerializeField] private float maxJumpTime = 0.75f;
    //private float currentJumpTime;
    [SerializeField] private float minimumFall = 2f;
    private float startOfFall;
    private bool isFalling;
    private bool trueFalling;
    /*-------------------------------------------------------------*/
    [Header("Life Setting")]
    [SerializeField] private int health = 3;
    //[SerializeField] private int currentLife;
    //[SerializeField] private int maxLife = 3;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Text lifes;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        health = 3;
        //GameManager.instance.playerAlive = true;
    }

    void Update()
    {
        CollisionCheck();
        FlipController();
        AnimatorContoller();
        CheckInput();
        Move();

        if (DialogueManager.instance.isDialogue)
        {
            rb.velocity = Vector2.zero;
            canMove = false;
        }
        else
        {
            if (isGrounded)
                canMove = true;
        }

        if (canWallSlide)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.1f);
        }
        
        if (canRope)
        {
            isRope = true;
            rb.velocity = new Vector2(rb.velocity.x, y: ropingSpeed * verticalInput);
            rb.gravityScale = 0;

            if (verticalInput < 0)
                ropingSpeed = 8;
            else
                ropingSpeed = 4;
          
        }
        else
        {
            rb.gravityScale = 4;
        }
        
        //heart
        foreach (Image img in hearts)
            img.sprite = emptyHeart;
        for (int i = 0;  i < health; i++)
            hearts[i].sprite = fullHeart;
    }
    
    private void FixedUpdate()
    {
        CheckGrounded();

        if (!isFalling && wasFalling)
            startOfFall = transform.position.y;

        if (!isGrounded && wasGrounded)
            FallDamage();

        if (trueFalling && isGrounded)
            StartCoroutine(FallCd());

        if (isWallDetected || isRopeDetected)
            startOfFall = 0;  

        isGrounded = wasGrounded;
        isFalling = wasFalling;

    }

    private void LateUpdate()
    {
        if (isGrounded)
        {
            if (!isHurt)
                beforeFalling.transform.position = transform.position;
        }
    }

    bool wasFalling { get { return (wasGrounded && rb.velocity.y < 0); } }

    private void FallDamage()
    {
        float fallDistance = startOfFall - transform.position.y;

        if (fallDistance > minimumFall)
            trueFalling = true;
            //Debug.Log("jatoh" + (startOfFall - transform.position.y) + "distance");
        else
            trueFalling = false;
    }

    private void CheckGrounded()
    {
        wasGrounded = Physics2D.Raycast(transform.position, Vector2.up - Vector2.up, 1.01f);
    }

    private void CheckInput()
    {
        movingInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetAxis("Vertical") < 0)
            canWallSlide = false;

        if (Input.GetButtonDown("Jump"))
            JumpBotton();
    }

    private void Move()
    {
        if (canMove)
        {
            if (isRope)
                rb.velocity = new Vector2(movingInput * 0, rb.velocity.y) * Time.timeScale;
            else
                rb.velocity = new Vector2(movingInput * speed, rb.velocity.y) * Time.timeScale;
        }
    }
    private void Flip()
    {
        if (Time.timeScale == 0)
            return;

        facingDirection = facingDirection * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void JumpBotton()
    {
        if (isWallSliding)
            WallJump();
        else if (isRope)
            RopeJump();
        else if (isGrounded)
            Jump();

        canWallSlide = false;
    }

    private void Jump()
    {
        if (DialogueManager.instance.isDialogue)
            return;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce) * Time.timeScale;
    }

    private void WallJump()
    {
        canMove = false;
        rb.velocity = new Vector2(wallJumpDirection.x * -facingDirection, wallJumpDirection.y) * Time.timeScale;
    }
    
    private void RopeJump()
    {
        if (facingRight && movingInput < 0)
            Flip();
        else if (!facingRight && movingInput > 0)
            Flip();

        transform.position += new Vector3(1 * facingDirection, 0);
        canMove = false;
        rb.velocity = new Vector2(ropeJumpDirection.x * facingDirection, ropeJumpDirection.y) * Time.timeScale;

    }

    private void GetDamage()
    {
        if (isHurt)
            return;

        health--;
        if (health <= 0)
            StartCoroutine(HealthEmpty());
        else
            StartCoroutine(GetHurt());
    }
    
    private void FlipController()
    {
        if(isGrounded)
        {
            if(isWallDetected)
            {
                if (facingRight && movingInput < 0)
                    Flip();
                else if (!facingRight && movingInput > 0)
                    Flip();
            }
        }

        if (rb.velocity.x > 0 && !facingRight)
            Flip();
        else if (rb.velocity.x < 0 && facingRight)
            Flip();
    }

    private void AnimatorContoller()
    {
        if (Time.timeScale > 0)
        {
            if (movingInput < -0.1f || movingInput > 0.1f)
                isMoving = true;
            else
                isMoving = false;
        }

        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isMoving", DialogueManager.instance.isDialogue? false: isMoving);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void CollisionCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
        isRopeDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, ropeCheckDistance, whatIsRope);

        if (isWallDetected && rb.velocity.y < 0)
            canWallSlide = true;

        if (isRopeDetected && rb.velocity.y < 0)
            canRope = true;

        if (!isWallDetected)
        {
            canWallSlide = false;
            isWallSliding = false;
        }

        if (!isRopeDetected)
        {
            canRope = false;
            isRope = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + wallCheckDistance * facingDirection, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + ropeCheckDistance * facingDirection, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }

    private void OnTriggerEnter2D(Collider2D cld_trggr)
    {
        if (cld_trggr.gameObject.name.Equals("Hole"))
        {
            GetDamage();
            transform.position = beforeFalling.transform.position;
        }
    }
    private void OnTriggerStay2D(Collider2D cld_trggr)
    {

    }

    private void OnTriggerExit2D(Collider2D cld_trggr)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D cls)
    {
        if (cls.transform.tag == "Enemy")
        {
            Debug.Log("kena damage");
            //GetDamage();
        }
    }

    IEnumerator FallCd()
    {
        yield return new WaitForSeconds(0.00001f);
        GetDamage();
        //isFalling = false;
        startOfFall = 0;
        trueFalling = false;
    }

    IEnumerator HealthEmpty()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
        GameManager.instance.playerAlive = false;
    }

    IEnumerator GetHurt()
    {
        Physics2D.IgnoreLayerCollision(3,8);
        GetComponent<Animator>().SetLayerWeight(1, 1);
        isHurt = true;

        yield return new WaitForSeconds(3);
        GetComponent<Animator>().SetLayerWeight(1, 0);
        Physics2D.IgnoreLayerCollision(3, 8, false);
        isHurt = false;
    }
}

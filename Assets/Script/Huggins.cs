using System.Collections;
using UnityEngine;

public class Huggins : MonoBehaviour
{
    public static Huggins instance;
    private Animator anim;

    public bool isFlying;
    public bool isIdle;

    public GameObject triggerObj;

    public Transform[] position;
    Transform nextPos;
    public int nextPosIndex;

    public float flySpeed;

    void Awake()
    {
        instance = this;    
    }

    public void Start()
    {
        anim = GetComponent<Animator>();

        nextPos = position[0];
        isIdle = true;
        isFlying = false;
    }

    public void Update()
    {
        AnimatorController();

        if (isIdle)
        {
            triggerObj.SetActive(true);
            isFlying = false;
        }

        if (isFlying)
        {
            triggerObj.SetActive(false);
            isIdle = false;
        }
            
        if (DialogueDisplay.instance.hugginsFlying)
            MoveGameObject();

        if (transform.position == nextPos.position)
            isIdle = true;
    }

    public void AnimatorController()
    {
        anim.SetBool("Idle", isIdle);
        anim.SetBool("isFly", isFlying);
    }

    public void MoveGameObject()
    {
        nextPos = position[nextPosIndex];

        if (transform.position == nextPos.position)
            DialogueDisplay.instance.hugginsFlying = false;
        else
            transform.position = Vector2.MoveTowards(transform.position, nextPos.position, flySpeed * Time.deltaTime);

        isFlying = true;
        isIdle = false;
    }
}
using System.Collections;
using UnityEngine;

public class Huggins : MonoBehaviour
{
    public static Huggins instance;
    private Animator anim;

    public bool isFlying;
    public bool isIdle;

    public Transform[] position;
    Transform nextPos;
    int nextPosIndex;

    public float flySpeed;

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
            isFlying = false;
            nextPosIndex++;
            nextPos = position[nextPosIndex];
        }

        if (isFlying)
            isIdle = false;

        if (DialogueDisplay.instance.hugginsFlying)
            MoveGameObject();
    }

    public void AnimatorController()
    {
        anim.SetBool("Idle", isIdle);
        anim.SetBool("isFly", isFlying);
    }

    public void MoveGameObject()
    {
        if (transform.position == nextPos.position)
            StartCoroutine(NotFly());
        else
            transform.position = Vector2.MoveTowards(transform.position, nextPos.position, flySpeed * Time.deltaTime);

        isFlying = true;
        isIdle = false;
    }

   IEnumerator NotFly()
    {
        yield return new WaitForSeconds(0.01f);
        isIdle = true;
        isFlying = false;
    }
}
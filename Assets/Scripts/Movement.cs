using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Action
{
    Attack,
    Block,
    Roll
}

public class Movement : MonoBehaviour
{

    private Rigidbody2D body;
    private Animator anim;
    [SerializeField] private BoxCollider2D boxCollider;
    private float colliderOffsetY;
    private float colliderSizeY;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float attackRange;
    [SerializeField] private float colliderDistance;

    [SerializeField] private float actionCooldown;
    private float actionCooldownTimer = Mathf.Infinity;
    private InputAction move;
    private InputAction attack;
    private InputAction roll;
    private InputAction block;
    private InputAction down;
    private Vector2 moveDirection = Vector2.zero;
    private float attackTrigger = 0f;
    private float blockTrigger = 0f;
    private float downTrigger = 0f;
    private float rollTrigger = 0f;

    private GameObject oneWayGround;

    [SerializeField] private float footstepsCooldown;
    private float footstepsCooldownTimer = Mathf.Infinity;
    [SerializeField] private List<AudioSource> footsteps;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        colliderOffsetY = boxCollider.offset.y;
        colliderSizeY = boxCollider.size.y;

        playerControls = new PlayerControls();

        //temp
        anim.SetBool("Grounded", true);
    }

    private void OnEnable() {
        move = playerControls.Player.Move;
        attack = playerControls.Player.Attack;
        roll = playerControls.Player.Roll;
        block = playerControls.Player.Block;
        down = playerControls.Player.Down;
        move.Enable();
        attack.Enable();
        roll.Enable();
        block.Enable();
        down.Enable();
    }

    private void OnDisable() {
        move.Disable();
        attack.Disable();
        roll.Disable();
        block.Disable();
        down.Disable();
    }

    private void FixedUpdate()          
    {
        if (!Player.IsDead)
        {
            moveDirection = move.ReadValue<Vector2>();
            attackTrigger = attack.ReadValue<float>();
            blockTrigger = block.ReadValue<float>();
            rollTrigger = roll.ReadValue<float>();
            downTrigger = down.ReadValue<float>();

            if (moveDirection.x != 0)
            {
                body.velocity = new Vector2(moveDirection.x * moveSpeed, body.velocity.y);
                anim.SetInteger("AnimState", 1);
                if (anim.GetBool("Grounded") && footstepsCooldown < footstepsCooldownTimer)
                {
                    int k = Random.Range(0, footsteps.Count);
                    footsteps[k].Play();
                    footstepsCooldownTimer = 0;
                }
            }
            else
            {
                body.velocity = new Vector2(0, body.velocity.y);
                anim.SetInteger("AnimState", 0);
            }

            if (moveDirection.x > 0.01f)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else if (moveDirection.x < -0.01f)
            {
                transform.localScale = new Vector2(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            CheckIfGrounded();

            Jump();

            Down();

            CheckAirSpeed();

            Roll();

            if (CanAttack())
            {
                Attack();

                Block();
            }
        }

        actionCooldownTimer += Time.deltaTime;
        footstepsCooldownTimer += Time.deltaTime;
    }

    private bool CheckAirSpeed()
    {
        if (body.velocity.y > 0.01f)
            anim.SetFloat("AirSpeedY", 1);
        else if (body.velocity.y < -0.01f)
        {
            anim.SetFloat("AirSpeedY", -1);
            boxCollider.offset = new Vector2(boxCollider.offset.x, colliderOffsetY);
            boxCollider.size = new Vector2(boxCollider.size.x, colliderSizeY);
        }
        else
            anim.SetFloat("AirSpeedY", 0);

        return true;
    }

    private bool Jump()
    {
        if (moveDirection.y > 0.01f && anim.GetBool("Grounded") && body.velocity.y < 0.01f)
        {
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);
            boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y + 0.18f);
            boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y - 0.35f);
        }
        return true;
    }

    private bool Down()
    {
        if (oneWayGround != null && downTrigger > 0.01f)
        {
            StartCoroutine(DisableCollision(0.5f));
        }

        return true;
    }

    private void Attack()
    {
        if (attackTrigger > 0.01f && actionCooldown < actionCooldownTimer)
        {
            int k = Random.Range(1, 4);
            switch (k)
            {
                case 1:
                    anim.SetTrigger("Attack1");
                    break;
                case 2:
                    anim.SetTrigger("Attack2");
                    break;
                case 3:
                    anim.SetTrigger("Attack3");
                    break;
            }
            actionCooldownTimer = 0;
            StartCoroutine(AttackEnumeration());
        }
     
    }

    private IEnumerator AttackEnumeration()
    {
        yield return new WaitForSeconds(0.25f);

        var temp = IsInRange(enemyLayer).collider;
        if (temp != null)
        {
            temp.gameObject.GetComponent<RangeEnemy>().Die();
        }
    }

    private RaycastHit2D IsInRange(LayerMask mask)
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * transform.localScale.x * attackRange * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * attackRange, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, mask);

        return hit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * transform.localScale.x * attackRange * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * attackRange, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private bool Block()
    {
        if (blockTrigger > 0.01f && actionCooldown < actionCooldownTimer)
        {
            anim.SetTrigger("Block");
            actionCooldownTimer = 0;
        }
        return true;
    }

    private bool Roll()
    {
        if (rollTrigger > 0.01f && actionCooldown < actionCooldownTimer)
        {
            anim.SetTrigger("Roll");
            StartCoroutine(WaitRoll(0.75f));
        }
        return true;
    }

    private IEnumerator WaitRoll(float delayTime)
    {
        boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - 0.18f);
        boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y - 0.35f);
        actionCooldownTimer = 0;
        yield return new WaitForSeconds(delayTime);
        boxCollider.offset = new Vector2(boxCollider.offset.x, colliderOffsetY);
        boxCollider.size = new Vector2(boxCollider.size.x, colliderSizeY);
    }

    private bool CanAttack()
    {
        return body.velocity == Vector2.zero;
    }

    private void CheckIfGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        anim.SetBool("Grounded", raycastHit2D);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "OneWayGround")
        {
            oneWayGround = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "OneWayGround")
        {
            oneWayGround = null;
        }
    }

    private IEnumerator DisableCollision(float delayTime)
    {
        BoxCollider2D groundCollider = oneWayGround.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(boxCollider, groundCollider);
        yield return new WaitForSeconds(delayTime);
        Physics2D.IgnoreCollision(boxCollider, groundCollider, false);
    }
}

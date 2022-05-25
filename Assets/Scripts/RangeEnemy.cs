using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer = Mathf.Infinity;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private float projectileSpeed;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] private BoxCollider2D boxCollider;

    private Animator anim;
    private WaypointMoving waypointMoving;
    private GameObject currentProjectile;
    private BoxCollider2D projectileBoxCollider;

    private bool isDead = false;
    

    private void Awake()
    {
        currentProjectile = null;
        anim = GetComponent<Animator>();
        waypointMoving = GetComponent<WaypointMoving>();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (attackCooldown < attackCooldownTimer)
            {
                if (PlayerInSight() & !Player.IsDead)
                {
                    attackCooldownTimer = 0;
                    anim.SetTrigger("rangedAttack");
                    StartCoroutine(Attack());
                }
            }

            attackCooldownTimer += Time.deltaTime;
        }
    }

    private IEnumerator Attack()
    {
        waypointMoving.enabled = false;
        yield return new WaitForSeconds(0.85f);
        currentProjectile = Instantiate(prefab, new Vector3(transform.Find("Projectile Spawn").position.x, 
            transform.Find("Projectile Spawn").position.y, transform.position.z), Quaternion.identity, transform);
        currentProjectile.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(projectileSpeed *Mathf.Sign(transform.localScale.x), 0), ForceMode2D.Impulse);
        projectileBoxCollider = currentProjectile.GetComponent<BoxCollider2D>();
        waypointMoving.enabled = true;
    }

    public void Die()
    {
        isDead = true;
        anim.SetTrigger("die");
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<WaypointMoving>().enabled = false;
    }

    public void DisableAnim()
    {
        anim.enabled = false;
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = 
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * transform.localScale.x * range * colliderDistance, 
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * transform.localScale.x * range * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

}

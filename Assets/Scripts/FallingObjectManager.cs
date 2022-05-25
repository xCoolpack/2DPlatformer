using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    private BoxCollider2D boxCollider;
    [SerializeField] private float fallCooldown;
    private float fallCooldownTimer = Mathf.Infinity;
    private GameObject currentGameObject;


    void Awake()
    {
        currentGameObject = null;
    }

    
    void FixedUpdate()
    {
        if (fallCooldown < fallCooldownTimer)
        {
            currentGameObject = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            boxCollider = currentGameObject.GetComponent<BoxCollider2D>();
            fallCooldownTimer = 0;
        }

        if (currentGameObject != null && CheckHitGround())
        {
            Destroy(currentGameObject);
            currentGameObject = null;
            boxCollider = null;
        }
        else 
            fallCooldownTimer += Time.deltaTime;
    }

    private bool CheckHitGround()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);           
    }

}

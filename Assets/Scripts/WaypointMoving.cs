using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMoving : MonoBehaviour
{
    [SerializeField] private Transform leftWaypoints;
    [SerializeField] private Transform rightWaypoints;
    [SerializeField] private float movingSpeed;
    [SerializeField] private bool Horizontal = true;
    private Vector3 initScale;
    private Animator anim;
    private bool movingLeft;
    private bool movingDown;
    

    void Awake()
    {
        initScale = transform.localScale;
        movingLeft = false;
        movingDown = false;

        if (IsEnemy()) 
        {
            anim = GetComponent<Animator>();
        }
    }

    void FixedUpdate()
    {
        if (Horizontal)
        {
            if (movingLeft)
            {
                if (transform.position.x >= leftWaypoints.position.x)
                    MoveInDirection(-1);
                else
                {
                    ChangeDirection();
                }
            }
            else
            {
                if (transform.position.x <= rightWaypoints.position.x)
                    MoveInDirection(1);
                else
                    ChangeDirection();
            }
        }
        else
        {
            if (movingDown)
            {
                if (transform.position.y >= leftWaypoints.position.y)
                    MoveInDirection(-1);
                else
                {
                    ChangeDirection();
                }
            }
            else
            {
                if (transform.position.y <= rightWaypoints.position.y)
                    MoveInDirection(1);
                else
                    ChangeDirection();
            }
        }


    }

    private bool IsEnemy()
    {
        return gameObject.layer == LayerMask.NameToLayer("Enemy");
    }
 
    private void ChangeDirection()
    {
        if (Horizontal)
            movingLeft = !movingLeft;
        else
            movingDown = !movingDown;
    }

    private void MoveInDirection(int direction)
    {
        if (IsEnemy())
            if (Horizontal)      
                transform.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
            else
                transform.localScale = new Vector3(initScale.x, Mathf.Abs(initScale.y) * direction, initScale.z);

        if (Horizontal)
        {
            transform.position = new Vector3(transform.position.x + Time.deltaTime * direction * movingSpeed,
                transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 
                transform.position.y + Time.deltaTime * direction * movingSpeed, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsEnemy()) { }
        else
        {
            if (collision.gameObject.name == "Player")
            {
                collision.gameObject.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsEnemy()) { }
        else
        {
            if (collision.gameObject.name == "Player")
            {
                collision.gameObject.transform.SetParent(GameObject.Find("Level").transform);
            }
        }
    }
}

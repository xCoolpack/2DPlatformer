using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    private float xPos;

    void FixedUpdate()
    {
        if (player.position.x < start.position.x && player.position.x < end.position.x)
            xPos = start.position.x;
        else if (player.position.x > start.position.x && player.position.x > end.position.x)
            xPos = end.position.x;
        else if (player.position.x > start.position.x && player.position.x < end.position.x)
            xPos = player.position.x;
           
        transform.position = new Vector3(xPos, player.position.y + 2f, transform.position.z);


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayMovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(1, 0, 0); // Set direction of platform movement
    public float speed = 2f; // Set the speed of movement
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Move platform back and forth
        transform.position = startPosition + moveDirection * Mathf.Sin(Time.time * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make player a child of platform to move along with it
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Detach player from platform when they leave
            collision.transform.SetParent(null);
        }
    }
}
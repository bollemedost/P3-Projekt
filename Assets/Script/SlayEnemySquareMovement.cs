using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayEnemySquareMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f; // Movement speed
    [SerializeField] private float width = 5f; // Width of the square/rectangle
    [SerializeField] private float height = 3f; // Height of the square/rectangle

    private Vector3[] points; // The four points that form the square/rectangle
    private int currentPointIndex = 0; // Index of the current point the enemy is moving toward

    private void Start()
    {
        // Initialize the four points of the square/rectangle relative to the enemy's starting position
        points = new Vector3[4];
        Vector3 startPosition = transform.position;
        points[0] = startPosition;
        points[1] = startPosition + new Vector3(width, 0, 0); // Move to the right (width)
        points[2] = startPosition + new Vector3(width, 0, height); // Move forward (height)
        points[3] = startPosition + new Vector3(0, 0, height); // Move to the left (width)
    }

    private void Update()
    {
        MoveInSquare();
    }

    private void MoveInSquare()
    {
        // Move towards the current target point
        Vector3 target = points[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the enemy has reached the current point
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // Move to the next point
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }
}


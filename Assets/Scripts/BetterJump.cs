using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    public float fallMultiplier = 2.5f; // Multiplier for faster falling
    public float lowJumpMultiplier = 2f; // Multiplier for low jumps
    private Rigidbody rb; // Rigidbody component

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Correct initialization
    }

    void Update()
    {
        if (rb.velocity.y < 0) // Falling down
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) // Ascending but Jump not held
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}

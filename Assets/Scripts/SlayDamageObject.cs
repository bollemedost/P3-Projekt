using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayDamageObject : MonoBehaviour
{
    public int damageAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        SlayPlayerHealth playerHealth = other.GetComponent<SlayPlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount); // or Heal for heal objects
        }
    }
}

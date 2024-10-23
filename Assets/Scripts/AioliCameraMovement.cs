using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    private float playerHeight = 2.0f;
    private float unitsFromCamera = -9.0f;

    void Update()
    {
        transform.position = new Vector3(player.position.x, playerHeight, unitsFromCamera); //position of camera
    }
}
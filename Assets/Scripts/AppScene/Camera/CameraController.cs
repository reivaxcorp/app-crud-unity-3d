using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSensitivity = 2f;
    public float verticalRotationLimit = 80f;  // Límite de rotación vertical en grados

    private float verticalRotation = 0f;

    void Update()
    {
        // Camera movement
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Camera rotation
        float rotationX = -Input.GetAxis("Mouse Y") * rotationSensitivity;
        verticalRotation += rotationX;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0f);

        float rotationY = Input.GetAxis("Mouse X") * rotationSensitivity;
        transform.Rotate(0f, rotationY, 0f);
    }
}

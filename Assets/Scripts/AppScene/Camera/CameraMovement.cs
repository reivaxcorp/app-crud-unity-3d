using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float upwardSpeed = 2f;

    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        float horizontalMovement = 0f;
        float verticalMovement = 0f;
        float upwardMovement = 0f;

        // Check pressed keys
        if (Input.GetKey(KeyCode.W))
            verticalMovement = 1f;
        if (Input.GetKey(KeyCode.S))
            verticalMovement = -1f;
        if (Input.GetKey(KeyCode.A))
            horizontalMovement = -1f;
        if (Input.GetKey(KeyCode.D))
            horizontalMovement = 1f;
        if (Input.GetKey(KeyCode.Space))
            upwardMovement = 1f;

        // Calculate movement vector
        Vector3 movement = new Vector3(horizontalMovement, upwardMovement, verticalMovement).normalized * movementSpeed * Time.deltaTime;

        // Move the camera
        transform.Translate(movement, Space.Self);
    }
}

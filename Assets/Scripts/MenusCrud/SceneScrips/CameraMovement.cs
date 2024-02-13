using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement) * speed * Time.deltaTime;

        transform.Translate(movement);
    }
}

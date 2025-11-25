using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 10f;
    //public float rotationSpeed = 100f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0, 20, -10); // posisi kamera di atas

    void Update()
    {
        HandleMovement();
        HandleCameraFollow();
    }

    void HandleMovement()
    {
        // Input WASD 
        float moveX = Input.GetAxis("Horizontal"); 
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        if (move.magnitude > 0.01f)
        {
            // Gerakkan kapal
            transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);

            // Rotasi menghadap arah gerakan
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void HandleCameraFollow()
    {
        if (cameraTransform != null)
        {
            // Kamera mengikuti posisi submarine
            cameraTransform.position = transform.position + cameraOffset;
            cameraTransform.LookAt(transform.position); // kamera selalu melihat ke submarine
        }
    }
}


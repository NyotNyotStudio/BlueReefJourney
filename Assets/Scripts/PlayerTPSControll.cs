using UnityEngine;

public class PlayerTPSControll : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 3f;
    public float sprintMultiplier = 2f; // kecepatan tambahan saat sprint

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public Vector3 cameraOffset = new Vector3(0, 5, -10);
    public float minPitch = -30f, maxPitch = 60f;
    public float forwardOffset = -90f; // koreksi arah model

    [Header("Sound Effect")]
    public AudioSource audioSource;
    public AudioClip dashSFX;

    private float yaw, pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;
    }

    void FixedUpdate()
    {
        HandleCamera();
        HandleMovement();
    }

    void HandleCamera()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity, minPitch, maxPitch);

        // Kamera follow
        Quaternion camRot = Quaternion.Euler(pitch, yaw, 0f);
        cameraTransform.position = transform.position + camRot * cameraOffset;
        cameraTransform.LookAt(transform.position);

        // Rotasi submarine (visual)
        transform.rotation = Quaternion.Euler(0f, yaw + forwardOffset, 0f);
    }

    void HandleMovement()
    {
        // Gerak horizontal & maju mundur
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 move = (yawRot * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))).normalized;


        //sprint
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(dashSFX);
        }

        // Naik / Turun
        float vMove = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;

        // Final gerakan
        transform.position += (move * currentSpeed + Vector3.up * vMove * verticalSpeed) * Time.deltaTime;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float boostSpeed = 20f;
    public float rotationSpeed = 50f;
    public float verticalSpeed = 5f;

    [Header("Physics")]
    public float waterDrag = 2f;
    public float buoyancyForce = 9.81f;
    public float targetDepth = 0f;
    public float depthStabilizationSpeed = 2f;

    private Rigidbody rb;
    private float moveInput;
    private float rotationInput;
    private float verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.drag = waterDrag;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");

        verticalInput = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            verticalInput = 1f;
            targetDepth = transform.position.y;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            verticalInput = -1f;
            targetDepth = transform.position.y;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        HandleVerticalMovement();
        HandleBuoyancy();
    }

    void HandleMovement()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : moveSpeed;
        Vector3 forwardMovement = transform.forward * moveInput * currentSpeed;
        rb.velocity = new Vector3(forwardMovement.x, rb.velocity.y, forwardMovement.z);
    }

    void HandleRotation()
    {
        if (rotationInput != 0)
        {
            float rotation = rotationInput * rotationSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0, rotation, 0);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

    void HandleVerticalMovement()
    {
        if (verticalInput != 0)
        {
            Vector3 verticalMovement = Vector3.up * verticalInput * verticalSpeed;
            rb.velocity = new Vector3(rb.velocity.x, verticalMovement.y, rb.velocity.z);
        }
    }

    void HandleBuoyancy()
    {
        if (verticalInput == 0)
        {
            float depthDifference = targetDepth - transform.position.y;
            float stabilizationForce = depthDifference * depthStabilizationSpeed;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Lerp(rb.velocity.y, stabilizationForce, Time.fixedDeltaTime * 5f), rb.velocity.z);
        }
    }
}
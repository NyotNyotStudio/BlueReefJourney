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

    [Header("Boundary Settings")]
    public Transform boundaryBox;
    public Terrain oceanSand;
    public float boundaryPadding = 1f;
    public float terrainHeightOffset = 0f;

    private Rigidbody rb;
    private float moveInput;
    private float rotationInput;
    private float verticalInput;
    private Vector3 minBounds;
    private Vector3 maxBounds;

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

        if (boundaryBox == null)
        {
            GameObject collusion = GameObject.Find("Collusion");
            if (collusion != null)
            {
                boundaryBox = collusion.transform;
            }
        }

        if (oceanSand == null)
        {
            GameObject sand = GameObject.Find("OceanSand");
            if (sand != null)
            {
                oceanSand = sand.GetComponent<Terrain>();
            }
        }

        CalculateBounds();
    }

    void CalculateBounds()
    {
        if (boundaryBox != null)
        {
            Vector3 scale = boundaryBox.localScale;
            Vector3 position = boundaryBox.position;

            minBounds = new Vector3(
                position.x - (scale.x / 2f) + boundaryPadding,
                position.y - (scale.y / 2f) + boundaryPadding,
                position.z - (scale.z / 2f) + boundaryPadding
            );

            maxBounds = new Vector3(
                position.x + (scale.x / 2f) - boundaryPadding,
                position.y + (scale.y / 2f) - boundaryPadding,
                position.z + (scale.z / 2f) - boundaryPadding
            );
        }
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
        ClampToBoundary();
    }

    void HandleMovement()
    {
        if (moveInput != 0)
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : moveSpeed;
            Vector3 movement = transform.right * moveInput * currentSpeed;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
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

    float GetTerrainHeightAtPosition(Vector3 position)
    {
        if (oceanSand == null)
            return minBounds.y;

        Vector3 terrainPos = oceanSand.transform.position;
        TerrainData terrainData = oceanSand.terrainData;

        Vector3 localPos = position - terrainPos;

        float normalizedX = Mathf.Clamp01(localPos.x / terrainData.size.x);
        float normalizedZ = Mathf.Clamp01(localPos.z / terrainData.size.z);

        float height = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);

        float worldHeight = terrainPos.y + height;

        return worldHeight + terrainHeightOffset;
    }

    void ClampToBoundary()
    {
        if (boundaryBox == null) return;

        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minBounds.z, maxBounds.z);

        float terrainHeightAtPosition = GetTerrainHeightAtPosition(clampedPosition);

        clampedPosition.y = Mathf.Clamp(clampedPosition.y, terrainHeightAtPosition, maxBounds.y);

        if (transform.position != clampedPosition)
        {
            transform.position = clampedPosition;

            if (transform.position.x == minBounds.x || transform.position.x == maxBounds.x)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
            if (transform.position.y <= terrainHeightAtPosition || transform.position.y == maxBounds.y)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                targetDepth = transform.position.y;
            }
            if (transform.position.z == minBounds.z || transform.position.z == maxBounds.z)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
        }
    }
}
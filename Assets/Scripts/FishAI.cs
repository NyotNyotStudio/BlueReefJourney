using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishAI : MonoBehaviour
{
    public float swimSpeed = 3f;
    public float turnSpeed = 2.0f;
    public float driftSpeed = 0.5f;
    public float wiggleSpeed = 8f;
    public float wiggleMagnitude = 0.4f;
    public Vector2 waitTimeRange = new Vector2(2f, 5f);

    public GameObject boundaryObject;
    public float minHeight = 20f;

    private BoxCollider boundaryCollider;
    private Vector3 currentTarget;
    private bool isIdle = false;
    private float wiggleTimer = 0f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.drag = 1f;
            rb.angularDrag = 1f;
        }

        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        if (boundaryObject != null)
        {
            boundaryCollider = boundaryObject.GetComponent<BoxCollider>();
        }
        else
        {
            GameObject boundsObj = GameObject.Find("Collusion");
            if (boundsObj != null)
            {
                boundaryObject = boundsObj;
                boundaryCollider = boundsObj.GetComponent<BoxCollider>();
            }
        }

        SetNewDestination();
    }

    void Update()
    {
        if (boundaryCollider == null) return;

        wiggleTimer += Time.deltaTime;

        float dist = Vector3.Distance(transform.position, currentTarget);
        if (dist < 1.5f && !isIdle)
        {
            StartCoroutine(IdleRoutine());
        }

        MoveFish();
    }

    void MoveFish()
    {
        if (!isIdle)
        {
            Vector3 directionToTarget = (currentTarget - transform.position).normalized;
            Vector3 wiggleOffset = transform.right * Mathf.Sin(wiggleTimer * wiggleSpeed) * wiggleMagnitude;
            Vector3 finalDirection = (directionToTarget + wiggleOffset).normalized;

            if (finalDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }

        Vector3 currentEuler = transform.rotation.eulerAngles;
        float clampedX = currentEuler.x;
        if (clampedX > 180) clampedX -= 360;
        clampedX = Mathf.Clamp(clampedX, -45f, 45f);

        transform.rotation = Quaternion.Euler(clampedX, currentEuler.y, 0f);

        float currentSpeed = isIdle ? driftSpeed : swimSpeed;
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    IEnumerator IdleRoutine()
    {
        isIdle = true;
        float waitTime = Random.Range(waitTimeRange.x, waitTimeRange.y);
        yield return new WaitForSeconds(waitTime);
        SetNewDestination();
        isIdle = false;
    }

    void SetNewDestination()
    {
        if (boundaryCollider == null) return;

        Bounds bounds = boundaryCollider.bounds;

        float safeMinY = Mathf.Max(bounds.min.y, minHeight);
        if (safeMinY >= bounds.max.y) safeMinY = bounds.max.y - 1f;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(safeMinY, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        currentTarget = new Vector3(x, y, z);
    }
}
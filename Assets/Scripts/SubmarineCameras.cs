using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8f;

    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            GameObject submarine = GameObject.Find("Submarine");
            if (submarine != null)
            {
                target = submarine.transform;
            }
        }

        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion targetRotationY = Quaternion.Euler(0, target.eulerAngles.y, 0);
        Vector3 rotatedOffset = targetRotationY * offset;

        Vector3 desiredPosition = target.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        Vector3 lookTarget = target.position;
        lookTarget.y = transform.position.y - 2f;

        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }
}
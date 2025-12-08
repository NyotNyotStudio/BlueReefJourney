using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SinkingItem : MonoBehaviour
{
    [SerializeField] private float sinkSpeed = 2f;
    [SerializeField] private float driftSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private float hoverHeight = 5f;

    private float driftOffset;

    private void Start()
    {
        driftOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        Vector3 currentPosition = transform.position;
        float newY = currentPosition.y;

        if (Physics.Raycast(currentPosition + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 200f, terrainLayer))
        {
            float targetY = hit.point.y + hoverHeight;

            if (currentPosition.y > targetY)
            {
                newY -= sinkSpeed * Time.deltaTime;

                float driftX = Mathf.Sin(Time.time + driftOffset) * driftSpeed * Time.deltaTime;
                float driftZ = Mathf.Cos(Time.time * 0.8f + driftOffset) * driftSpeed * Time.deltaTime;

                currentPosition.x += driftX;
                currentPosition.z += driftZ;

                if (newY < targetY)
                {
                    newY = targetY;
                }
            }
            else
            {
                newY = targetY;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            }
        }
        else
        {
            newY -= sinkSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
    }
}
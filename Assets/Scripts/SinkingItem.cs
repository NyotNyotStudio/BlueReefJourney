using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float hoverHeight = 5f;
    [SerializeField] private float landingSmoothing = 0.2f;
    [SerializeField] private bool alignToSlope = true;

    [Header("Movement")]
    [SerializeField] private float sinkSpeed = 2f;
    [SerializeField] private float driftIntensity = 0.5f;
    [SerializeField] private float tumbleSpeed = 15f;

    private Terrain targetTerrain;
    private float _currentYVelocity;
    private float _noiseOffset;
    private Collider[] _myColliders;

    private void Start()
    {
        _noiseOffset = Random.Range(0f, 1000f);
        _myColliders = GetComponentsInChildren<Collider>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (targetTerrain == null)
        {
            targetTerrain = Terrain.activeTerrain;
        }
    }

    private void Update()
    {
        HandleSinkingAndLanding();
    }

    private void HandleSinkingAndLanding()
    {
        Vector3 currentPos = transform.position;
        float groundY = GetGroundHeight(currentPos, out Vector3 groundNormal);
        float targetY = groundY + hoverHeight;

        float noiseX = (Mathf.PerlinNoise(Time.time * 0.5f, _noiseOffset) - 0.5f) * driftIntensity;
        float noiseZ = (Mathf.PerlinNoise(_noiseOffset, Time.time * 0.5f) - 0.5f) * driftIntensity;

        if (currentPos.y > targetY + 0.1f)
        {
            float newY = currentPos.y - (sinkSpeed * Time.deltaTime);

            transform.position = new Vector3(currentPos.x + (noiseX * Time.deltaTime), newY, currentPos.z + (noiseZ * Time.deltaTime));

            transform.Rotate(Vector3.up * tumbleSpeed * Time.deltaTime + Vector3.right * (tumbleSpeed * 0.5f) * Time.deltaTime);
        }
        else
        {
            float newY = Mathf.SmoothDamp(currentPos.y, targetY, ref _currentYVelocity, landingSmoothing);

            float bobbing = Mathf.Sin(Time.time * 1.5f) * 0.05f;
            newY += bobbing * Time.deltaTime;

            transform.position = new Vector3(currentPos.x + (noiseX * Time.deltaTime * 0.2f), newY, currentPos.z + (noiseZ * Time.deltaTime * 0.2f));

            if (alignToSlope)
            {
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            }
        }
    }

    private float GetGroundHeight(Vector3 pos, out Vector3 normal)
    {
        normal = Vector3.up;
        float resultY = -1000f;
        bool foundGround = false;

        Ray ray = new Ray(pos + Vector3.up * 50f, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 200f, groundLayer);

        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (IsMyCollider(hit.collider)) continue;

            if (hit.distance < closestDist)
            {
                resultY = hit.point.y;
                normal = hit.normal;
                closestDist = hit.distance;
                foundGround = true;
            }
        }

        if (!foundGround && targetTerrain != null)
        {
            resultY = targetTerrain.SampleHeight(pos) + targetTerrain.transform.position.y;
            normal = targetTerrain.terrainData.GetInterpolatedNormal(
                (pos.x - targetTerrain.transform.position.x) / targetTerrain.terrainData.size.x,
                (pos.z - targetTerrain.transform.position.z) / targetTerrain.terrainData.size.z
            );
            foundGround = true;
        }

        if (!foundGround)
        {
            resultY = pos.y - hoverHeight;
        }

        return resultY;
    }

    private bool IsMyCollider(Collider col)
    {
        if (_myColliders == null) return false;
        for (int i = 0; i < _myColliders.Length; i++)
        {
            if (_myColliders[i] == col) return true;
        }
        return false;
    }
}
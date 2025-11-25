using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Waypoints Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public float speed = 3f;
    public float reachThreshold = 0.5f;

    private int currentIndex = 0;

    void Start()
    {
        waypoints = WaypointManager.Instance.waypoints;
    }

    void Update()
    {
        if (waypoints.Count == 0) return;

        // Move fish towards current waypoint
        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Make fish look towards waypoint (optional)
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
        }

        // Check if fish reached the waypoint
        if (Vector3.Distance(transform.position, target.position) < reachThreshold)
        {
            currentIndex = (currentIndex + 1) % waypoints.Count;
        }
    }
}

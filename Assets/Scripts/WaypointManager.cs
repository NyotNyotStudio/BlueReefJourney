using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    public static WaypointManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
}

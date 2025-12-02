using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Waypoints : NetworkBehaviour
{
    public List<Transform> WaypointsList { get; private set; }
    Transform waypoints;

    public override void OnNetworkSpawn()
    {
        InitWaypoints();
    }

    void InitWaypoints()
    {
        WaypointsList = new();
        waypoints = GameObject.FindGameObjectsWithTag("NavigationPoint").FirstOrDefault().GetComponent<SpawnInGameManager>().GetWaypoints();
        if (waypoints != null)
        {
            foreach (Transform child in waypoints)
            {
                WaypointsList.Add(child);
            }
        }
    }
}

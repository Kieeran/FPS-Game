using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Waypoints : NetworkBehaviour
{
    public List<Transform> WaypointsList { get; private set; }
    Transform waypoints;
    Transform currentWaypoint;

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

    public Transform GetRandomWaypoint()
    {
        if (WaypointsList == null || WaypointsList.Count == 0)
        {
            Debug.LogError("WaypointsList is null or empty!");
            return null;
        }

        Transform waypoint;
        while (true)
        {
            if (currentWaypoint != null)
            {
                waypoint = WaypointsList[Random.Range(0, WaypointsList.Count)];
                if (waypoint.position != currentWaypoint.position)
                {
                    currentWaypoint = waypoint;
                    break;
                }
            }
            else
            {
                currentWaypoint = WaypointsList[Random.Range(0, WaypointsList.Count)];
                break;
            }
        }
        Debug.Log($"Patrol to {currentWaypoint.gameObject.name}: {currentWaypoint.position}");
        return currentWaypoint;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace AIBot
{
    /// <summary>
    /// Simple container for inspector-assigned waypoint transforms.
    /// Provides helper methods to read current waypoint and advance the index with wrap.
    /// </summary>
    public class WaypointPath : MonoBehaviour
    {
        [Tooltip("Assign ordered waypoint transforms as children or references.")]
        public List<Transform> waypoints;

        [Tooltip("Start index when patrol begins.")]
        public int startIndex = 0;

        /// <summary>Current index into <see cref="waypoints"/>.</summary>
        public int CurrentIndex { get; private set; } = 0;

        private void Reset()
        {
            // default to children transforms if none assigned
            if ((waypoints == null || waypoints.Count == 0) && transform.childCount > 0)
            {
                waypoints = new List<Transform>();
                foreach (Transform child in transform)
                    waypoints.Add(child);
            }
            CurrentIndex = startIndex;
        }

        private void Awake()
        {
            waypoints = InGameManager.Instance.Waypoints.WaypointsList;
            // clamp startIndex
            if (waypoints == null || waypoints.Count == 0) CurrentIndex = 0;
            else CurrentIndex = Mathf.Clamp(startIndex, 0, waypoints.Count - 1);
        }

        /// <summary>
        /// Returns the current waypoint transform or null if none.
        /// </summary>
        public Transform GetCurrent()
        {
            if (waypoints == null || waypoints.Count == 0) return null;
            if (CurrentIndex < 0 || CurrentIndex >= waypoints.Count) return null;
            return waypoints[CurrentIndex];
        }

        /// <summary>
        /// Advances the waypoint index, wrapping to 0 at the end.
        /// </summary>
        public void Next()
        {
            if (waypoints == null || waypoints.Count == 0) return;
            CurrentIndex++;
            if (CurrentIndex >= waypoints.Count) CurrentIndex = 0;
        }

        /// <summary>
        /// Set the waypoint index explicitly (clamped).
        /// </summary>
        public void SetIndex(int idx)
        {
            if (waypoints == null || waypoints.Count == 0) { CurrentIndex = 0; return; }
            CurrentIndex = Mathf.Clamp(idx, 0, waypoints.Count - 1);
        }
    }
}

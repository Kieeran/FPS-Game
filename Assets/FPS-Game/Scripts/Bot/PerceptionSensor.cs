using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using Unity.VisualScripting;
using UnityEngine;

namespace AIBot
{
    /// <summary>
    /// Very small perception sensor that detects a single player transform via distance + FOV + simple raycast occlusion.
    /// Fires OnPlayerSpotted and OnPlayerLost events.
    /// Designed for demo simplicity.
    /// </summary>
    public class PerceptionSensor : MonoBehaviour
    {
        [Header("Perception")]
        [Tooltip("Transform of the detected player.")]
        [SerializeField] Transform targetPlayer;
        [Tooltip("Last seen world position (updated when spotted).")]
        [SerializeField] Vector3 lastKnownPlayerPos = Vector3.zero;

        [Tooltip("Maximum sight distance.")]
        [SerializeField] float viewDistance;
        [SerializeField] LayerMask obstacleMask;

        // [Tooltip("Angle of vision cone in degrees.")]
        // public float viewHalfAngle = 30f;

        // [Tooltip("Layer mask used for occlusion (what can block sight).")]
        // public LayerMask occlusionMask = ~0;

        // // [Tooltip("How often (seconds) to check perception.")]
        // // public float checkInterval = 0.1f;

        // public float height = 1f;
        // // public Color meshColor = Color.yellow;
        // int count;
        // Collider[] colliders = new Collider[50];
        // public LayerMask layers;
        // public List<GameObject> Objects = new List<GameObject>();

        // Mesh mesh;

        // /// <summary>Raised when player becomes visible. Args: worldPos, playerTransform</summary>
        // public event Action<Vector3, GameObject> OnPlayerSpotted;

        // /// <summary>Raised when player becomes not visible (lost).</summary>
        // public event Action OnPlayerLost;

        // /// <summary>Current visibility state (cached)</summary>
        // public bool isPlayerVisible { get; private set; } = false;

        // private float _nextCheck = 0f;
        // public Vector3 posOffset;
        PlayerRoot botRoot;
        float botHorizontalFOV;
        Dictionary<Transform, Color> targetsDebug = new();

        void Awake()
        {
            botRoot = transform.root.GetComponent<PlayerRoot>();
            // if (mesh == null) mesh = CreateWedgeMesh();
        }

        void Start()
        {
            CalculateBotFOV();
        }

        private void Update()
        {
            // if (playerTransform == null) return;

            // if (Time.time >= _nextCheck)
            // {
            //     _nextCheck = Time.time + checkInterval;
            //     Scan();
            // }

            targetsDebug.Clear();
            if (InGameManager.Instance != null)
            {
                PlayerRoot root = CheckSurroundingFOV(InGameManager.Instance.AllCharacters, viewDistance, botHorizontalFOV, obstacleMask);
                if (root != null)
                {
                    // Debug.Log($"Nearest player: {root}");
                    targetPlayer = root.PlayerCamera.GetPlayerCameraTarget();
                }
                else
                {
                    // Debug.Log("There is no nearest player");
                    targetPlayer = null;
                }
            }
        }

        public Transform GetTargetPlayerTransform()
        {
            return targetPlayer;
        }

        public Vector3 GetLastKnownPlayerPos()
        {
            return lastKnownPlayerPos;
        }

        PlayerRoot CheckSurroundingFOV(List<PlayerRoot> targets, float detectRange, float fov, LayerMask obstacleMask)
        {
            PlayerRoot nearest = null;
            float nearestDist = Mathf.Infinity;
            Transform botCameraTransform = botRoot.PlayerCamera.GetPlayerCameraTarget();

            foreach (PlayerRoot targetRoot in targets)
            {
                if (targetRoot == null || targetRoot == botRoot)
                    continue;

                Transform targetCameraTransform = targetRoot.PlayerCamera.GetPlayerCameraTarget();

                Vector3 dir = (targetCameraTransform.position - botCameraTransform.position).normalized;
                float dist = Vector3.Distance(botCameraTransform.position, targetCameraTransform.position);

                // outside range
                if (dist > detectRange)
                {
                    targetsDebug.Add(targetCameraTransform, Color.red);
                    continue;
                }

                // outside FOV
                if (Vector3.Dot(botCameraTransform.forward, dir) < Mathf.Cos(fov * 0.5f * Mathf.Deg2Rad))
                {
                    targetsDebug.Add(targetCameraTransform, Color.yellow);
                    continue;
                }

                if (Physics.Raycast(botCameraTransform.position, dir, out RaycastHit hit, dist, obstacleMask))
                {
                    // Debug.Log($"Hit something: {hit.collider.name}");
                    targetsDebug.Add(targetCameraTransform, Color.yellow);
                    continue;
                }

                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = targetRoot;
                }
            }
            if (nearest != null)
            {
                targetsDebug.Add(nearest.PlayerCamera.GetPlayerCameraTarget(), Color.green);
                lastKnownPlayerPos = nearest.transform.position;
            }
            return nearest;
        }

        // private void Scan()
        // {
        //     count = Physics.OverlapSphereNonAlloc(transform.position, viewDistance, colliders, layers, QueryTriggerInteraction.Collide);

        //     Objects.Clear();
        //     for (int i = 0; i < count; i++)
        //     {
        //         GameObject obj = colliders[i].GameObject();
        //         if (IsInSight(obj))
        //         {
        //             SetPlayerSpotted(obj.transform.position);
        //             Objects.Add(obj);
        //         }
        //         else SetPlayerLost();
        //     }
        // }

        // private void Scan()
        // {
        //     Objects.Clear();
        //     if (playerTransform == null)
        //     {
        //         SetPlayerLost();
        //         return;
        //     }
        //     GameObject obj = playerTransform.gameObject;
        //     if (IsInSight(obj))
        //     {
        //         SetPlayerSpotted(obj.transform.position);
        //         Objects.Add(obj);
        //     }
        //     else
        //     {
        //         SetPlayerLost();
        //     }
        // }

        // private void Scan()
        // {
        //     count = Physics.OverlapSphereNonAlloc(
        //         transform.position, viewDistance, colliders, layers, QueryTriggerInteraction.Collide);

        //     Objects.Clear();
        //     bool foundPlayer = false;
        //     GameObject playerObj = null;
        //     Vector3 playerPos = Vector3.zero;

        //     for (int i = 0; i < count; i++)
        //     {
        //         GameObject obj = colliders[i].gameObject;

        //         if (obj.CompareTag("Player") && IsInSight(obj))
        //         {
        //             playerObj = obj;
        //             playerPos = obj.transform.position;
        //             Objects.Add(obj);
        //             foundPlayer = true;
        //             // Continue looping – there might be more (defensive)
        //         }
        //     }

        //     if (foundPlayer && playerObj != null)
        //     {
        //         // Only fire if we haven't already this scan
        //         if (!isPlayerVisible)
        //         {
        //             isPlayerVisible = true;
        //             LastSeenPos = playerPos;
        //         }
        //         OnPlayerSpotted?.Invoke(playerPos, playerObj);
        //     }
        //     else if (isPlayerVisible)
        //     {
        //         // Only transition to lost if we were previously visible
        //         isPlayerVisible = false;
        //         OnPlayerLost?.Invoke();
        //     }
        // }

        // public bool IsInSight(GameObject obj)
        // {
        //     Vector3 origin = transform.position;
        //     Vector3 dest = obj.transform.position;
        //     Vector3 direction = dest - origin;
        //     if (direction.y < 0 || direction.y > height)
        //     {
        //         Debug.Log("direction.y");
        //         return false;
        //     }

        //     direction.y = 0;
        //     float deltaAngle = Vector3.Angle(direction, transform.forward);
        //     if (deltaAngle > viewHalfAngle)
        //     {
        //         Debug.Log("deltaAngle");
        //         return false;
        //     }

        //     origin.y += height / 2;
        //     dest.y = origin.y;
        //     if (Physics.Linecast(origin, dest, occlusionMask))
        //     {
        //         Debug.Log("occlusionMask");
        //         return false;
        //     }

        //     return true;
        // }

        // private void SetPlayerSpotted(Vector3 pos)
        // {
        //     bool already = isPlayerVisible;
        //     isPlayerVisible = true;
        //     LastSeenPos = pos;
        //     if (!already) OnPlayerSpotted?.Invoke(pos, playerTransform.GameObject());
        //     else
        //     {
        //         // still visible, but update last-seen position so BT can chase live
        //         OnPlayerSpotted?.Invoke(pos, playerTransform.GameObject());
        //     }
        // }

        // private void SetPlayerLost()
        // {
        //     if (!isPlayerVisible) return;
        //     isPlayerVisible = false;
        //     OnPlayerLost?.Invoke();
        // }

        // Mesh CreateWedgeMesh()
        // {
        //     Mesh mesh = new Mesh();

        //     int segments = 10;
        //     int numTriangles = (segments * 4) + 2 + 2; // segment * 4: Top, bottom, the tip and the far end of the mesh
        //     int numVertices = numTriangles * 3;

        //     Vector3[] vertices = new Vector3[numVertices];
        //     int[] triangles = new int[numVertices];

        //     Vector3 bottomCenter = Vector3.zero;
        //     Vector3 bottomLeft = Quaternion.Euler(0, -viewHalfAngle, 0) * Vector3.forward * viewDistance;
        //     Vector3 bottomRight = Quaternion.Euler(0, viewHalfAngle, 0) * Vector3.forward * viewDistance;

        //     Vector3 topCenter = bottomCenter + Vector3.up * height;
        //     Vector3 topRight = bottomRight + Vector3.up * height;
        //     Vector3 topLeft = bottomLeft + Vector3.up * height;

        //     int vert = 0;

        //     // Left side
        //     vertices[vert++] = bottomCenter;
        //     vertices[vert++] = bottomLeft;
        //     vertices[vert++] = topLeft;

        //     vertices[vert++] = topLeft;
        //     vertices[vert++] = topCenter;
        //     vertices[vert++] = bottomCenter;

        //     // Right side
        //     vertices[vert++] = bottomCenter;
        //     vertices[vert++] = bottomRight;
        //     vertices[vert++] = topRight;

        //     vertices[vert++] = topRight;
        //     vertices[vert++] = topCenter;
        //     vertices[vert++] = bottomCenter;

        //     float currentAngle = -viewHalfAngle;
        //     float deltaAngle = viewHalfAngle * 2 / segments;
        //     for (int i = 0; i < segments; ++i)
        //     {
        //         bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * viewDistance;

        //         bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * viewDistance;


        //         topRight = bottomRight + Vector3.up * height;

        //         topLeft = bottomLeft + Vector3.up * height;


        //         // Far side
        //         vertices[vert++] = bottomRight;
        //         vertices[vert++] = bottomLeft;
        //         vertices[vert++] = topRight;

        //         vertices[vert++] = topLeft;
        //         vertices[vert++] = topRight;
        //         vertices[vert++] = bottomLeft;

        //         // Top side
        //         vertices[vert++] = topCenter;
        //         vertices[vert++] = topLeft;
        //         vertices[vert++] = topRight;

        //         // Bottom side
        //         vertices[vert++] = bottomCenter;
        //         vertices[vert++] = bottomLeft;
        //         vertices[vert++] = bottomRight;

        //         currentAngle += deltaAngle;
        //     }

        //     for (int i = 0; i < numVertices; ++i)
        //     {
        //         triangles[i] = i;
        //     }

        //     mesh.vertices = vertices;
        //     mesh.triangles = triangles;
        //     mesh.RecalculateNormals();

        //     return mesh;
        // }

        // private void OnValidate()
        // {
        //     mesh = CreateWedgeMesh();
        // }

        // private void OnDrawGizmos()
        // {
        //     Transform target;
        //     if (!Application.isPlaying)
        //     {
        //         target = transform;
        //     }
        //     else
        //     {
        //         if (PlayerRoot == null) return;
        //         target = PlayerRoot.PlayerCamera.GetPlayerCameraTarget();
        //     }

        //     if (mesh)
        //     {
        //         Gizmos.color = meshColor;
        //         Gizmos.DrawMesh(mesh, target.position + posOffset, target.rotation);
        //     }

        //     if (Application.isPlaying)
        //     {
        //         DrawLinesToUsers();
        //     }

        //     // Gizmos.DrawWireSphere(playerCameraTarget.position, viewDistance);
        //     // for (int i = 0; i < count; i++)
        //     // {
        //     //     Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
        //     // }

        //     // Gizmos.color = Color.green;
        //     // foreach (var obj in Objects)
        //     // {
        //     //     Gizmos.DrawSphere(obj.transform.position, 0.2f);
        //     // }
        // }

        float GetHorizontalFOV(float verticalFov, float aspect)
        {
            float vFovRad = verticalFov * Mathf.Deg2Rad;
            float hFovRad = 2f * Mathf.Atan(Mathf.Tan(vFovRad / 2f) * aspect);
            return hFovRad * Mathf.Rad2Deg;
        }

        void CalculateBotFOV()
        {
            Camera playerCam = Camera.main;
            botHorizontalFOV = GetHorizontalFOV(playerCam.fieldOfView, playerCam.aspect);
        }

        private void OnDrawGizmos()
        {
            Transform target;
            if (!Application.isPlaying)
            {
                target = transform;
            }
            else
            {
                if (botRoot == null) return;
                target = botRoot.PlayerCamera.GetPlayerCameraTarget();
            }

            // Vẽ bán kính detect
            Gizmos.color = new Color(0f, 1f, 1f, 0.12f);
            Gizmos.DrawWireSphere(target.position, viewDistance);

            // Vẽ nón FOV
            Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.15f);
            DrawFOVGizmo(target, viewDistance, botHorizontalFOV);

            // Vẽ đường tới các user
            DrawLinesToUsers();
        }

        void DrawFOVGizmo(Transform eye, float range, float fov)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.25f);

            Vector3 left = Quaternion.Euler(0, -fov * 0.5f, 0) * eye.forward;
            Vector3 right = Quaternion.Euler(0, fov * 0.5f, 0) * eye.forward;

            Gizmos.DrawLine(eye.position, eye.position + left * range);
            Gizmos.DrawLine(eye.position, eye.position + right * range);

            int segments = 32;
            Vector3 prevPoint = eye.position + right * range;
            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.Lerp(fov * 0.5f, -fov * 0.5f, i / (float)segments);
                Vector3 point = eye.position + Quaternion.Euler(0, angle, 0) * eye.forward * range;

                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }

        void DrawLinesToUsers()
        {
            if (targetsDebug == null) return;

            Dictionary<Transform, Color> targets = targetsDebug;
            foreach (var t in targets)
            {
                if (t.Key == null)
                    continue;

                Gizmos.color = t.Value;
                Gizmos.DrawLine(
                    botRoot.PlayerCamera.GetPlayerCameraTarget().position,
                    t.Key.position
                );
            }
        }
    }
}

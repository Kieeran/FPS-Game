using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

namespace AIBot
{
    [Serializable]
    public struct LastKnownData
    {
        public Vector3 position;
        public Quaternion rotation;

        public LastKnownData(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }

        public LastKnownData(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
        }

        public bool IsValid()
        {
            return position != Vector3.zero && rotation != Quaternion.identity;
        }

        public void SetValue(Transform val)
        {
            position = val.position;
            rotation = val.rotation;
        }
    }

    [System.Serializable]
    public class SharedLastKnownData : SharedVariable<LastKnownData>
    {
        public static implicit operator SharedLastKnownData(LastKnownData value)
        {
            return new SharedLastKnownData { Value = value };
        }
    }

    public class PerceptionSensor : MonoBehaviour
    {
        [Header("Perception")]
        [Tooltip("Transform of the detected player.")]
        [SerializeField] Transform targetPlayer;
        [Tooltip("Last seen world data (updated when spotted).")]
        [SerializeField] LastKnownData lastKnownData = new();

        [Tooltip("Maximum sight distance.")]
        [SerializeField] float viewDistance;
        [SerializeField] LayerMask obstacleMask;
        PlayerRoot botRoot;
        float botHorizontalFOV;
        Dictionary<Transform, Color> targetsDebug = new();

        [Header("Search Sampling")]
        [SerializeField] int sampleDirectionCount = 16;
        float sampleRadius = 10f;
        float navMeshSampleMaxDistance = 10f;

        List<Vector3> theoreticalSamplePoints = new();
        List<Vector3> navMeshSamplePoints = new();

        void Awake()
        {
            botRoot = transform.root.GetComponent<PlayerRoot>();
        }

        void Start()
        {
            CalculateBotFOV();
        }

        private void Update()
        {
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

            if (targetPlayer == null && lastKnownData.IsValid())
            {
                // GenerateNavMeshSamplePoints();
            }
        }

        public Transform GetTargetPlayerTransform()
        {
            return targetPlayer;
        }

        public LastKnownData GetLastKnownPlayerData()
        {
            return lastKnownData;
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
                lastKnownData.SetValue(nearest.PlayerCamera.GetPlayerCameraTarget());
            }
            return nearest;
        }

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

        void GenerateNavMeshSamplePoints()
        {
            theoreticalSamplePoints.Clear();
            navMeshSamplePoints.Clear();

            Vector3 center = lastKnownData.position;

            for (int i = 0; i < sampleDirectionCount; i++)
            {
                float angle = i * (360f / sampleDirectionCount);
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                // (1) Điểm lý thuyết trong không gian
                Vector3 theoreticalPoint = center + dir * sampleRadius;
                theoreticalSamplePoints.Add(theoreticalPoint);

                // (2) Nhờ NavMesh tìm điểm đứng hợp lệ gần đó
                if (NavMesh.SamplePosition(
                    theoreticalPoint,
                    out NavMeshHit hit,
                    navMeshSampleMaxDistance,
                    NavMesh.AllAreas))
                {
                    navMeshSamplePoints.Add(hit.position);
                }
            }
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

            DrawSearchSamplingGizmos();
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

        void DrawSearchSamplingGizmos()
        {
            // Điểm lý thuyết (màu vàng)
            Gizmos.color = Color.yellow;
            foreach (var p in theoreticalSamplePoints)
            {
                Gizmos.DrawSphere(p, 0.12f);
            }

            // Điểm NavMesh thật (màu xanh lá)
            Gizmos.color = Color.green;
            foreach (var p in navMeshSamplePoints)
            {
                Gizmos.DrawSphere(p, 0.18f);
            }

            // Nối LKPP → điểm NavMesh
            Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
            foreach (var p in navMeshSamplePoints)
            {
                Gizmos.DrawLine(lastKnownData.position, p);
            }
        }
    }
}

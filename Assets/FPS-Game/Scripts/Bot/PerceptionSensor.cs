using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

namespace AIBot
{
    public class PerceptionSensor : MonoBehaviour
    {
        [Header("Perception")]
        [Tooltip("Transform of the detected player.")]
        [SerializeField] Transform targetPlayer;
        [Tooltip("Last seen world data (updated when spotted).")]
        [SerializeField] TPointData lastKnownData = new();

        [Tooltip("Maximum sight distance.")]
        [SerializeField] float viewDistance;
        [SerializeField] LayerMask obstacleMask;

        public event Action<TPointData> OnPlayerLost;
        bool isTriggerOnPlayerLost = false;

        [SerializeField] BotController botController;
        [SerializeField] BotTactics botTactics;
        PlayerRoot botRoot;
        float botHorizontalFOV;
        Dictionary<Transform, Color> targetsDebug = new();

        [Header("Search Sampling")]
        [SerializeField] int sampleDirectionCount = 16;
        float sampleRadius = 10f;
        float navMeshSampleMaxDistance = 10f;

        List<Vector3> theoreticalSamplePoints = new();
        List<Vector3> navMeshSamplePoints = new();
        List<Transform> currentSearchPath = new();
        [SerializeField] Color botToTPLineColor;

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
                PlayerRoot root = null;
                if (!DebugManager.Instance.IgnorePlayer)
                {
                    root = CheckSurroundingFOV(InGameManager.Instance.AllCharacters, viewDistance, botHorizontalFOV, obstacleMask);
                }

                if (root != null)
                {
                    // Debug.Log($"Nearest player: {root}");
                    targetPlayer = root.PlayerCamera.GetPlayerCameraTarget();

                    isTriggerOnPlayerLost = false;
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

                // Kích hoạt event OnPlayerLost chỉ một lần
                if (!isTriggerOnPlayerLost)
                {
                    isTriggerOnPlayerLost = true;
                    OnPlayerLost.Invoke(lastKnownData);
                }

                // if (currentSearchPath != null && currentSearchPath.Count > 0)
                // {
                //     CheckVisibleTacticalPoints(currentSearchPath, viewDistance, botHorizontalFOV, obstacleMask);
                // }
            }

            ScanInfoPointInArea();
        }

        public void SetCurrentSearchPath(List<Transform> val)
        {
            currentSearchPath = val;
        }

        public Transform GetTargetPlayerTransform()
        {
            return targetPlayer;
        }

        public TPointData GetLastKnownPlayerData()
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

        void ScanInfoPointInArea()
        {
            if (botTactics == null || botTactics.currentVisiblePoint == null) return;

            List<InfoPoint> visiblePoints = botTactics.currentVisiblePoint;
            if (visiblePoints.Count <= 0) return;

            Transform botCameraTransform = botRoot.PlayerCamera.GetPlayerCameraTarget();
            foreach (InfoPoint point in visiblePoints)
            {
                if (point == null) continue;

                Vector3 dir = (point.position - botCameraTransform.position).normalized;
                float dist = Vector3.Distance(botCameraTransform.position, point.position);

                // outside range
                if (dist > viewDistance)
                {
                    continue;
                }

                // outside FOV
                if (Vector3.Dot(botCameraTransform.forward, dir) < Mathf.Cos(botHorizontalFOV * 0.5f * Mathf.Deg2Rad))
                {
                    continue;
                }

                point.isChecked = true;
            }
        }

        void CheckVisibleTacticalPoints(List<Transform> tpList, float range, float fov, LayerMask obstacleMask)
        {
            if (tpList == null || tpList.Count == 0) return;

            Transform eye = botRoot.PlayerCamera.GetPlayerCameraTarget();
            float fovCos = Mathf.Cos(fov * 0.5f * Mathf.Deg2Rad);

            // Duyệt ngược từ cuối danh sách lên đầu để xóa an toàn
            for (int i = tpList.Count - 1; i >= 0; i--)
            {
                if (tpList[i] == null)
                {
                    tpList.RemoveAt(i);
                    continue;
                }

                if (IsLocationVisible(tpList[i].position, eye.position, eye.forward, range, fovCos, obstacleMask))
                {
                    // Nếu nhìn thấy, loại bỏ khỏi danh sách ngay lập tức
                    tpList.RemoveAt(i);
                }
            }
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

            if (!lastKnownData.IsValid()) return;

            Vector3 center = lastKnownData.Position;
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

        bool IsLocationVisible(Vector3 targetPos, Vector3 eyePos, Vector3 forward, float range, float fovCos, LayerMask mask)
        {
            Vector3 dir = targetPos - eyePos;
            float dist = dir.magnitude;

            if (dist > range) return false; // Lọc khoảng cách

            dir = dir.normalized;

            // Lọc FOV
            if (Vector3.Dot(forward, dir) < fovCos) return false;

            // Lọc vật cản
            if (Physics.Raycast(eyePos, dir, dist, mask)) return false;

            return true;
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

            // Vẽ đường tới các tacticle point
            DrawLinesToTPs();

            // DrawSearchSamplingGizmos();
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

        void DrawLinesToTPs()
        {
            if (currentSearchPath == null || currentSearchPath.Count <= 0) return;

            Gizmos.color = botToTPLineColor;
            foreach (var tf in currentSearchPath)
            {
                Gizmos.DrawLine(
                    botRoot.PlayerCamera.GetPlayerCameraTarget().position,
                    tf.position
                );
            }
        }

        // void DrawSearchSamplingGizmos()
        // {
        //     // Điểm lý thuyết (màu vàng)
        //     Gizmos.color = Color.yellow;
        //     foreach (var p in theoreticalSamplePoints)
        //     {
        //         Gizmos.DrawSphere(p, 0.12f);
        //     }

        //     // Điểm NavMesh thật (màu xanh lá)
        //     Gizmos.color = Color.green;
        //     foreach (var p in navMeshSamplePoints)
        //     {
        //         Gizmos.DrawSphere(p, 0.18f);
        //     }

        //     if (!lastKnownData.IsValid()) return;

        //     // Nối LKPP → điểm NavMesh
        //     Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
        //     foreach (var p in navMeshSamplePoints)
        //     {
        //         Gizmos.DrawLine(lastKnownData.Position, p);
        //     }
        // }
    }
}

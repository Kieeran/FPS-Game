using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AIBot;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ScanRange
{
    public Vector3 leftDir;
    public Vector3 rightDir;
    public float angleRange;
}

public class BotTactics : MonoBehaviour
{
    [Header("Search Settings")]
    [SerializeField] float searchRadius = 20f; // Bán kính tìm kiếm quanh LKP

    // [Header("Weights")]
    // [Range(0, 1)][SerializeField] float directionWeight = 0.6f; // Độ quan trọng của hướng chạy
    // [Range(0, 1)][SerializeField] float distanceWeight = 0.4f;  // Độ quan trọng của khoảng cách từ LKP

    // Cấu trúc bổ trợ để lưu điểm số
    public struct ScoredPoint
    {
        public Transform point;
        public float score;
    }

    [Header("Debug Gizmos")]
    [SerializeField] bool showDebugGizmos = true;
    [SerializeField] Color debugColor = Color.yellow;

    // Biến lưu tạm để vẽ Gizmos
    private Vector3 lastDebugLKP;
    List<Transform> lastCandidates = new();
    public List<Transform> currentSearchPath { get; private set; } = new();

    [SerializeField] BotController botController;

    // Current master point list
    public List<InfoPoint> currentInfoPointsToScan = new();
    public InfoPoint currentInfoPoint = new();
    public ScanRange currentScanRange = new();
    public List<InfoPoint> currentVisiblePoint = new();

    public ZoneData currentTargetZoneData;

    public event Action OnCurrentVisiblePointsCompleted;
    public event Action OnZoneFullyScanned;

    public bool isCurrentVisiblePointsCompleted { get; private set; } = false;
    public bool isZoneFullyScanned { get; private set; } = false;
    public bool canScan { get; private set; } = false;

    void ClearZoneScanningData()
    {
        currentInfoPointsToScan.Clear();
        currentVisiblePoint.Clear();
        currentInfoPoint = null;
        currentScanRange = null;

        canScan = false;
        isZoneFullyScanned = false;
    }

    public void InitializeZoneScanning(List<InfoPoint> infoPoints, InfoPoint point)
    {
        ClearZoneScanningData();
        currentInfoPointsToScan = infoPoints;

        SetupNextScanSession(point);
    }

    public void SetupNextScanSession(InfoPoint point)
    {
        if (point != null)
        {
            currentInfoPoint = point;
        }
        else
        {
            currentInfoPoint = GetBestPoint();
        }
        currentInfoPoint.isChecked = true;

        CalculateCurrentScanRange();
        isCurrentVisiblePointsCompleted = false;
    }

    InfoPoint GetBestPoint()
    {
        List<InfoPoint> leftOverPoints = new();
        foreach (var point in currentInfoPointsToScan)
        {
            if (!point.isChecked) leftOverPoints.Add(point);
        }
        if (leftOverPoints.Count == 0) return null;

        InfoPoint bestPoint = leftOverPoints[0];
        for (int i = 1; i < leftOverPoints.Count; i++)
        {
            if (leftOverPoints[i].priority > bestPoint.priority)
            {
                bestPoint = leftOverPoints[i];
            }
        }
        return bestPoint;
    }

    public void CalculateCurrentVisiblePoint()
    {
        currentVisiblePoint.Clear();
        for (int i = 0; i < currentInfoPoint.visibleIndices.Count; i++)
        {
            int pointIndexInList = currentInfoPoint.visibleIndices[i];
            currentVisiblePoint.Add(currentInfoPointsToScan[pointIndexInList]);
        }
        canScan = true;
    }

    void CalculateCurrentScanRange()
    {
        var indices = new List<int>(currentInfoPoint.visibleIndices);
        if (indices == null || indices.Count == 0) return;

        List<InfoPoint> masterPoints = currentInfoPointsToScan;

        // Lọc điểm đã check
        for (int i = indices.Count - 1; i >= 0; i--)
        {
            if (masterPoints[indices[i]].isChecked)
            {
                indices.RemoveAt(i);
            }
        }
        if (indices.Count == 0) return;

        // Tính góc tuyệt đối của tất cả các điểm
        List<float> angles = new();
        for (int i = 0; i < indices.Count; i++)
        {
            Vector3 targetPos = masterPoints[indices[i]].position;
            Vector3 dir = (targetPos - currentInfoPoint.position).normalized;
            float yaw = Quaternion.LookRotation(dir).eulerAngles.y;
            angles.Add(yaw);
        }

        angles.Sort();

        // Tìm khoảng trống lớn nhất giữa các điểm liên tiếp
        float maxGap = 0;
        int gapStartIndex = 0;

        for (int i = 0; i < angles.Count; i++)
        {
            int nextIndex = (i + 1) % angles.Count;
            float gap = angles[nextIndex] - angles[i];

            // Xử lý wrap around
            if (gap < 0) gap += 360f;

            if (gap > maxGap)
            {
                maxGap = gap;
                gapStartIndex = nextIndex;
            }
        }

        // Tính góc cần quét (phần còn lại sau khi trừ khoảng trống)
        float scanAngle = 360f - maxGap;

        // ===== LOGIC ĐÚNG: LUÔN QUÉT VÙNG CÓ CHỨA CÁC ĐIỂM =====
        // Sau khi tìm maxGap, vùng cần quét là phần BÊN KIA của gap
        // leftYaw = điểm đầu tiên sau gap (góc nhỏ nhất trong vùng có điểm)
        // rightYaw = điểm cuối cùng trước gap (góc lớn nhất trong vùng có điểm)

        float leftYaw = angles[gapStartIndex];
        int rightIndex = (gapStartIndex - 1 + angles.Count) % angles.Count;
        float rightYaw = angles[rightIndex];
        float finalAngleRange = 360f - maxGap; // Góc chứa TẤT CẢ các điểm

        currentScanRange = new ScanRange
        {
            leftDir = Quaternion.Euler(0, leftYaw, 0) * Vector3.forward,
            rightDir = Quaternion.Euler(0, rightYaw, 0) * Vector3.forward,
            angleRange = finalAngleRange
        };
    }

    public ZoneData PredictMostSuspiciousZone(TPointData lastKnownData)
    {
        if (!lastKnownData.IsValid())
        {
            Debug.LogWarning("[Predict] TPointData is null! Cannot predict.");
            return null;
        }

        Zone currentZone = ZoneManager.Instance.GetZoneAt(lastKnownData.Position);
        if (currentZone == null) return null;

        ZoneData currentZoneData = currentZone.zoneData;

        ZoneData targetZoneData = null;
        Vector3 playerLookDir = (lastKnownData.Rotation * Vector3.forward).normalized;
        float bestMatch = -0.5f;

        Debug.Log($"Analyzing {currentZoneData.portals.Count} portals in {currentZoneData.name}. Player Dir: {playerLookDir}");
        foreach (var portal in currentZoneData.portals)
        {
            Vector3 dirToPortal = (portal.position - lastKnownData.Position).normalized;
            float dot = Vector3.Dot(playerLookDir, dirToPortal);

            if (dot > bestMatch)
            {
                bestMatch = dot;
                targetZoneData = portal.GetOtherZone(currentZoneData);
            }
        }

        // Nếu sau vòng lặp mà bestMatch vẫn quá thấp, có thể người chơi chỉ đang đứng yên hoặc xoay vòng
        if (bestMatch < 0.2f) // Ví dụ: góc lệch quá 78 độ
        {
            Debug.Log($"Confidence low (Best Dot: {bestMatch}). Staying in current zone: {currentZoneData.name}");
            return currentZoneData; // Ở lại zone cũ để tìm kỹ hơn thay vì đoán bừa sang zone khác
        }

        Debug.Log($"Target identified: {targetZoneData.zoneID} with Match Score: {bestMatch}");
        return targetZoneData;
    }

    void Update()
    {
        if (!canScan) return;
        if (currentInfoPointsToScan == null || currentInfoPointsToScan.Count == 0) return;
        if (currentVisiblePoint == null || currentVisiblePoint.Count == 0) return;

        int isCheckedCount = 0;

        // Check tất cả các point ở zone hiện tại
        foreach (var point in currentInfoPointsToScan)
        {
            if (point.isChecked) isCheckedCount++;
        }
        if (isCheckedCount == currentInfoPointsToScan.Count)
        {
            if (!isZoneFullyScanned)
            {
                isZoneFullyScanned = true;
                canScan = false;

                OnZoneFullyScanned?.Invoke();
            }
            return;
        }

        // // Check ở các point nhìn thấy hiện tại
        isCheckedCount = 0;
        foreach (var point in currentVisiblePoint)
        {
            if (point.isChecked) isCheckedCount++;
        }
        if (isCheckedCount == currentVisiblePoint.Count)
        {
            if (!isCurrentVisiblePointsCompleted && !isZoneFullyScanned)
            {
                isCurrentVisiblePointsCompleted = true;
                canScan = false;

                OnCurrentVisiblePointsCompleted?.Invoke();
            }
            return;
        }
    }

    // public List<Transform> GetPointsAroundLKP(Vector3 lkp)
    // {
    //     List<Transform> candidatePoints = new List<Transform>();

    //     InGameManager instance = InGameManager.Instance;

    //     if (instance == null || instance.spawnInGameManager.GetTacticalPointsList() == null)
    //     {
    //         Debug.LogWarning("TacticalPointsList chưa được gán hoặc danh sách TP trống!");
    //         return candidatePoints;
    //     }

    //     foreach (Transform tp in instance.spawnInGameManager.GetTacticalPointsList())
    //     {
    //         if (tp == null) continue;

    //         float distance = Vector3.Distance(lkp, tp.position);

    //         // Chỉ lấy các điểm nằm trong bán kính cho phép
    //         if (distance <= searchRadius)
    //         {
    //             candidatePoints.Add(tp);
    //         }
    //     }

    //     // Lưu lại vị trí để Gizmos có thể vẽ
    //     lastDebugLKP = lkp;

    //     // Cập nhật danh sách cuối cùng để vẽ Line nối trong Gizmos
    //     lastCandidates = candidatePoints.OrderBy(p => Vector3.Distance(lkp, p.position)).ToList();
    //     return lastCandidates;
    // }

    // public List<Transform> GetRankedPoints(Vector3 lkp, Vector3 lkDir)
    // {
    //     List<ScoredPoint> scoredPoints = new List<ScoredPoint>();
    //     var candidates = GetPointsAroundLKP(lkp); // Hàm bạn đã có

    //     foreach (Transform tp in candidates)
    //     {
    //         // 1. Tính toán Direction Score (Dùng Dot Product)
    //         Vector3 dirToPoint = (tp.position - lkp).normalized;
    //         float dot = Vector3.Dot(dirToPoint, lkDir.normalized);
    //         // Chuẩn hóa dot từ [-1, 1] về [0, 1]
    //         float directionScore = Mathf.Clamp01((dot + 1f) / 2f);

    //         // 2. Tính toán Distance Score (Càng gần LKP điểm càng cao)
    //         float distToLKP = Vector3.Distance(tp.position, lkp);
    //         float distanceScore = 1f - Mathf.Clamp01(distToLKP / searchRadius);

    //         // 3. Tổng hợp điểm số có trọng số
    //         float finalScore = (directionScore * directionWeight) + (distanceScore * distanceWeight);

    //         scoredPoints.Add(new ScoredPoint { point = tp, score = finalScore });
    //     }

    //     // Sắp xếp giảm dần theo điểm số
    //     return scoredPoints.OrderByDescending(sp => sp.score).Select(sp => sp.point).ToList();
    // }

    // public void CalculateSearchPath(TPointData lastKnownData, Action<List<Transform>> onDoneCalculate)
    // {
    //     if (!lastKnownData.IsValid()) return;

    //     currentSearchPath.Clear();
    //     currentSearchPath = GetRankedPoints(
    //         lastKnownData.Position,
    //         lastKnownData.Rotation.eulerAngles
    //     );

    //     onDoneCalculate?.Invoke(currentSearchPath);
    // }

    // public Transform GetNextPoint()
    // {
    //     if (currentSearchPath.Count <= 0) return null;

    //     Transform point = currentSearchPath[0];
    //     currentSearchPath.RemoveAt(0);

    //     return point;
    // }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        DrawSearchRadius();
        DrawCurrentPath();
    }

    private void DrawSearchRadius()
    {
        if (lastDebugLKP == Vector3.zero) return;

        Gizmos.color = debugColor;
        // Vẽ vòng tròn bán kính tìm kiếm
        Gizmos.DrawWireSphere(lastDebugLKP, searchRadius);
    }

    private void DrawCurrentPath()
    {
        if (currentSearchPath == null || currentSearchPath.Count == 0) return;

        for (int i = 0; i < currentSearchPath.Count; i++)
        {
            if (currentSearchPath[i] == null) continue;

            // Vẽ khối cầu tại mỗi điểm TP
            Gizmos.DrawSphere(currentSearchPath[i].position, 0.3f);
        }
    }

    [Header("Debug")]
    public bool drawcCurrentVisiblePoint = false;
    public bool drawcCurrentInfoPointsToScan = false;
    public int remainingCurrentVisiblePoint = 0;
    public int remainingCurrentInfoPointsToScan = 0;

    private void OnDrawGizmosSelected()
    {
        if (currentVisiblePoint.Count > 0)
        {
            remainingCurrentVisiblePoint = 0;
            foreach (var point in currentVisiblePoint)
            {
                if (point.isChecked)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                    remainingCurrentVisiblePoint++;
                }

                if (drawcCurrentVisiblePoint)
                {
#if UNITY_EDITOR
                    Gizmos.DrawSphere(point.position, 0.2f);
                    Handles.Label(point.position + Vector3.up * 0.5f, point.priority.ToString());
#endif
                }
            }
        }

        if (currentInfoPointsToScan.Count > 0)
        {
            remainingCurrentInfoPointsToScan = 0;
            foreach (var point in currentInfoPointsToScan)
            {
                if (point.isChecked)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                    remainingCurrentInfoPointsToScan++;
                }

                if (drawcCurrentInfoPointsToScan)
                {
#if UNITY_EDITOR
                    Gizmos.DrawSphere(point.position, 0.2f);
                    Handles.Label(point.position + Vector3.up * 0.5f, point.priority.ToString());
#endif
                }
            }
        }
    }
}
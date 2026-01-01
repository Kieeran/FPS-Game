using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotTactics : MonoBehaviour
{
    [Header("Search Settings")]
    [SerializeField] float searchRadius = 20f; // Bán kính tìm kiếm quanh LKP

    [Header("Weights")]
    [Range(0, 1)][SerializeField] float directionWeight = 0.6f; // Độ quan trọng của hướng chạy
    [Range(0, 1)][SerializeField] float distanceWeight = 0.4f;  // Độ quan trọng của khoảng cách từ LKP

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

    public List<Transform> GetPointsAroundLKP(Vector3 lkp)
    {
        List<Transform> candidatePoints = new List<Transform>();

        InGameManager instance = InGameManager.Instance;

        if (instance == null || instance.spawnInGameManager.GetTacticalPointsList() == null)
        {
            Debug.LogWarning("TacticalPointsList chưa được gán hoặc danh sách TP trống!");
            return candidatePoints;
        }

        foreach (Transform tp in instance.spawnInGameManager.GetTacticalPointsList())
        {
            if (tp == null) continue;

            float distance = Vector3.Distance(lkp, tp.position);

            // Chỉ lấy các điểm nằm trong bán kính cho phép
            if (distance <= searchRadius)
            {
                candidatePoints.Add(tp);
            }
        }

        // Lưu lại vị trí để Gizmos có thể vẽ
        lastDebugLKP = lkp;

        // Cập nhật danh sách cuối cùng để vẽ Line nối trong Gizmos
        lastCandidates = candidatePoints.OrderBy(p => Vector3.Distance(lkp, p.position)).ToList();
        return lastCandidates;
    }

    public List<Transform> GetRankedPoints(Vector3 lkp, Vector3 lkDir)
    {
        List<ScoredPoint> scoredPoints = new List<ScoredPoint>();
        var candidates = GetPointsAroundLKP(lkp); // Hàm bạn đã có

        foreach (Transform tp in candidates)
        {
            // 1. Tính toán Direction Score (Dùng Dot Product)
            Vector3 dirToPoint = (tp.position - lkp).normalized;
            float dot = Vector3.Dot(dirToPoint, lkDir.normalized);
            // Chuẩn hóa dot từ [-1, 1] về [0, 1]
            float directionScore = Mathf.Clamp01((dot + 1f) / 2f);

            // 2. Tính toán Distance Score (Càng gần LKP điểm càng cao)
            float distToLKP = Vector3.Distance(tp.position, lkp);
            float distanceScore = 1f - Mathf.Clamp01(distToLKP / searchRadius);

            // 3. Tổng hợp điểm số có trọng số
            float finalScore = (directionScore * directionWeight) + (distanceScore * distanceWeight);

            scoredPoints.Add(new ScoredPoint { point = tp, score = finalScore });
        }

        // Sắp xếp giảm dần theo điểm số
        return scoredPoints.OrderByDescending(sp => sp.score).Select(sp => sp.point).ToList();
    }

    public void CalculateSearchPath(TPointData lastKnownData, Action<List<Transform>> onDoneCalculate)
    {
        if (!lastKnownData.IsValid()) return;

        currentSearchPath.Clear();
        currentSearchPath = GetRankedPoints(
            lastKnownData.Position,
            lastKnownData.Rotation.eulerAngles
        );

        onDoneCalculate?.Invoke(currentSearchPath);
    }

    public Transform GetNextPoint()
    {
        if (currentSearchPath.Count <= 0) return null;

        Transform point = currentSearchPath[0];
        currentSearchPath.RemoveAt(0);

        return point;
    }

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
}
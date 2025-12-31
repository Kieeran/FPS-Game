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
    Queue<Transform> currentSearchPath = new();

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

    public List<Transform> GetRankedPoints(Vector3 lkp, Vector3 lkDir, Vector3 botPos)
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

    public void CalculateSearchPath(TPointData lastKnownData)
    {
        if (!lastKnownData.IsValid()) return;

        var rankedPoints = GetRankedPoints(
            lastKnownData.Position,
            lastKnownData.Rotation.eulerAngles,
            transform.position
        );

        currentSearchPath = new Queue<Transform>(rankedPoints);
    }

    public Transform GetNextPoint()
    {
        return currentSearchPath.Count > 0 ? currentSearchPath.Dequeue() : null;
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || lastDebugLKP == Vector3.zero) return;

        Gizmos.color = debugColor;

        // 1. Vẽ vòng tròn bán kính tìm kiếm tại LKP (Dùng WireSphere hoặc vẽ vòng tròn phẳng)
        Gizmos.DrawWireSphere(lastDebugLKP, searchRadius);

        // 2. Vẽ một biểu tượng nhỏ tại tâm LKP
        Gizmos.DrawIcon(lastDebugLKP + Vector3.up * 1f, "SearchIcon", true);

        // 3. Vẽ đường nối từ LKP đến các điểm tìm được để dễ quan sát
        if (lastCandidates != null)
        {
            foreach (var tp in lastCandidates)
            {
                if (tp == null) continue;

                // Vẽ đường line mờ nối đến các điểm trong danh sách
                Gizmos.color = new Color(debugColor.r, debugColor.g, debugColor.b, 0.3f);
                Gizmos.DrawLine(lastDebugLKP, tp.position);

                // Vẽ sphere nhỏ tại các điểm được chọn
                Gizmos.DrawSphere(tp.position, 0.3f);
            }
        }
    }
}
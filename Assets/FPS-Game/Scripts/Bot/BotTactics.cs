using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotTactics : MonoBehaviour
{
    [Header("Search Settings")]
    [SerializeField] float searchRadius = 20f; // Bán kính tìm kiếm quanh LKP

    [Header("Debug Gizmos")]
    [SerializeField] bool showDebugGizmos = true;
    [SerializeField] Color debugColor = Color.yellow;

    // Biến lưu tạm để vẽ Gizmos
    private Vector3 lastDebugLKP;
    private List<Transform> lastCandidates = new List<Transform>();

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
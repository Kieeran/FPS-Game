using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }
    public List<Zone> allZones = new();
    public List<PortalPoint> allPortals = new();
    public float heightOffset = 2.84f;

    public LayerMask obstacleLayer;
    [SerializeField] LayerMask zoneLayer;
    [SerializeField] Transform zoneContainer;

    public Dictionary<ZoneID, Zone> zoneCache;

    Dictionary<PortalPoint, List<(PortalPoint v, float w)>> adj;

    // Chạy cả Edit Mode & Play Mode
    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
#if UNITY_EDITOR
        else if (Instance != this)
        {
            Debug.LogWarning(
                "Multiple ZoneManager detected in scene. Only one should exist.",
                this);
        }
#endif

        BuildZoneCache();
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    // Runtime-only logic
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        adj = CalculateAdjacencyList();
    }

    void OnValidate()
    {
        if (zoneContainer != null)
        {
            allZones.Clear();
            allZones = zoneContainer.GetComponentsInChildren<Zone>(true).ToList();

            Dictionary<string, PortalPoint> portalPointDict = new();
            foreach (Zone zone in allZones)
            {
                foreach (PortalPoint portal in zone.zoneData.portals)
                {
                    if (portalPointDict.ContainsKey(portal.portalName)) continue;
                    portalPointDict[portal.portalName] = portal;
                }
            }
            allPortals.Clear();
            foreach (PortalPoint point in portalPointDict.Values)
            {
                allPortals.Add(point);
            }
        }
    }

    private Zone GetHighestWeighZone()
    {
        if (allZones == null || allZones.Count == 0)
        {
            Debug.Log("Unvalid zones list");
            return null;
        }

        Zone bestZone = allZones[0];
        foreach (Zone zone in allZones)
        {
            if (zone.GetCurrentWeight() > bestZone.GetCurrentWeight())
            {
                bestZone = zone;
            }
        }
        Debug.Log($"Bot patrol to zone: {bestZone.zoneData.zoneID}");
        bestZone.zoneData.ResetIsChecked();
        return bestZone;
    }

    public Zone GetZoneByID(ZoneID zoneID)
    {
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData.zoneID == zoneID) return zone;
        }

        return null;
    }

    public PortalPoint GetPortalPointByName(string name)
    {
        if (name == "") return null;
        foreach (PortalPoint point in allPortals)
        {
            if (point.portalName == name) return point;
        }
        return null;
    }

    public Zone GetZoneAt(Transform pointTF)
    {
        // Lấy bán kính từ SphereCollider của Object hoặc lấy scale
        float radius = 1.0f;
        if (pointTF.TryGetComponent<SphereCollider>(out var sphereCol))
        {
            radius = sphereCol.radius * pointTF.transform.lossyScale.x;
        }

        Collider[] hitColliders = Physics.OverlapSphere(pointTF.transform.position, radius, zoneLayer);
        if (hitColliders.Length == 0) return null;

        Zone zone = hitColliders[0].GetComponentInParent<Zone>();
        return zone;
    }

    public Zone GetZoneAt(Vector3 pointPos)
    {
        if (allZones == null || allZones.Count == 0) return null;
        foreach (Zone zone in allZones)
        {
            foreach (var col in zone.colliders)
            {
                // Kiểm tra nếu vị trí điểm xét nằm trong vùng của Collider
                if (col.bounds.Contains(pointPos))
                {
                    return zone;
                }
            }
        }
        return null;
    }

    public void BuildZoneCache()
    {
        zoneCache = new();
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData != null && zone.zoneData.zoneID != ZoneID.None)
            {
                if (!zoneCache.ContainsKey(zone.zoneData.zoneID))
                    zoneCache.Add(zone.zoneData.zoneID, zone);
            }
        }
    }

    public Vector3 GetSnappedPos(Vector3 originalPos)
    {
        if (NavMesh.SamplePosition(originalPos, out NavMeshHit hit, 4.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        Debug.Log("Không snap được");
        return originalPos;
    }

    [ContextMenu("Bake Visibility And Priority of InfoPoints")]
    public void BakeInfoPointVisibility()
    {
        Debug.Log("Bắt đầu quá trình Bake Visibility cho tất cả các Zone");

        int totalZones = allZones.Count;
        int totalPointsProcessed = 0;
        int totalConnectionsFound = 0;

        foreach (Zone zone in allZones)
        {
            if (zone.zoneData == null) continue;

            List<InfoPoint> masterPoints = new(zone.zoneData.masterPoints);
            int pointCount = masterPoints.Count;
            int connectionsInZone = 0;

            // Log bắt đầu xử lý Zone
            Debug.Log($"Đang xử lý Zone: {zone.name} ({pointCount} points)");

            for (int i = 0; i < masterPoints.Count; i++)
            {
                Vector3 startPos = masterPoints[i].position;
                List<int> visibleIndices = new();

                for (int j = 0; j < masterPoints.Count; j++)
                {
                    if (i == j) continue; // Không tự chiếu chính mình

                    Vector3 endPos = masterPoints[j].position;

                    // Bắn tia Linecast để kiểm tra vật cản
                    if (!Physics.Linecast(startPos, endPos, obstacleLayer))
                    {
                        // Nếu không có vật cản, thêm index j vào danh sách nhìn thấy của i
                        visibleIndices.Add(j);
                        connectionsInZone++;
                    }
                }

                masterPoints[i].visibleIndices.Clear();
                masterPoints[i].visibleIndices = visibleIndices;
                masterPoints[i].priority = visibleIndices.Count;
                totalPointsProcessed++;
            }

            totalConnectionsFound += connectionsInZone;
            // Log kết quả của từng Zone
            Debug.Log($"Done Zone {zone.name}: Tìm thấy {connectionsInZone} đường nhìn thấy.");

#if UNITY_EDITOR
            EditorUtility.SetDirty(zone.zoneData);
#endif
        }
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif

        Debug.Log($"[BakeVisibility] HOÀN TẤT!");
        Debug.Log($"Tổng cộng: {totalZones} Zones, {totalPointsProcessed} Points, {totalConnectionsFound} Connections.");
    }

    [ContextMenu("Bake All Portal Connection Traversal Cost")]
    public void BakeAllPortalConnectionTraversalCost()
    {
        int successCount = 0;
        NavMeshPath path = new();

        foreach (Zone zone in allZones)
        {
            if (zone.zoneData == null) continue;
            zone.zoneData.internalPaths.Clear();

            List<PortalPoint> portals = zone.zoneData.portals;

            for (int i = 0; i < portals.Count; i++)
            {
                for (int j = i + 1; j < portals.Count; j++) // Chỉ xét các portal phía sau i
                {
                    PortalPoint pA = portals[i];
                    PortalPoint pB = portals[j];

                    float dist = GetNavMeshDistance(GetSnappedPos(pA.position), GetSnappedPos(pB.position), path);

                    if (dist < float.PositiveInfinity && dist > 0)
                    {
                        zone.zoneData.internalPaths.Add(new PortalConnection
                        {
                            portalAID = pA.pointID,
                            portalBID = pB.pointID,
                            portalA = pA,
                            portalB = pB,
                            traversalCost = dist
                        });
                        successCount++;
                    }
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(zone.zoneData);
#endif
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        Debug.Log($"Bake hoàn tất! Đã cập nhật Portal Connection Traversal Cost cho {successCount} connection.");
    }

    [ContextMenu("Reset All Zone (Be careful !!!)")]
    public void ResetAllZone()
    {
        foreach (var zone in allZones)
        {
            zone.zoneData.HardResetZone();
        }
    }

    //     [ContextMenu("Bake All Portal Traversal Cost")]
    //     public void BakeAllPortalTraversalCost()
    //     {
    //         int successCount = 0;
    //         NavMeshPath path = new();

    //         foreach (Zone zone in allZones)
    //         {
    //             if (zone.zoneData == null) continue;

    //             foreach (PortalPoint portal in zone.zoneData.portals)
    //             {
    //                 ZoneData zoneA = portal.zoneDataA;
    //                 ZoneData zoneB = portal.zoneDataB;
    //                 if (zoneA == null || zoneB == null) continue;

    //                 float dist1 = GetNavMeshDistance(GetSnappedPos(zoneA.centerPos), GetSnappedPos(portal.position), path);
    //                 float dist2 = GetNavMeshDistance(GetSnappedPos(portal.position), GetSnappedPos(zoneB.centerPos), path);

    //                 // Lưu tổng quãng đường thực tế
    //                 portal.traversalCost = dist1 + dist2;
    //                 successCount++;
    //             }
    // #if UNITY_EDITOR
    //             EditorUtility.SetDirty(zone.zoneData);
    // #endif
    //         }

    // #if UNITY_EDITOR
    //         AssetDatabase.SaveAssets();
    // #endif
    //         Debug.Log($"Bake hoàn tất! Đã cập nhật NavMesh Distance cho {successCount} portal.");
    //     }

    // Tính chiều dài đường đi NavMesh
    private float GetNavMeshDistance(Vector3 start, Vector3 end, NavMeshPath path)
    {
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            float distance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return distance;
        }

        Debug.LogWarning($"Không tìm thấy đường NavMesh giữa {start} và {end}.");
        return 0;
    }

    public Transform currentBotTransform;
    // public ZoneData startZone;
    public ZoneData targetZone;
    // List<ZoneData> route = new();
    // public List<AdjRow> adjList = new();

    // [System.Serializable]
    // public class AdjRow // Đại diện cho một dòng trong danh sách kề
    // {
    //     public string portalName; // Tên portal gốc để dễ nhìn
    //     public List<PortalPoint> neighbors = new(); // Các portal có thể đi tới
    // }

    // [ContextMenu("Get Shortest Path")]
    // public void CalculateShortestPath()
    // {
    //     route = GetShortestPath(startZone, targetZone);

    //     if (route == null || route.Count == 0)
    //         Debug.LogWarning("Không tìm thấy lộ trình!");
    //     else
    //         Debug.Log($"Đã tìm thấy lộ trình qua {route.Count} Zone.");
    // }

    public List<PortalPoint> portalPointPath = new();
    //     [ContextMenu("Bake Path")]
    //     public void BakePath()
    //     {
    //         portalPointPath = CalculatePath(currentBotTransform.position, targetZone);

    // #if UNITY_EDITOR
    //         EditorUtility.SetDirty(this);
    // #endif
    //     }

    public List<PortalPoint> CalculatePath(Vector3 botPosition, ZoneData currentZoneData)
    {
        (ZoneData currentZone, ZoneData targetZone) = InitializeDijkstraData(botPosition, currentZoneData);
        Debug.Log($"Bắt đầu tính toán lộ trình tới Zone: {targetZone.zoneID}");
        // Khởi tạo danh sách đích (Targets)
        List<PortalPoint> targets = new();
        foreach (PortalPoint point in targetZone.portals)
        {
            PortalPoint p = GetPortalPointByName(point.portalName);
            if (p != null)
            {
                targets.Add(p);
            }
        }
        return Dijkstra(botPosition, currentZone, targets, adj);
    }

    (ZoneData currentZone, ZoneData targetZone) InitializeDijkstraData(Vector3 botPosition, ZoneData currentZoneData)
    {
        Zone currentZone = GetZoneAt(botPosition);
        if (currentZone == null)
        {
            currentZone = GetZoneByID(currentZoneData.zoneID);
        }
        Zone bestZone = FindBestZone(currentZone);

        return (currentZone.zoneData, bestZone.zoneData);
    }

    Zone FindBestZone(Zone currentZone)
    {
        Zone bestZone;
        while (true)
        {
            bestZone = GetHighestWeighZone();
            bestZone.ResetWeight();
            // Nếu best zone là zone đang đứng thì reset weigh và bỏ qua
            if (bestZone == currentZone)
            {
                continue;
            }

            foreach (var portal in bestZone.zoneData.portals)
            {
                // Nếu best zone là zone liền kề với zone đang đứng thì reset weigh và bỏ qua
                if (portal.zoneDataA == currentZone.zoneData || portal.zoneDataB == currentZone.zoneData)
                {
                    continue;
                }
            }
            break;
        }
        return bestZone;
    }

    Dictionary<PortalPoint, List<(PortalPoint v, float w)>> CalculateAdjacencyList()
    {
        // Xây dựng danh sách kề (Adjacency List - adj)
        // Coi PortalPoint là Key, danh sách các Portal hàng xóm và khoảng cách là Value
        var adj = new Dictionary<PortalPoint, List<(PortalPoint v, float w)>>();
        int connectionCount = 0;
        foreach (Zone zone in allZones)
        {
            foreach (var conn in zone.zoneData.internalPaths)
            {
                PortalPoint a = GetPortalPointByName(conn.portalA.portalName);
                PortalPoint b = GetPortalPointByName(conn.portalB.portalName);

                if (a != null && b != null)
                {
                    AddEdge(adj, a, b, conn.traversalCost);
                    AddEdge(adj, b, a, conn.traversalCost);
                    connectionCount++;
                }
            }
        }
        Debug.Log($"Đã dựng đồ thị với {connectionCount} kết nối nội bộ.");

        return adj;
    }

    // List<PortalPoint> CalculatePath(ZoneData currentZoneData, ZoneData targetZone)
    // {
    //     Debug.Log($"Bắt đầu tính toán lộ trình tới Zone: {targetZone.zoneID}");
    //     // Khởi tạo danh sách đích (Targets)
    //     List<PortalPoint> targets = new();
    //     foreach (PortalPoint point in targetZone.portals)
    //     {
    //         PortalPoint p = GetPortalPointByName(point.portalName);
    //         if (p != null)
    //         {
    //             targets.Add(p);
    //         }
    //     }

    //     // Xây dựng danh sách kề (Adjacency List - adj)
    //     // Coi PortalName là Key, danh sách các Portal hàng xóm và khoảng cách là Value
    //     var adj = new Dictionary<PortalPoint, List<(PortalPoint v, float w)>>();

    //     int connectionCount = 0;
    //     foreach (Zone zone in allZones)
    //     {
    //         foreach (var conn in zone.zoneData.internalPaths)
    //         {
    //             PortalPoint a = GetPortalPointByName(conn.portalA.portalName);
    //             PortalPoint b = GetPortalPointByName(conn.portalB.portalName);

    //             if (a != null && b != null)
    //             {
    //                 AddEdge(adj, a, b, conn.traversalCost);
    //                 AddEdge(adj, b, a, conn.traversalCost);
    //                 connectionCount++;
    //             }
    //         }
    //     }
    //     Debug.Log($"Đã dựng đồ thị với {connectionCount} kết nối nội bộ.");

    //     // Gọi lõi thuật toán Dijkstra
    //     return Dijkstra(botPosition, currentZoneData, targets, adj);
    // }

    private void AddEdge(Dictionary<PortalPoint, List<(PortalPoint v, float w)>> adj, PortalPoint from, PortalPoint to, float w)
    {
        if (!adj.ContainsKey(from))
        {
            // Debug.Log($"<color=cyan>Add key {from.portalName} to adj</color>");
            adj[from] = new List<(PortalPoint, float)>();
        }

        // Debug.Log($"Add value {to.portalName} to key <color=cyan>{from.portalName}</color> of adj");
        adj[from].Add((to, w));
    }

    // Chưa tính tới trường hợp: ở lần tìm đường sau, nếu trước đó bot đứng quá gần portal trong current zone,
    // khi add các portal vào danh sách (bao gồm portal gần bot) thì sau khi tính toán route, portal gần bot đó sẽ nằm đầu danh sách
    // và bot phải đi tới đó, dẫn đến việc hệ thống không kịp chuyển sang portal ở zone kế tiếp trong danh sách route.
    public List<PortalPoint> Dijkstra(Vector3 source, ZoneData currentZoneData, List<PortalPoint> targets, Dictionary<PortalPoint, List<(PortalPoint v, float w)>> adj)
    {
        var pq = new List<(PortalPoint u, float d)>();
        var dists = new Dictionary<PortalPoint, float>();
        var prevs = new Dictionary<PortalPoint, PortalPoint>();

        // Khởi tạo dist = vô cực
        foreach (PortalPoint point in allPortals)
        {
            dists[point] = float.MaxValue;
        }

        // Tìm các portal ở Zone hiện tại và nạp vào pq (Nguồn động)
        Zone zone;
        if (currentZoneData != null)
        {
            zone = GetZoneByID(currentZoneData.zoneID);
        }
        else
        {
            zone = GetZoneAt(source);
            if (zone == null)
            {
                Debug.LogError("Không tìm thấy Zone tại vị trí Bot!");
                return null;
            }
        }

        // Thêm các node hiện có ở source vào pq
        foreach (var pRaw in zone.zoneData.portals)
        {
            PortalPoint pUnique = GetPortalPointByName(pRaw.portalName);
            if (pUnique == null) continue;

            float d = GetNavMeshDistance(GetSnappedPos(source), GetSnappedPos(pUnique.position), new NavMeshPath());
            dists[pUnique] = d;
            pq.Add((pUnique, d));
            Debug.Log($"Nạp portal nguồn: {pUnique.portalName}, Khoảng cách: {d}");
        }

        PortalPoint finalNode = null;
        int steps = 0;

        while (pq.Count > 0)
        {
            steps++;
            pq.Sort((a, b) => a.d.CompareTo(b.d));
            var current = pq[0];
            pq.RemoveAt(0);

            PortalPoint u = current.u;
            float d = current.d;

            if (d > dists[u]) continue;

            // Log bước nhảy
            // Debug.Log($"[Step {steps}] Đang xét Portal: {u.portalName} (Dist: {d})");

            // KIỂM TRA ĐÍCH (Nếu targets chứa u)
            if (targets.Any(t => t.portalName == u.portalName))
            {
                finalNode = u;
                Debug.Log($"<color=green>Đã tìm thấy đích tại {u.portalName}</color>");
                break;
            }

            // Lặp qua danh sách kề (adj)
            if (adj.ContainsKey(u))
            {
                foreach (var edge in adj[u])
                {
                    // Đảm bảo hàng xóm cũng là bản Unique
                    PortalPoint vUnique = GetPortalPointByName(edge.v.portalName);
                    if (vUnique == null) continue;

                    float newDist = d + edge.w;
                    if (newDist < dists[vUnique])
                    {
                        dists[vUnique] = newDist;
                        prevs[vUnique] = u;
                        pq.Add((vUnique, newDist));
                    }
                }
            }
        }
        if (finalNode == null) Debug.LogWarning("Không tìm thấy đường tới Target Zone!");

        // Truy vấn ngược
        return ReconstructPath_V2(prevs, finalNode);
    }

    private List<PortalPoint> ReconstructPath_V2(Dictionary<PortalPoint, PortalPoint> prevs, PortalPoint finalPortal)
    {
        if (finalPortal == null) return new List<PortalPoint>();

        List<PortalPoint> path = new();
        PortalPoint curr = finalPortal;

        // Sử dụng HashSet để chống vòng lặp vô tận (Safety check)
        HashSet<PortalPoint> visited = new();

        while (curr != null && !visited.Contains(curr))
        {
            path.Add(curr);
            visited.Add(curr);

            if (prevs.ContainsKey(curr))
                curr = prevs[curr];
            else
                curr = null;
        }

        path.Reverse();
        return path;
    }

    // public List<ZoneData> GetShortestPath(ZoneData startZone, ZoneData targetZone)
    // {
    //     if (startZone == null || targetZone == null) return new List<ZoneData>();

    //     Dictionary<ZoneData, float> dists = new();
    //     Dictionary<ZoneData, ZoneData> prevs = new();
    //     List<(ZoneData zone, float d)> pq = new();

    //     // Khởi tạo
    //     foreach (var z in allZones)
    //     {
    //         dists[z.zoneData] = float.MaxValue;
    //         prevs[z.zoneData] = null;
    //     }

    //     dists[startZone] = 0;
    //     pq.Add((startZone, 0));

    //     while (pq.Count > 0)
    //     {
    //         pq.Sort((a, b) => a.d.CompareTo(b.d));

    //         (ZoneData u, float d) = pq[0];
    //         pq.RemoveAt(0);

    //         if (d > dists[u]) continue;
    //         if (u == targetZone) break;

    //         foreach (var portal in u.portals)
    //         {
    //             ZoneData v = portal.GetOtherZone(u);
    //             float weight = portal.traversalCost; // traversalCost đã bake

    //             if (dists[u] + weight < dists[v])
    //             {
    //                 dists[v] = dists[u] + weight;
    //                 prevs[v] = u;
    //                 pq.Add((v, dists[v]));
    //             }
    //         }
    //     }

    //     return ReconstructPath(prevs, targetZone);
    // }

    // private List<ZoneData> ReconstructPath(Dictionary<ZoneData, ZoneData> prevs, ZoneData target)
    // {
    //     List<ZoneData> path = new();
    //     ZoneData curr = target;

    //     while (curr != null)
    //     {
    //         path.Add(curr);
    //         // Kiểm tra xem node hiện tại có nằm trong bảng truy vết không
    //         if (prevs.ContainsKey(curr))
    //             curr = prevs[curr];
    //         else
    //             break;
    //     }

    //     path.Reverse();
    //     return path;
    // }

    // Hàm phụ trợ tìm Portal nối giữa 2 Zone
    private PortalPoint GetPortalBetween(ZoneData a, ZoneData b)
    {
        foreach (var p in a.portals)
        {
            if (p.GetOtherZone(a) == b) return p;
        }

        Debug.Log($"Không tìm thấy portal nối giữa {a.zoneID} và {b.zoneID}");
        return null;
    }

    // public bool showResultRoute = true;
    // public bool showNavigationGraphBetweenCenterPos = true;
    public bool showConnectionGraphBetweenPortal = true;
    public bool showShortestPortalPointPath = true;

    private void OnDrawGizmos()
    {
        // if (showNavigationGraphBetweenCenterPos)
        // {
        //     DrawNavigationGraphBetweenCenterPos();
        // }

        // if (showResultRoute)
        // {
        //     DrawResultRoute();
        // }

        if (showConnectionGraphBetweenPortal)
        {
            DrawConnectionGraphBetweenPortal();
        }

        if (showShortestPortalPointPath)
        {
            DrawPath();
        }
    }

    // void DrawNavigationGraphBetweenCenterPos()
    // {
    //     NavMeshPath path = new();
    //     foreach (Zone zone in allZones)
    //     {
    //         if (zone.zoneData == null) continue;
    //         Vector3 startPos = zone.zoneData.centerPos;
    //         if (zone.zoneData.portals == null) continue;

    //         foreach (var portal in zone.zoneData.portals)
    //         {
    //             if (portal == null) continue;

    //             // Vẽ đường đi dưới đất từ Center -> Portal
    //             DrawNavMeshGizmoLine(GetSnappedPos(startPos), GetSnappedPos(portal.position), path, Color.green);
    //         }
    //     }
    // }

    void DrawConnectionGraphBetweenPortal()
    {
        NavMeshPath path = new();
        foreach (Zone zone in allZones)
        {
            if (zone.zoneData == null) continue;

            foreach (var connection in zone.zoneData.internalPaths)
            {
                if (connection == null) continue;

                // Vẽ đường đi dưới đất từ Portal -> Portal
                DrawNavMeshGizmoLine(
                    GetSnappedPos(connection.portalA.position),
                    GetSnappedPos(connection.portalB.position),
                    path,
                    Color.green
                );
            }
        }
    }

    // void DrawResultRoute()
    // {
    //     NavMeshPath path = new();
    //     if (route != null && route.Count >= 2)
    //     {
    //         for (int i = 0; i < route.Count - 1; i++)
    //         {
    //             ZoneData current = route[i];
    //             ZoneData next = route[i + 1];
    //             PortalPoint connector = GetPortalBetween(current, next);

    //             if (connector != null)
    //             {
    //                 DrawNavMeshGizmoLine(GetSnappedPos(current.centerPos), GetSnappedPos(connector.position), path, Color.green);
    //                 DrawNavMeshGizmoLine(GetSnappedPos(connector.position), GetSnappedPos(next.centerPos), path, Color.green);
    //             }
    //         }
    //     }
    // }

    void DrawPath()
    {
        NavMeshPath path = new();
        if (portalPointPath != null && portalPointPath.Count >= 2)
        {
            DrawNavMeshGizmoLine(GetSnappedPos(currentBotTransform.position), GetSnappedPos(portalPointPath[0].position), path, Color.green);
            for (int i = 0; i < portalPointPath.Count - 1; i++)
            {
                PortalPoint current = portalPointPath[i];
                PortalPoint next = portalPointPath[i + 1];

                DrawNavMeshGizmoLine(GetSnappedPos(current.position), GetSnappedPos(next.position), path, Color.green);
            }
        }
    }

    // Hàm phụ trợ vẽ đường gấp khúc dựa trên NavMesh Corners
    void DrawNavMeshGizmoLine(Vector3 start, Vector3 end, NavMeshPath path, Color color)
    {
        if (NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
        {
            Gizmos.color = color;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);

                // Vẽ các điểm nút trên đường đi để dễ quan sát
                Gizmos.DrawSphere(path.corners[i], 0.1f);
            }
        }
        else
        {
            // Nếu không có NavMesh, vẽ đường thẳng màu đỏ cảnh báo
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);
        }
    }
}
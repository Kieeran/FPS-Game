using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class SpawnInGameManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameManagerPrefab;

    void Start()
    {
        StartCoroutine(TrySpawnInGameManager());
    }

    private IEnumerator TrySpawnInGameManager()
    {
        // Chờ NetworkManager được khởi tạo và Netcode sẵn sàng
        yield return new WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening);

        // Chờ thêm 1 frame để chắc chắn player objects đã spawn
        yield return null;

        if (!NetworkManager.Singleton.IsServer)
            yield break; // Chỉ server mới spawn

        // Nếu đã có sẵn instance (do Netcode SceneObject hay ai đó spawn rồi) thì bỏ qua
        if (InGameManager.Instance != null)
        {
            Debug.Log("[GameSceneLoader] InGameManager đã tồn tại, bỏ qua spawn.");
            yield break;
        }

        // Kiểm tra prefab
        if (inGameManagerPrefab == null)
        {
            Debug.LogError("[GameSceneLoader] Prefab InGameManager chưa được gán!");
            yield break;
        }

        // Tiến hành spawn
        GameObject obj = Instantiate(inGameManagerPrefab);
        NetworkObject netObj = obj.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("[GameSceneLoader] Prefab InGameManager thiếu NetworkObject!");
            Destroy(obj);
            yield break;
        }

        try
        {
            netObj.Spawn();
            Debug.Log("[GameSceneLoader] Spawned InGameManager thành công!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameSceneLoader] Spawn thất bại: {ex.Message}");
            Destroy(obj);
        }
    }
}
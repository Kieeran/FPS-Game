using UnityEngine;

public class PlayerLook : PlayerBehaviour
{
    public Transform TargetLook;
    [SerializeField] float distance = 2f;
    Transform playerCameraTarget;

    void Start()
    {
        playerCameraTarget = PlayerRoot.PlayerCamera.GetPlayerCameraTarget();
    }

    void Update()
    {
        UpdateTargetLook();
    }

    void UpdateTargetLook()
    {
        if (TargetLook == null) return;

        Vector3 targetPosition;
        if (PlayerRoot.IsBot.Value)
        {
            targetPosition = playerCameraTarget.position + playerCameraTarget.forward * distance;
        }
        else
        {
            if (Camera.main == null) return;

            targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
        }
        TargetLook.position = targetPosition;
    }
}
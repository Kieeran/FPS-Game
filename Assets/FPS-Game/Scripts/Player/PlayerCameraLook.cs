using UnityEngine;

public class PlayerCameraLook : PlayerBehaviour
{
    [SerializeField] Transform _cameraPivot;
    bool _toggleCameraRotation = true;

    public override void InitializeAwake()
    {
        base.InitializeAwake();
        PlayerRoot.PlayerUI.ToggleEscapeUI += () =>
        {
            _toggleCameraRotation = !_toggleCameraRotation;
        };

        PlayerRoot.PlayerTakeDamage.OnPlayerDead += () =>
        {
            _toggleCameraRotation = false;
        };

        PlayerRoot.PlayerNetwork.OnPlayerRespawn += () =>
        {
            _toggleCameraRotation = true;
        };
    }

    void LateUpdate()
    {
        if (_toggleCameraRotation)
        {
            transform.position = _cameraPivot.transform.position;
            // transform.SetPositionAndRotation(_cameraPivot.transform.position, _cameraPivot.transform.rotation);
        }
    }
}

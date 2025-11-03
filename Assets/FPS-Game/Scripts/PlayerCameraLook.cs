using UnityEngine;

public class PlayerCameraLook : PlayerBehaviour
{
    // [SerializeField] Transform _cameraPivot;
    // bool _toggleCameraRotation = true;

    // public override void InitializeAwake()
    // {
    //     base.InitializeAwake();
    //     PlayerRoot.Events.ToggleEscapeUI += () =>
    //     {
    //         _toggleCameraRotation = !_toggleCameraRotation;
    //     };

    //     PlayerRoot.Events.OnPlayerDead += () =>
    //     {
    //         _toggleCameraRotation = false;
    //     };

    //     PlayerRoot.Events.OnPlayerRespawn += () =>
    //     {
    //         _toggleCameraRotation = true;
    //     };
    // }

    // void LateUpdate()
    // {
    //     if (_toggleCameraRotation)
    //     {
    //         transform.position = _cameraPivot.transform.position;
    //         // transform.SetPositionAndRotation(_cameraPivot.transform.position, _cameraPivot.transform.rotation);
    //     }
    // }
}

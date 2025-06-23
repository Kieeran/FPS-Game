using Unity.Netcode;
using UnityEngine;

public class SwayAndBob : NetworkBehaviour
{
    public PlayerRoot PlayerRoot;

    Vector2 _moveInput;
    Vector2 _lookInput;

    [Header("Sway")]
    public float Step = 0.1f;
    public float MaxStepDistance = 0.5f;
    Vector3 _swayPos;

    [Header("Sway Rotation")]
    public float RotationStep = 4f;
    public float MaxRotationStep = 5f;
    Vector3 _swayEulerRotation;

    [Header("Bobbing")]
    public float SpeedCurve;

    float _curveSin { get => Mathf.Sin(SpeedCurve); }
    float _curveCos { get => Mathf.Cos(SpeedCurve); }

    public Vector3 TravelLimit = Vector3.one * 0.025f;
    public Vector3 BobLimit = Vector3.one * 0.01f;

    Vector3 _bobPosition;

    float _smooth = 10f;
    float _smoothRotation = 12f;

    [Header("Bob Rotation")]
    public Vector3 Multiplier;
    Vector3 _bobEulerRotation;

    Vector3 _lastPosition;
    Vector3 _velocity;
    public float MaxVelocityMagnitude = 10f;

    public Vector3 AimPositionOffset;
    public Quaternion AimRotationOffset;

    void Start()
    {
        AimPositionOffset = Vector3.zero;
        AimRotationOffset = Quaternion.identity;
    }

    void Update()
    {
        if (IsOwner == false) return;
        if (PlayerRoot.PlayerUI.IsEscapeUIOn()) return;

        _moveInput = PlayerRoot.PlayerAssetsInputs.move;
        _lookInput = PlayerRoot.PlayerAssetsInputs.look;

        Sway();
        SwayRotation();

        _velocity = (PlayerRoot.transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = PlayerRoot.transform.position;

        if (_velocity.magnitude > MaxVelocityMagnitude)
            SpeedCurve += Time.deltaTime * MaxVelocityMagnitude * 1.5f + 0.01f;
        else
            SpeedCurve += Time.deltaTime * _velocity.magnitude * 1.5f + 0.01f;

        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    void Sway()
    {
        if (IsOwner == false) return;

        Vector3 invertLook = _lookInput * -Step;
        invertLook.x = Mathf.Clamp(invertLook.x, -MaxStepDistance, MaxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -MaxStepDistance, MaxStepDistance);

        _swayPos = invertLook;
    }

    void SwayRotation()
    {
        if (IsOwner == false) return;

        Vector2 invertLook = _lookInput * -RotationStep;

        invertLook.x = Mathf.Clamp(invertLook.x, -MaxRotationStep, MaxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -MaxRotationStep, MaxRotationStep);

        _swayEulerRotation = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void BobOffset()
    {
        _bobPosition.x = _curveCos * BobLimit.x;
        _bobPosition.y = _curveSin * BobLimit.y - _velocity.y * TravelLimit.y;
        _bobPosition.z = -_moveInput.y * TravelLimit.z;

        _bobPosition.x = _curveCos * BobLimit.x;
        _bobPosition.y = _curveSin * BobLimit.y;
    }

    void BobRotation()
    {
        _bobEulerRotation.x = _moveInput != Vector2.zero ? Multiplier.x * Mathf.Sin(2 * SpeedCurve) :
                                                         Multiplier.x * Mathf.Sin(2 * SpeedCurve) / 2;
        _bobEulerRotation.y = _moveInput != Vector2.zero ? Multiplier.y * _curveCos : 0;
        _bobEulerRotation.z = _moveInput != Vector2.zero ? Multiplier.z * _curveCos * _moveInput.x : 0;
    }

    void CompositePositionRotation()
    {
        // Tính target position với sway + bob + aim offset
        Vector3 targetPos = _swayPos + _bobPosition + AimPositionOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * _smooth);

        // Tính target rotation
        Quaternion targetRot = Quaternion.Euler(_swayEulerRotation) * AimRotationOffset;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * _smoothRotation);
    }
}
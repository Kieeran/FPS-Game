using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayAndBob : MonoBehaviour
{
    [Header("External references")]
    public Rigidbody rb;

    private Vector2 walkInput;
    private Vector2 lookInput;

    private void Start()
    {

    }

    private void Update()
    {
        //lookInput = PlayerInput.Instance.GetMousePos();
        //walkInput = PlayerInput.Instance.GetMoveInput();

        if (Input.GetMouseButton(1)) return;
        //if (Input.GetMouseButton(0)) return;

        Sway();
        SwayRotation();

        speedCurve += Time.deltaTime * rb.velocity.magnitude * 1.5f + 0.01f;

        BobOffset();
        //BobRotation();

        CompositePositionRotation();
    }

    [Header("Sway")]
    public float step = 0.1f;
    public float maxStepDistance = 0.5f;
    Vector3 swayPos;

    private void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRotation;

    private void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;

        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRotation = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    [Header("Bobbing")]
    public float speedCurve;

    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;

    Vector3 bobPosition;

    private void BobOffset()
    {
        bobPosition.x = curveCos * bobLimit.x;
        bobPosition.y = curveSin * bobLimit.y - rb.velocity.y * travelLimit.y;
        bobPosition.z = -walkInput.y * travelLimit.z;

        //bobPosition.x = curveCos * bobLimit.x;
        //bobPosition.y = curveSin * bobLimit.y;
    }

    //[Header("Bob Rotation")]
    //public Vector3 multiplier;
    //Vector3 bobEulerRotation;

    //private void BobRotation()
    //{
    //    bobEulerRotation.x = walkInput != Vector2.zero ? multiplier.x * Mathf.Sin(2 * speedCurve) :
    //                                                     multiplier.x * Mathf.Sin(2 * speedCurve) / 2;

    //    bobEulerRotation.y = walkInput != Vector2.zero ? multiplier.y * curveCos : 0;

    //    bobEulerRotation.z = walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0;

    //}

    float smooth = 10f;
    float smoothRotation = 12f;

    private void CompositePositionRotation()
    {
        transform.localPosition =
            Vector3.Lerp(transform.localPosition,
            swayPos + bobPosition,
            Time.deltaTime * smooth);

        transform.localRotation =
            Quaternion.Lerp(transform.localRotation,
            Quaternion.Euler(swayEulerRotation),
            Time.deltaTime * smoothRotation);
    }
}
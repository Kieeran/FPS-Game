using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
namespace CustomTask
{
    [TaskCategory("Custom")]
    public class CustomLookAt : Action
    {
        [Tooltip("Transform of the targetCamera")]
        [SerializeField] SharedTransform targetCamera;

        [Tooltip("Target to look at")]
        [SerializeField] SharedTransform target;

        [Tooltip("Output desired look euler angles (x=pitch, y=yaw, z=roll)")]
        [SerializeField] SharedVector3 lookEuler;

        [Tooltip("Clamp pitch (min, max)")]
        [SerializeField] Vector2 pitchClamp;

        public override TaskStatus OnUpdate()
        {
            if (targetCamera.Value == null || target.Value == null)
                return TaskStatus.Failure;

            Vector3 dir = target.Value.position - targetCamera.Value.position;
            if (dir.sqrMagnitude < 0.0001f)
                return TaskStatus.Failure;

            Vector3 norm = dir.normalized;

            // -------- YAW --------
            Vector3 yawDir = norm;
            if (yawDir.sqrMagnitude < 0.0001f)
                return TaskStatus.Failure;

            float yaw = Mathf.Atan2(yawDir.x, yawDir.z) * Mathf.Rad2Deg;

            // -------- PITCH --------
            // Negative because Unity pitch is inverted
            float pitch = -Mathf.Asin(norm.y) * Mathf.Rad2Deg;
            pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);

            // -------- ROLL --------
            float roll = lookEuler.Value.z;

            lookEuler.Value = new Vector3(pitch, yaw, roll);
            return TaskStatus.Success;
        }
    }
}
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class LookAround : Action
    {
        [SerializeField] float topPitch;
        [SerializeField] float bottomPitch;
        [SerializeField] float speed;
        [SerializeField] SharedFloat targetPitch;

        string mode = "Up";
        bool endTask = false;

        public override void OnStart()
        {
            base.OnStart();
            mode = "Up";
            endTask = false;
            targetPitch.Value = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            float pitch = targetPitch.Value;

            if (mode == "Up")
            {
                pitch = LookUp(pitch);

                if (HasReachedValue(pitch, topPitch))
                {
                    Debug.Log("Has reach topPitch");
                    pitch = topPitch;

                    StartCoroutine(WaitThenChangeToMode("Normal"));
                }
            }
            else if (mode == "Normal")
            {
                pitch = BackToNormal(pitch);
                // Debug.Log(pitch);
                if (HasReachedValue(pitch, 0f))
                {
                    Debug.Log("Back to normal");
                    pitch = 0f;

                    if (endTask)
                    {
                        targetPitch.Value = pitch;
                        return TaskStatus.Success;
                    }

                    StartCoroutine(WaitThenChangeToMode("Down"));
                }
            }
            else if (mode == "Down")
            {
                pitch = LookDown(pitch);

                if (HasReachedValue(pitch, bottomPitch))
                {
                    Debug.Log("Has reach bottomPitch");
                    pitch = bottomPitch;

                    endTask = true;
                    StartCoroutine(WaitThenChangeToMode("Normal"));
                }
            }
            targetPitch.Value = pitch;

            return TaskStatus.Running;
        }

        IEnumerator WaitThenChangeToMode(string mode)
        {
            yield return new WaitForSeconds(0.5f);
            this.mode = mode;
        }

        bool HasReachedValue(float val, float targetVal)
        {
            return Mathf.Abs(val - targetVal) <= 0.1f;
        }

        float LookUp(float pitch)
        {
            pitch -= speed * Time.deltaTime;
            return Mathf.Max(pitch, topPitch);
        }

        float LookDown(float pitch)
        {
            pitch += speed * Time.deltaTime;
            return Mathf.Min(pitch, bottomPitch);
        }

        float BackToNormal(float pitch)
        {
            if (pitch < 0f)
                pitch += speed * Time.deltaTime;
            else if (pitch > 0f)
                pitch -= speed * Time.deltaTime;

            // Avoid jitter around 0
            if (pitch <= 0.6f && pitch >= -0.6f)
            {
                pitch = 0;
            }

            return pitch;
        }

        public override void OnReset()
        {
            base.OnReset();
            mode = "Up";
            endTask = false;
            targetPitch.Value = 0f;
        }
    }
}
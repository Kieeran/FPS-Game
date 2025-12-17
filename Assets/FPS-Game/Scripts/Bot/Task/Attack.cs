using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
namespace CustomTask
{
    [TaskCategory("Custom")]
    public class Attack : Action
    {
        [SerializeField] SharedBool attack;
        [SerializeField] float fireDuration;
        [SerializeField] float fireCooldown;

        float timer;
        bool isFiring;

        public override void OnStart()
        {
            timer = 0f;
            isFiring = false;
            attack.Value = false;
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;

            if (isFiring)
            {
                // Đang bật attack
                if (timer >= fireDuration)
                {
                    attack.Value = false;
                    isFiring = false;
                    timer = 0f;
                }
            }
            else
            {
                // Đang chờ cooldown
                if (timer >= fireCooldown)
                {
                    attack.Value = true;
                    isFiring = true;
                    timer = 0f;
                }
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            attack.Value = false;
        }

        public override void OnReset()
        {
            attack.Value = false;
        }
    }
}
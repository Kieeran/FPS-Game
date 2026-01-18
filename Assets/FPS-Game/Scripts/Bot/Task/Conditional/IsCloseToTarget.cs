using AIBot;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class IsCloseToTarget : Conditional
    {
        [Tooltip("The GameObject that the agent is seeking")]
        [SerializeField] SharedGameObject target;
        [Tooltip("If target is null then use the target position")]
        [SerializeField] SharedVector3 targetPosition;

        [SerializeField] float closeDistance;

        public override TaskStatus OnUpdate()
        {
            if (target != null && target.Value != null)
            {
                if (Vector3.Distance(target.Value.transform.position, transform.position) <= closeDistance)
                {
                    return TaskStatus.Success;
                }
                else return TaskStatus.Failure;
            }

            else
            {
                if (Vector3.Distance(targetPosition.Value, transform.position) <= closeDistance)
                {
                    return TaskStatus.Success;
                }
                else return TaskStatus.Failure;
            }
        }
    }
}
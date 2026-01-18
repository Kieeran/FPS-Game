using AIBot;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class HasTargetPortal : Conditional
    {
        [SerializeField] SharedInt portalLeft;

        public override TaskStatus OnUpdate()
        {
            return portalLeft.Value > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
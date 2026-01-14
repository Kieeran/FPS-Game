using AIBot;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class IsDataValid : Conditional
    {
        [SerializeField] SharedTPointData data;

        public override TaskStatus OnUpdate()
        {
            return data.Value.IsValid() ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
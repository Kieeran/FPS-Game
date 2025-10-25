// GetListItem.cs
// Place in: Assets/Scripts/BehaviorDesigner/Tasks/GetListItem.cs
// Requires: Behavior Designer runtime & tasks (BehaviorDesigner.Runtime, BehaviorDesigner.Runtime.Tasks)

using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using Unity.VisualScripting;

/// <summary>
/// GetListItem: retrieve an element from a SharedTransformList or SharedGameObjectList by index and write it to a SharedTransform (or SharedVector3).
/// - Robust: validates index and list, returns Failure on invalid state.
/// - Options: clampIndex (bool) to clamp to valid range (default true).
/// </summary>
[TaskCategory("Utility/List")]
[TaskDescription("Get an element from a SharedTransformList or SharedGameObjectList by index and output as SharedTransform.")]
public class GetListItem : Action
{
    [Tooltip("Primary list of transforms")]
    public SharedTransformList transformList;

    [Tooltip("Optional list of gameobjects (fallback)")]
    public SharedGameObjectList gameObjectList;

    [Tooltip("Index to read from")]
    public SharedInt index;

    [Tooltip("Output transform (the element at index)")]
    public SharedGameObject result;

    [Tooltip("Output transform (the element at index)")]
    public SharedVector3 resultPos;

    [Tooltip("If true, clamp index into valid range. If false, invalid index returns Failure.")]
    public bool clampIndex = true;

    public override void OnStart()
    {
        // nothing to cache; lists are referenced directly to avoid allocations
    }

    public override TaskStatus OnUpdate()
    {
        int count = 0;

        // Prefer transformList
        if (transformList != null && transformList.Value != null && transformList.Value.Count > 0)
        {
            count = transformList.Value.Count;
            if (count == 0) return TaskStatus.Failure;

            int idx = index == null ? 0 : index.Value;

            if (clampIndex)
            {
                idx = Mathf.Clamp(idx, 0, count - 1);
            }
            else
            {
                if (idx < 0 || idx >= count) return TaskStatus.Failure;
            }

            Transform t = transformList.Value[idx];
            if (t == null) return TaskStatus.Failure;

            if (result == null)
            {
                // if user did not supply a SharedTransform, try to set underlying var by name
                var v = Owner.GetVariable("currentWaypoint") as SharedTransform;
                var vPos = Owner.GetVariable("currentWaypointPos") as SharedVector3;
                if (v != null) v.Value = t;
                if (vPos != null) vPos.Value = t.position;
            }
            else
            {
                result.Value = t.GameObject();
                Debug.Log("resultt = " + result + " | result.value = " + result.Value + " | result.Value.name" + result.Value.name);
            }

            return TaskStatus.Success;
        }

        // Fallback to gameObjectList
        if (gameObjectList != null && gameObjectList.Value != null && gameObjectList.Value.Count > 0)
        {
            count = gameObjectList.Value.Count;
            int idx = index == null ? 0 : index.Value;

            if (clampIndex)
            {
                idx = Mathf.Clamp(idx, 0, count - 1);
            }
            else
            {
                if (idx < 0 || idx >= count) return TaskStatus.Failure;
            }

            GameObject go = gameObjectList.Value[idx];
            if (go == null) return TaskStatus.Failure;

            if (result == null)
            {
                var v = Owner.GetVariable("currentWaypoint") as SharedTransform;
                var vPos = Owner.GetVariable("currentWaypointPos") as SharedVector2;
                var t = Owner.GetVariable("targetPlayer") as SharedGameObject;
                if (v != null) v.Value = go.transform;
                if (vPos != null) vPos.Value = go.transform.position;
                if (t != null) t.Value = go;
            }
            else
            {
                result.Value = go;
            }

            return TaskStatus.Success;
        }

        // Neither list available
        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        // nothing to cleanup
    }

    public override void OnReset()
    {
        transformList = null;
        gameObjectList = null;
        index = 0;
        result = null;
        resultPos = null;
        clampIndex = true;
    }
}

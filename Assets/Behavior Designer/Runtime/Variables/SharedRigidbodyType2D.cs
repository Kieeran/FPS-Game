#if UNITY_6000_0_OR_NEWER
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedRigidbodyType2D : SharedVariable<RigidbodyType2D>
    {
        public static implicit operator SharedRigidbodyType2D(RigidbodyType2D value) { return new SharedRigidbodyType2D { mValue = value }; }
    }
}
#endif
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent navMeshAgent;
    void Start()
    {
        navMeshAgent.SetDestination(target.position);
    }
}
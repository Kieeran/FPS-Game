using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITarget : MonoBehaviour
{
    public Transform target;
    public float AttackDistance;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, target.position);
        if (m_Distance < AttackDistance)
        {
            m_Agent.isStopped = true;
            m_Animator.SetBool("IsAttacking", true);
        }
        else
        {
            m_Agent.isStopped = false;
            m_Animator.SetBool("IsAttacking", false);
            m_Agent.destination = target.position;
        }
    }

    void OnAnimatorMove()
    {
        if (m_Animator.GetBool("IsAttacking") == false)
        {
            m_Agent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
        }
    }
}

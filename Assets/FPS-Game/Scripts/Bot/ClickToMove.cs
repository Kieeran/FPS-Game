using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;

namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMove : MonoBehaviour
    {
        private NavMeshAgent m_Agent;
        private RaycastHit m_HitInfo = new RaycastHit();
        private Animator m_Animator;

        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                {
                    m_Agent.destination = m_HitInfo.point;
                }
            }

            if (m_Agent.velocity.magnitude != 0f)
            {
                m_Animator.SetBool("IsRunning", true);
                m_Animator.SetFloat("Speed", 2f);
            }
            else
            {
                m_Animator.SetBool("IsRunning", false);
                m_Animator.SetFloat("Speed", 1f);
            }
        }

        void OnAnimatorMove()
        {
            if (m_Animator.GetBool("IsRunning"))
            {
                m_Agent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
                m_Animator.SetFloat("Speed", 2f);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Patrol : MonoBehaviour {
    private AIEnemyController m_AICharacterControl;
    private Transform m_Target;
    public Transform priorityTarget;
    public int patrolIndex = 0;
    public bool m_RandomIndices = false;
    private float m_IdleTimer = 0;
    
    public Transform patrolRoute;
    public float idleTime;
    public float chaseDistance;

    // Start is called before the first frame update
    void Start() {
        m_AICharacterControl = GetComponent<AIEnemyController>();
        if (m_RandomIndices)
            patrolIndex = Random.Range(0, patrolRoute.childCount);
    }

    // Update is called once per frame
    void Update() {
        if (patrolRoute) {
            m_Target = patrolRoute.GetChild(patrolIndex);
            float distance = Vector3.Distance(transform.position, m_Target.position);
            if (distance <= 0.3f) {
                if (m_IdleTimer >= idleTime) {
                    if (m_RandomIndices)
                        patrolIndex = Random.Range(0, patrolRoute.childCount);
                    else
                        patrolIndex = (patrolIndex + 1) % patrolRoute.childCount;
                    m_IdleTimer = 0;
                }
                else
                    m_IdleTimer += Time.deltaTime;
            }
        }

        if (priorityTarget) {
            float priorityTargetDistance = Vector3.Distance(transform.position, priorityTarget.position);
            if (priorityTargetDistance <= chaseDistance)
                m_Target = priorityTarget;
        }

        if (m_Target) {
            m_AICharacterControl.target = m_Target;
        }
    }
}

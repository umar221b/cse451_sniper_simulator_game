using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof (Enemy))]

public class AIEnemyController : MonoBehaviour {
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public Enemy character { get; private set; } // the character we are controlling
    public Transform target;                             // target to aim for

    private void Start() {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<Enemy>();
        agent.updateRotation = false;
        agent.updatePosition = true;
    }


    private void Update() {
        if (target != null)
            agent.SetDestination(target.position);

        if (agent.remainingDistance > agent.stoppingDistance)
            character.Move(agent.desiredVelocity, false, false);
        else
            character.Move(Vector3.zero, false, false);
    }


    public void SetTarget(Transform target) {
        this.target = target;
    }

    public void Die() {
        agent.enabled = false;
        enabled = false;
    }
}

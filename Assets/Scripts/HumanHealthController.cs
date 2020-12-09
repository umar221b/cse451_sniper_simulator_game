using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HumanHealthController : GenericHealthController {

    private bool m_Dying = false;
    private Animator m_Animator;
    private static readonly int Die1 = Animator.StringToHash("die");

    private void Start() {
        m_Animator = GetComponent<Animator>();
    }

    public override void TakeDamage(float damage, RaycastHit hitLocation) {
        base.TakeDamage(damage, hitLocation);
        if (currentHealth <= 0)
            GameManager.instance.UpdateLongestShot(hitLocation.point);
    }
    
    protected override void Die() {
        if (m_Dying)
            return;

        AIEnemyController aiEnemyController = GetComponent<AIEnemyController>();
        if (aiEnemyController) {
            m_Dying = true;
            aiEnemyController.Die();
            m_Animator.SetTrigger(Die1);
            Destroy(this.gameObject, 25);
        }
        else
            Destroy(this.gameObject);
    }
}
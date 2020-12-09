using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHealthController : MonoBehaviour {
    public float maxHealth;
    public float currentHealth;
    public ParticleSystem hitParticles;

    public virtual void TakeDamage(float damage, RaycastHit hitLocation) {
        if (hitParticles)
            ShowHit(hitLocation);
        
        currentHealth -= damage;
        
        if (currentHealth <= 0)
            Die();
    }
    
    protected void ShowHit(RaycastHit hitLocation) {
        ParticleSystem hit = Instantiate(hitParticles, hitLocation.point,
            Quaternion.FromToRotation(Vector3.up, hitLocation.normal));
        hit.transform.parent = gameObject.transform;
    }
    
    protected virtual void Die() {
        throw new NotImplementedException(); 
    }
    
    public float GetMaxHealth() {
        return maxHealth;
    }
    
    public float GetCurrentHealth() {
        return currentHealth;
    }
}

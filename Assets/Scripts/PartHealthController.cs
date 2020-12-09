using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartHealthController : GenericHealthController {
    public float damageMultiplier;
    public GenericHealthController parentHealthController;
    
    public override void TakeDamage(float damage, RaycastHit hitLocation) {
        parentHealthController.TakeDamage(damage * damageMultiplier, hitLocation);
    }
    
    protected override void Die() {
        return;
    }
    
}

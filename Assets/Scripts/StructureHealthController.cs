using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureHealthController : GenericHealthController {
    protected override void Die() {
        Destroy(this.gameObject);
    }
}

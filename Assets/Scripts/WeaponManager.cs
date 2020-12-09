using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public float switchDelay;

    public Weapon[] weapons;

    private int m_Index;
    private bool m_IsSwitching = false;

    // Start is called before the first frame update
    void Start() {
        InitializeWeapons();
    }

    // Update is called once per frame
    void Update() {
        float indexChanging = Input.GetAxis("Mouse ScrollWheel");
        if (indexChanging != 0 && !m_IsSwitching && !weapons[m_Index].IsReloading()) {
            int offset = 1;
            if (indexChanging > 0)
                offset = 1;
            else if (indexChanging < 0)
                offset = -1;
            StartCoroutine(SwitchAfterDelay(offset));
        }
    }

    void InitializeWeapons() {
        foreach (Weapon weapon in weapons) {
            weapon.gameObject.SetActive(false);
        }
        if (weapons.Length > 0)
            weapons[0].gameObject.SetActive(true);
    }

    private IEnumerator SwitchAfterDelay(int offset) {
        m_IsSwitching = true;
        
        yield return new WaitForSeconds(switchDelay);

        m_IsSwitching = false;
        SwitchWeapons(offset);
    }
    
    void SwitchWeapons(int offset) {
        weapons[m_Index].gameObject.SetActive(false);
        m_Index += offset;
        if (m_Index < 0)
            m_Index = weapons.Length - 1;
        m_Index %= weapons.Length;
        weapons[m_Index].gameObject.SetActive(true);
    }
}

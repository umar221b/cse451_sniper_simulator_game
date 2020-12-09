using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SniperAimSway : MonoBehaviour {
    public float scopeSwayMin = -1.5f; 
    public float scopeSwayMax = 1.5f;  
    public float scopeSwaySpeed = 0.01f;
    public GameObject playerRotation;

    private Weapon m_Sniper;
    
    
    private float m_RotateX, m_RotateY;
    private Quaternion m_Target;
    private float m_MaxAngle;

    private void Awake() {
        m_Sniper = GetComponent<Weapon>();

        m_RotateX = Random.Range(scopeSwayMin, scopeSwayMax);
        m_RotateY = Random.Range(scopeSwayMin, scopeSwayMax);
        
        m_Target = Quaternion.Euler(new Vector3(m_RotateX, m_RotateY, 0));
        m_MaxAngle = Quaternion.Angle(playerRotation.transform.localRotation, m_Target);
    }

    private void Update() {
        if (m_Sniper.IsAiming()) {
            Quaternion localRotation = playerRotation.transform.localRotation;
            float angle = Quaternion.Angle(localRotation, m_Target);

            float factor = Mathf.Abs(-0.3f * angle * (angle - m_MaxAngle) + 0.1f);
            if (angle > 0.1f) {
                playerRotation.transform.localRotation = Quaternion.Slerp (localRotation, m_Target , factor * scopeSwaySpeed * Time.deltaTime); 
            }
            else {
                m_RotateX = Random.Range(scopeSwayMin, scopeSwayMax);
                m_RotateY = Random.Range(scopeSwayMin, scopeSwayMax);
            
                m_Target = Quaternion.Euler(new Vector3(m_RotateX, m_RotateY, 0));
                m_MaxAngle = Quaternion.Angle(localRotation, m_Target);
            }
        }
    }
}

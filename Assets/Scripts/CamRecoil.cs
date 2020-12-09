using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class CamRecoil : MonoBehaviour {
    [Header("Weapon")]
    public Weapon weapon;

    [Header("Recoil Settings")]
    public float rotationSpeed;
    public float returnSpeed;
    
    [Header("Hip Fire")]
    public Vector3 recoilRotation;

    [Header("Aiming")]
    public Vector3 recoilRotationAiming;
    
    private Vector3 m_CurrentRotation;
    private Vector3 m_Rot;

    private void FixedUpdate() {
        m_CurrentRotation = Vector3.Lerp(m_CurrentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        m_Rot = Vector3.Slerp(m_Rot, m_CurrentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(m_Rot);
    }

    public void Rotate() {
        if (weapon.IsAiming()) {
            m_CurrentRotation += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
        }
        else {
            m_CurrentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        }
    }

    private void Update() {
        if (weapon.IsFiring())
            Rotate();
    }
}
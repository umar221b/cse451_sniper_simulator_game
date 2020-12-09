using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponRecoil : MonoBehaviour {
    [Header("Weapon")]
    public Weapon weapon;
    
    [Header("Reference Points")]
    public Transform recoilPosition;
    public Transform rotationPoint;
    
    [Header("Speed")]
    public float positionalRecoilSpeed;
    public float rotationalRecoilSpeed;
    public float positionalReturnSpeed;
    public float rotationalReturnSpeed;
    
    [Header("Amount")]
    public Vector3 recoilRotation;
    public Vector3 recoilKickback;
    public Vector3 recoilRotationAim;
    public Vector3 recoilKickbackAim;

    private Vector3 m_RotationalRecoil;
    private Vector3 m_PositionalRecoil;
    private Vector3 m_Rot;
    private Vector3 m_OriginalPosition;
    private Vector3 m_OriginalRotation;

    private void Awake() {
        m_OriginalPosition = recoilPosition.localPosition;
        m_OriginalRotation = rotationPoint.localEulerAngles;
    }

    private void FixedUpdate() {
        m_PositionalRecoil = Vector3.Lerp(m_PositionalRecoil, m_OriginalPosition, positionalReturnSpeed * Time.deltaTime);
        m_RotationalRecoil = Vector3.Lerp(m_RotationalRecoil, m_OriginalRotation, rotationalReturnSpeed * Time.deltaTime);
        
        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, m_PositionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        m_Rot = Vector3.Slerp(m_Rot, m_RotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(m_Rot);
    }

    private void Recoil() {
        if (weapon.IsAiming()) {
            m_PositionalRecoil += new Vector3(Random.Range(-recoilKickbackAim.x, recoilKickbackAim.x), Random.Range(-recoilKickbackAim.y, recoilKickbackAim.y), recoilKickbackAim.z);
            m_RotationalRecoil += new Vector3(-recoilRotationAim.x, Random.Range(-recoilRotationAim.y, recoilRotationAim.y), Random.Range(-recoilRotationAim.z, recoilRotationAim.z));
        }
        else {
            m_PositionalRecoil += new Vector3(Random.Range(-recoilKickback.x, recoilKickback.x), Random.Range(-recoilKickback.y, recoilKickback.y), recoilKickback.z);
            m_RotationalRecoil += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        }
    }

    private void Update() {
        if (weapon.IsFiring())
            Recoil();
    }
}

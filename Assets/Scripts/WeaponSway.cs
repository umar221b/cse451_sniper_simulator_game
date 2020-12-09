using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour {

    [Header("Position")]
    public float amount;
    public float maxSway;
    public float smoothAmount;
    
    [Header("Rotation")]
    public float rotationAmount;
    public float rotationMaxSway;
    public float rotationSmoothAmount;

    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;
    
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;
    
    private float m_InputX, m_InputY;

    // Start is called before the first frame update
    void Start() {
        var objectTransform = transform;
        m_InitialPosition = objectTransform.localPosition;
        m_InitialRotation = objectTransform.localRotation;
    }

    // Update is called once per frame
    void Update() {
        CalculateSway();
        MoveSway();
        TiltSway();
    }

    void CalculateSway() {
        m_InputX = -Input.GetAxis("Mouse X");
        m_InputY = -Input.GetAxis("Mouse Y");
    }
    
    void MoveSway() {
        float movementX = Mathf.Clamp(m_InputX * amount, -maxSway, maxSway);
        float movementY = Mathf.Clamp(m_InputY * amount, -maxSway, maxSway);
        
        Vector3 finalOffset = new Vector3(movementX, movementY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_InitialPosition + finalOffset, Time.deltaTime * smoothAmount);
    }
    
    void TiltSway() {
        float tiltX = Mathf.Clamp(m_InputX * rotationAmount, -rotationMaxSway, rotationMaxSway);
        float tiltY = Mathf.Clamp(m_InputY * rotationAmount, -rotationMaxSway, rotationMaxSway);
        
        Quaternion finalOffset = Quaternion.Euler(new Vector3(
            rotationX ? -tiltX : 0f,
            rotationY ? tiltY : 0f, 
            rotationZ ? tiltY : 0f
        ));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, m_InitialRotation * finalOffset, Time.deltaTime * rotationSmoothAmount);
    }
}

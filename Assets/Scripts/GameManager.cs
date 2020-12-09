using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Transform firingPoint;
    public TextMeshProUGUI text;
    public float windSpeedMin, windSpeedMax;
    public float windChangeMin, windChangeMax;

    private float m_WindTimer = 0;
    private float m_CurWindChangeTime = 0;
    private float m_LongestShotDistance;
    private Vector3 m_WindSpeed;

    // Start is called before the first frame update
    void Start() {
        if (!instance)
            instance = this;
        
        m_WindSpeed = Vector3.zero;
        m_LongestShotDistance = PlayerPrefs.GetFloat("highscore");
    }

    // Update is called once per frame
    void Update() {
        text.SetText("longest kill: " + m_LongestShotDistance + "m");

        m_WindTimer += Time.deltaTime;
        
        if (m_WindTimer >= m_CurWindChangeTime) {
            m_WindSpeed.x = Random.Range(windSpeedMin, windSpeedMax);
            m_WindSpeed.z = Random.Range(windSpeedMin, windSpeedMax);
            m_WindTimer = 0;
            m_CurWindChangeTime = Random.Range(windChangeMin, windChangeMax);
        }
    }

    public void UpdateLongestShot(Vector3 location) {
        float distance = Vector3.Distance(firingPoint.position, location);
        distance = Mathf.Round(distance);
        if (distance > m_LongestShotDistance) {
            m_LongestShotDistance = distance;
            PlayerPrefs.SetFloat("highscore", m_LongestShotDistance);
        }
    }
    
    public Vector3 GetWindSpeed() {
        return m_WindSpeed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFlowManager : MonoBehaviour {
    public static TimeFlowManager instance;
    
    public float slowDownLength, slowDownFactor;
    // Start is called before the first frame update
    void Start() {
        if (!instance)
            instance = this;
    }

    // Update is called once per frame
    void Update() {
        Time.timeScale += (1.0f / slowDownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
    }

    public void DoSlowMotion() {
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}

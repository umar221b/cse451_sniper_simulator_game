using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public Text highscoreText;

    private void Start() {
        highscoreText.text = "Highscore: " + PlayerPrefs.GetFloat("highscore", 0) + "m";
    }

    public void PlayGame() {
        SceneManager.LoadScene("Game");
    }

    public void ResetHighscore() {
        PlayerPrefs.SetFloat("highscore", 0);
        highscoreText.text = "Highscore: 0m";
    }
    
    public void SetEnemyMarkers(bool value) {
        int intVal = 0;
        if (value)
            intVal = 1;
        PlayerPrefs.SetInt("enemey_markers", intVal);
    }
}

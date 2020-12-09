using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager instance;
    
    public int minNumOfEnemies;
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public GameObject[] patrolRoutes;

    private int m_CurNumOfEnemies = 0;
    private bool m_Quitting = false;

    void Start() {
        if (!instance)
            instance = this;
    }

    public void RegisterEnemy() {
        ++m_CurNumOfEnemies;
    }

    public void UnRegisterEnemy() {
        --m_CurNumOfEnemies;
        if (!m_Quitting && m_CurNumOfEnemies < minNumOfEnemies)
            SpanNewEnemy();
    }

    private void SpanNewEnemy() {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject patrolRoute = patrolRoutes[Random.Range(0, patrolRoutes.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<Patrol>().patrolRoute = patrolRoute.transform;
    }

    private void OnApplicationQuit() {
        m_Quitting = true;
    }
}

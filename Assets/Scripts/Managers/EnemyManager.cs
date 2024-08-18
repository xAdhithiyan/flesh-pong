using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using Unity.Mathematics;

public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float enemySpawnDelayMin;
    [SerializeField] private float enemySpawnDelayMax;
    [SerializeField] private int maxEnemy;
    [SerializeField, Range(5,150)] private int minEnemySpawnRange;
    [SerializeField, Range(5,150)] private int maxEnemySpawnRange;
    [SerializeField, ReadOnly]
    private int currentEnemies;
    [SerializeField]
    private Enemy EnemyPrefab;

    private Timer spawnTimer;
    void Start()
    {
        spawnTimer = GameManager.Instance.TimerManager.GenerateTimers(gameObject);
        ResetTime();    
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer.IsTimeZero() && currentEnemies < maxEnemy)
        {
            Vector2 randPos = new Vector2(UnityEngine.Random.Range(minEnemySpawnRange, maxEnemySpawnRange), UnityEngine.Random.Range(minEnemySpawnRange, maxEnemySpawnRange));
            randPos.x *= (UnityEngine.Random.Range(0, 2) < 1) ? 1 : -1;
            randPos.y *= (UnityEngine.Random.Range(0, 2) < 1) ? 1 : -1;
            Enemy temp = GameObject.Instantiate(EnemyPrefab, (Vector3)randPos,quaternion.identity);
            temp.Initialise(UnityEngine.Random.Range(1,6),UnityEngine.Random.Range(1,4));
            currentEnemies++;
            ResetTime();
        }
    }

    private void ResetTime()
    {
        spawnTimer.SetTime(UnityEngine.Random.Range(enemySpawnDelayMin, enemySpawnDelayMax));
    }

    public void RemoveEnemy()
    {
        currentEnemies--;
    }
}

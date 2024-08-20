using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.MoreAttributes;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float enemySpawnDelayMin;
    [SerializeField] private float enemySpawnDelayMax;
    [SerializeField] private int maxEnemy;
    [SerializeField, Range(5,50)] public int minEnemySpawnRange;
    [SerializeField, Range(5,50)] private int maxEnemySpawnRange;
    [SerializeField, ReadOnly]
    private int currentEnemies;
    [SerializeField]
    private Enemy EnemyPrefab;
    [SerializeField] 
    private int playerScale;
    [SerializeField]
    private int projectileScale;


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
            Vector2 randPos = new Vector2(Random.Range(minEnemySpawnRange, maxEnemySpawnRange), UnityEngine.Random.Range(minEnemySpawnRange, maxEnemySpawnRange));
            randPos.x *= (Random.Range(0, 2) < 1) ? 1 : -1;
            randPos.y *= (Random.Range(0, 2) < 1) ? 1 : -1;
            Enemy temp = Instantiate(EnemyPrefab, (Vector3)randPos,quaternion.identity);
            temp.Initialise(Mathf.Clamp(GameManager.Instance.PCM.values.GetCurrentScale() + ((Random.Range(1,4) > 2)? 1 : 0) + ((Random.Range(1, 4) > 2) ? -1 : 0),1,5), Random.Range(1,4));
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

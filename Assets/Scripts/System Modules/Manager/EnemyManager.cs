using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyManager类继承自Singleton<EnemyManager>，确保该类是一个单例，且在游戏中只存在一个实例
public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];
    [Header(" --- UI ---")]
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;
    [SerializeField] GameObject waveUI; // 波数UI对象，显示当前波数

    [Header(" --- Enemy Spawn Settings ---")]
    [SerializeField] bool spawnEnemy = true; // 是否继续生成敌人
    [SerializeField] GameObject[] enemyPrefabs; // 敌人预制体数组
    [SerializeField] float timeBetweenSpawns = 1f; // 每次生成敌人之间的时间间隔
    [SerializeField] float timeBetweenWaves = 1f; // 每波之间的时间间隔
    [SerializeField] int minEnemyAmount = 4; // 最少敌人数量
    [SerializeField] int maxEnemyAmount = 10; // 最多敌人数量


    [Header("--- BOSS Settings ---")]
    [SerializeField] GameObject bossPrefab;
    [SerializeField] int bossWaveNumber;

    [Header(" --- Internal State ---")]
    List<GameObject> enemyList;
    int waveNumber = 1;
    int enemyAmount;

    WaitForSeconds waitTimeBetweenSpawns;
    WaitForSeconds waitTImeBetweenWaves;
    WaitUntil waitUntilNoEnemy; // 等待直到没有敌人

    /// <summary>
    /// 初始化方法，确保在游戏开始时准备好敌人生成相关数据
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        enemyList = new List<GameObject>();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTImeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        waitUntilNoEnemy = new WaitUntil(() => enemyList.Count == 0);
    }

    /// <summary>
    /// 游戏开始时的协程，负责波数控制与敌人生成
    /// </summary>
    /// <returns>协程的IEnumerator</returns>
    IEnumerator Start()
    {
        while (spawnEnemy && GameManager.GameState != GameState.GameOver)
        {
            waveUI.SetActive(true);
            yield return waitTImeBetweenWaves;
            waveUI.SetActive(false);

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    /// <summary>
    /// 随机生成敌人的协程
    /// </summary>
    /// <returns>协程的IEnumerator</returns>
    IEnumerator RandomlySpawnCoroutine()
    {
        if (waveNumber % bossWaveNumber == 0)
        {
            var boss = PoolManager.Release(bossPrefab);
            enemyList.Add(boss);
        }
        else
        {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / bossWaveNumber, maxEnemyAmount);

            for (int i = 0; i < enemyAmount; i++)
            {
                enemyList.Add(PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
                yield return waitTimeBetweenSpawns;
            }
        }

        yield return waitUntilNoEnemy;

        waveNumber++;
    }


    /// <summary>
    /// 从敌人列表中移除指定的敌人
    /// </summary>
    /// <param name="enemy">要移除的敌人</param>
    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人管理器（单例模式）
/// 控制每一波敌人或Boss的生成逻辑，管理敌人对象的生命周期和波数状态。
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{
    /// <summary>
    /// 从敌人列表中随机获取一个敌人（用于追踪或攻击逻辑）
    /// </summary>
    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];

    [Header(" --- UI ---")]
    [SerializeField] GameObject waveUI; // 波数UI对象，显示当前波数
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;

    [Header(" --- Enemy Spawn Settings ---")]
    [SerializeField] bool spawnEnemy = true; // 是否继续生成敌人
    [SerializeField] GameObject[] enemyPrefabs; // 敌人预制体数组
    [SerializeField] float timeBetweenSpawns = 1f; // 每个敌人生成间隔
    [SerializeField] float timeBetweenWaves = 1f; // 每波生成间隔
    [SerializeField] int minEnemyAmount = 4; // 每波最少敌人数
    [SerializeField] int maxEnemyAmount = 10; // 每波最多敌人数

    [Header(" --- BOSS Settings ---")]
    [SerializeField] GameObject bossPrefab; // Boss预制体
    [SerializeField] int bossWaveNumber; // 每多少波出现一次Boss

    [Header(" --- Internal State ---")]
    List<GameObject> enemyList; // 当前场上敌人列表
    int waveNumber = 1;         // 当前波数
    int enemyAmount;            // 当前波敌人数量

    WaitForSeconds waitTimeBetweenSpawns; // 等待每个敌人之间
    WaitForSeconds waitTImeBetweenWaves;  // 等待每波之间
    WaitUntil waitUntilNoEnemy;           // 等待敌人全部死亡

    /// <summary>
    /// 初始化敌人列表和等待对象
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
    /// 启动敌人生成流程，每轮等待再生成新敌人或Boss
    /// </summary>
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
    /// 随机生成敌人或Boss（每bossWaveNumber波生成一次Boss）
    /// </summary>
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
                var enemy = PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
                enemyList.Add(enemy);
                yield return waitTimeBetweenSpawns;
            }
        }

        yield return waitUntilNoEnemy;

        waveNumber++;
    }

    /// <summary>
    /// 从当前敌人列表中移除已死亡的敌人
    /// </summary>
    /// <param name="enemy">要移除的敌人对象</param>
    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);
}

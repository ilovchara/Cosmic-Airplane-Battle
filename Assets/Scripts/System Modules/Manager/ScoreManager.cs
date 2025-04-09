using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 分数管理器（单例模式）
/// 控制分数的增加、显示动画、高分记录的保存与读取。
/// </summary>
public class ScoreManager : PersistentSingleton<ScoreManager>
{
    #region 分数显示逻辑

    public int Score => score; // 当前分数，只读属性

    int score;             // 当前已展示的分数
    int currentScore;      // 实际累积分数

    Vector3 scoreTextScale = new Vector3(1.2f, 1.2f, 1f); // 分数文本动画缩放值

    /// <summary>
    /// 重置分数为0
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpdateText(score);
    }

    /// <summary>
    /// 增加分数（通过协程播放逐步增加动画）
    /// </summary>
    /// <param name="scorePoint">增加的分数值</param>
    public void AddScore(int scorePoint)
    {
        currentScore += scorePoint;
        StartCoroutine(nameof(AddScoreCoroutine));
    }

    /// <summary>
    /// 分数动画协程：逐步增加分数并更新显示
    /// </summary>
    IEnumerator AddScoreCoroutine()
    {
        ScoreDisplay.ScaleText(scoreTextScale);
        while (score < currentScore)
        {
            score += 1;
            ScoreDisplay.UpdateText(score);
            yield return null;
        }
        ScoreDisplay.ScaleText(Vector3.one);
    }

    #endregion

    #region 玩家分数数据结构

    /// <summary>
    /// 单个玩家分数结构
    /// </summary>
    [System.Serializable]
    public class PlayerScore
    {
        public int score;
        public string playerName;

        public PlayerScore(int score, string playerName)
        {
            this.score = score;
            this.playerName = playerName;
        }
    }

    /// <summary>
    /// 玩家分数集合，用于保存多个玩家分数
    /// </summary>
    [System.Serializable]
    public class PlayerScoreData
    {
        public List<PlayerScore> list = new List<PlayerScore>();
    }

    #endregion

    #region 分数存储与读取

    readonly string SaveFileName = "player_score.json"; // 保存文件名
    string playerName = "No Name";                      // 当前玩家名

    /// <summary>
    /// 当前分数是否为新高分（和第10名比较）
    /// </summary>
    public bool HasNewHighScore => score > LoadPlayerScoreData().list[9].score;

    /// <summary>
    /// 设置当前玩家名称
    /// </summary>
    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    /// <summary>
    /// 保存当前分数到排行榜中，并自动排序
    /// </summary>
    public void SavePlayerScoreData()
    {
        var playerScoreData = LoadPlayerScoreData();
        playerScoreData.list.Add(new PlayerScore(score, playerName));
        playerScoreData.list.Sort((x, y) => y.score.CompareTo(x.score)); // 高分靠前
        SaveSystem.Save(SaveFileName, playerScoreData);
    }

    /// <summary>
    /// 读取玩家分数数据，如不存在则初始化默认数据
    /// </summary>
    /// <returns>玩家分数数据</returns>
    public PlayerScoreData LoadPlayerScoreData()
    {
        var playerScoreData = new PlayerScoreData();

        if (SaveSystem.SaveFileExists(SaveFileName))
        {
            playerScoreData = SaveSystem.Load<PlayerScoreData>(SaveFileName);
        }
        else
        {
            while (playerScoreData.list.Count < 10)
            {
                playerScoreData.list.Add(new PlayerScore(0, playerName));
            }
            SaveSystem.Save(SaveFileName, playerScoreData);
        }

        return playerScoreData;
    }

    #endregion
}

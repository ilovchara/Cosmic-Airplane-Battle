using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ScoreManager : PersistentSingleton<ScoreManager>
{
    #region  Score Display
    // 属性
    public int Score => score; // 当前分数，只读属性
    int score; // 当前分数
    int currentScore; // 当前累积的分数

    Vector3 scoreTextScale = new Vector3(1.2f, 1.2f, 1f); // 分数文本的缩放比例

    // 重置分数
    public void ResetScore()
    {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpdateText(score); // 更新分数显示
    }

    // 增加分数
    public void AddScore(int scorePoint)
    {
        currentScore += scorePoint;
        StartCoroutine(nameof(AddScoreCoroutine)); // 启动协程更新分数显示
    }

    // 协程，逐步增加分数并更新显示
    IEnumerator AddScoreCoroutine()
    {
        ScoreDisplay.ScaleText(scoreTextScale); // 缩放分数文本
        while (score < currentScore)
        {
            score += 1;
            ScoreDisplay.UpdateText(score); // 更新分数显示
            yield return null;
        }
        ScoreDisplay.ScaleText(Vector3.one); // 恢复分数文本缩放
    }
    #endregion

    // 玩家分数类
    [System.Serializable]
    public class PlayerScore
    {
        public int score; // 分数
        public string playerName; // 玩家名称

        public PlayerScore(int score, string playerName)
        {
            this.score = score;
            this.playerName = playerName;
        }
    }

    // 玩家分数数据类，包含玩家分数列表
    [System.Serializable]
    public class PlayerScoreData
    {
        public List<PlayerScore> list = new List<PlayerScore>(); // 玩家分数列表
    }

    readonly string SaveFileName = "player_score.json"; // 保存分数数据的文件名
    string playerName = "No Name"; // 玩家名称

    // 判断当前分数是否为新的高分
    public bool HasNewHighScore => score > LoadPlayerScoreData().list[9].score;

    // 设置玩家名称
    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }

    // 保存玩家分数数据
    public void SavePlayerScoreData()
    {
        var playerScoreData = LoadPlayerScoreData();

        playerScoreData.list.Add(new PlayerScore(score, playerName));
        playerScoreData.list.Sort((x, y) => y.score.CompareTo(x.score)); // 按分数降序排序

        SaveSystem.Save(SaveFileName, playerScoreData); // 保存数据
    }

    // 加载玩家分数数据，如果文件不存在则创建一个新的数据文件
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
}

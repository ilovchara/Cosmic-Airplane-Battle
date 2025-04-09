using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{
    [Header(" == BACKGROUND ==")]
    [SerializeField] Image background; // 背景图像
    [SerializeField] Sprite[] backgroundImages; // 背景图像数组

    [Header(" == SCORING SCREEN == ")]
    [SerializeField] Canvas scoringScreenCanvas; // 得分界面画布
    [SerializeField] Text playerScoreText; // 显示玩家分数的文本
    [SerializeField] Button buttonMainMenu; // 主菜单按钮
    [SerializeField] Transform highScoreLeaderboardContainer; // 高分榜容器

    [Header(" == HIGH SCORE SCREEN")]
    [SerializeField] Canvas newHightScoreScreenCanvas; // 新高分界面画布
    [SerializeField] Button buttonCancel; // 取消按钮
    [SerializeField] Button buttonSubmit; // 提交按钮
    [SerializeField] InputField playerNameInputField; // 玩家姓名输入框

    // Start时初始化并显示对应的界面
    void Start()
    {
        Cursor.visible = true; // 显示鼠标
        Cursor.lockState = CursorLockMode.None; // 解除鼠标锁定
        ShowRandomBackground(); // 显示随机背景

        // 判断是否为新高分
        if (ScoreManager.Instance.HasNewHighScore)
        {
            ShowNewHighScoreScreen(); // 显示新高分界面
        }
        else
        {
            ShowScoringScreen(); // 显示得分界面
        }

        // 将按钮与对应事件绑定
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonMainMenu.gameObject.name, OnButtonMainMenuClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonSubmit.gameObject.name, OnButtonSUbmitClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonCancel.gameObject.name, HideNewHightScoreScreen);

        // 设置游戏状态为得分
        GameManager.GameState = GameState.Scoring;
    }

    // 显示新高分界面
    void ShowNewHighScoreScreen()
    {
        newHightScoreScreenCanvas.enabled = true; // 启用新高分界面
        UIInput.Instance.SelectUI(buttonCancel); // 选择取消按钮
    }

    // 脚本禁用时清空按钮功能表
    void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    // 显示随机背景
    void ShowRandomBackground()
    {
        background.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
    }

    // 显示得分界面，更新分数，并调整鼠标状态
    void ShowScoringScreen()
    {
        scoringScreenCanvas.enabled = true; // 启用得分界面
        playerScoreText.text = ScoreManager.Instance.Score.ToString(); // 显示当前分数

        UIInput.Instance.SelectUI(buttonMainMenu); // 选择主菜单按钮
        UpdateHighScoreLeaderboard(); // 更新高分榜
    }

    // 更新高分榜
    public void UpdateHighScoreLeaderboard()
    {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list; // 获取玩家得分列表

        // 遍历并更新高分榜
        for (int i = 0; i < highScoreLeaderboardContainer.childCount; i++)
        {
            var child = highScoreLeaderboardContainer.GetChild(i);

            child.Find("Rank").GetComponent<Text>().text = (i + 1).ToString(); // 排名
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString(); // 分数
            child.Find("Name").GetComponent<Text>().text = playerScoreList[i].playerName; // 玩家姓名
        }
    }

    // 主菜单按钮点击事件
    void OnButtonMainMenuClicked()
    {
        scoringScreenCanvas.enabled = false; // 隐藏得分界面
        SceneLoader.Instance.LoadMainMenuScene(); // 加载主菜单场景
    }

    // 提交按钮点击事件
    void OnButtonSUbmitClicked()
    {
        if(!string.IsNullOrEmpty(playerNameInputField.text)) // 确保玩家输入了姓名
        {
            ScoreManager.Instance.SetPlayerName(playerNameInputField.text); // 设置玩家姓名
        }
        HideNewHightScoreScreen(); // 隐藏新高分界面
    }

    // 隐藏新高分界面并保存分数
    private void HideNewHightScoreScreen()
    {
        newHightScoreScreenCanvas.enabled = false; // 隐藏新高分界面
        ScoreManager.Instance.SavePlayerScoreData(); // 保存玩家分数数据
        ShowRandomBackground(); // 显示随机背景
        ShowScoringScreen(); // 显示得分界面
    }
}

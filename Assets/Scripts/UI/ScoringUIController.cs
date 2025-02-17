using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{

    [Header(" == BACKGROUND ==")]
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundImages;
    [Header(" == SCORING SCREEN == ")]
    [SerializeField] Canvas scoringScreenCanvas;
    [SerializeField] Text playerScoreText;
    [SerializeField] Button buttonMainMenu;
    [SerializeField] Transform highScoreLeaderboardContainer;

    void Start()
    {
        ShowRandomBackground();
        ShowScoringScreen();

        ButtonPressedBehavior.buttonFunctionTable.Add(buttonMainMenu.gameObject.name, OnButtonMainMenuClicked);
        GameManager.GameState = GameState.Scoring;
    }

    void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }


    void ShowRandomBackground()
    {
        background.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
    }
    
    // 显示得分界面，更新分数，并调整鼠标状态
    void ShowScoringScreen()
    {
        scoringScreenCanvas.enabled = true;
        playerScoreText.text = ScoreManager.Instance.Score.ToString();

        UIInput.Instance.SelectUI(buttonMainMenu);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        UpdateHighScoreLeaderboard();
    }


    public void UpdateHighScoreLeaderboard()
    {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list;

        for (int i = 0; i < highScoreLeaderboardContainer.childCount; i++)
        {
            var child = highScoreLeaderboardContainer.GetChild(i);

            child.Find("Rank").GetComponent<Text>().text = (i + 1).ToString();
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString();
            child.Find("Name").GetComponent<Text>().text = playerScoreList[i].playerName;
        }
    }



    void OnButtonMainMenuClicked()
    {
        scoringScreenCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }







}

using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    // 静态文本对象，用于显示分数
    static Text scoreText;

    // 在Awake中初始化scoreText
    void Awake()
    {
        scoreText = GetComponent<Text>();
    }

    // 在Start中重置分数
    void Start()
    {
        ScoreManager.Instance.ResetScore(); // 重置分数
    }

    // 更新显示的分数
    public static void UpdateText(int score) => scoreText.text = score.ToString();

    // 设置文本的缩放
    public static void ScaleText(Vector3 targetScale) => scoreText.rectTransform.localScale = targetScale;
}

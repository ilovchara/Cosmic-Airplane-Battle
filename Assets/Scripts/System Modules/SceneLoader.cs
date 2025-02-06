using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // 定义常量
    const string GAMEPLAY_SCENE = "Gameplay";

    // 序列化字段
    [SerializeField] private Image transitionImage; // 过渡图像组件
    [SerializeField] private float fadeTime = 3.5f; // 淡入淡出时间

    // 私有变量
    private Color transitionColor; // 用于控制过渡图像的透明度

    // 初始化方法
    private void Start()
    {
        // 初始化过渡图像状态
        transitionImage.gameObject.SetActive(false);
        transitionColor = new Color(0, 0, 0, 0); // 初始透明度为0
    }

    // 公共方法：加载指定场景
    public void LoadScene(string sceneName)
    {
        StopAllCoroutines(); // 停止所有正在运行的协程
        StartCoroutine(LoadCoroutine(sceneName));
    }

    // 公共方法：加载游戏场景（快捷方法）
    public void LoadGamePlayScene()
    {
        LoadScene(GAMEPLAY_SCENE);
    }

    // 协程：加载场景并执行过渡效果
    private IEnumerator LoadCoroutine(string sceneName)
    {
        // 开始异步加载场景
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false; // 禁止自动激活场景

        // 显示过渡图像
        transitionImage.gameObject.SetActive(true);

        // 淡入效果
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            transitionColor.a = elapsedTime / fadeTime;
            transitionImage.color = transitionColor;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // 等待场景加载完成
        while (!loadingOperation.isDone)
        {
            // 显示加载进度（可选）
            Debug.Log($"Loading Progress: {loadingOperation.progress * 100}%");

            // 如果加载完成，激活场景
            if (loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        // 淡出效果
        elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            transitionColor.a = 1f - (elapsedTime / fadeTime);
            transitionImage.color = transitionColor;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // 隐藏过渡图像
        transitionImage.gameObject.SetActive(false);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// SceneLoader 管理类，用于实现场景的渐变过渡效果并进行异步加载。
/// 继承 PersistentSingleton，确保全局唯一且跨场景存在。
/// </summary>
public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] UnityEngine.UI.Image transitionImage; // 用于淡入淡出的 UI 图像
    [SerializeField] float fadeTime = 2f; // 渐变所需时间

    Color color;

    const string GAMEPLAY = "Gameplay";     // 游戏主场景名称
    const string MAIN_MENU = "MainMenu";    // 主菜单场景名称
    const string SCORING = "Scoring";       // 计分场景名称

    /// <summary>
    /// 异步加载场景并执行淡出 / 淡入动画
    /// </summary>
    /// <param name="sceneName">目标场景名</param>
    /// <returns>协程</returns>
    IEnumerator LoadingCoroutine(string sceneName)
    {
        // 异步加载目标场景，但暂不激活
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        // 开始淡出：设置遮罩图像可见
        transitionImage.gameObject.SetActive(true);

        // 淡出动画：从透明到不透明
        while (color.a < 1f)
        {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;
            yield return null;
        }

        // 等待加载进度到达 90%
        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        // 激活场景
        loadingOperation.allowSceneActivation = true;

        // 淡入动画：从不透明回到透明
        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;
            yield return null;
        }

        // 关闭遮罩图像
        transitionImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 加载 Gameplay 场景
    /// </summary>
    public void LoadGameplayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(GAMEPLAY));
    }

    /// <summary>
    /// 加载主菜单场景
    /// </summary>
    public void LoadMainMenuScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(MAIN_MENU));
    }

    /// <summary>
    /// 加载计分界面场景
    /// </summary>
    public void LoadScoringScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(SCORING));
    }
}

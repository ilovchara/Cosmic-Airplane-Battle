using System.Collections;
using UnityEngine;

// 实现背景滚动的类
public class BackgroundScoller : MonoBehaviour
{
    // 背景滚动的速度，控制 X 和 Y 轴方向的滚动速度
    [SerializeField] Vector2 scroVelocity;

    // 背景材质
    public Material material;

    // 初始化时获取材质
    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // 协程：模拟背景滚动的效果
    IEnumerator Start()
    {
        // 当游戏状态不是游戏结束时，持续滚动背景
        while (GameManager.GameState != GameState.GameOver)
        {
            // 每帧更新材质的纹理偏移量，模拟背景滚动
            material.mainTextureOffset += scroVelocity * Time.deltaTime;
            
            // 等待下一帧继续执行
            yield return null;
        }
    }
}

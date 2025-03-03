using System.Collections;
using UnityEngine;
// One 实现背景的滚动
public class BackgroundScoller : MonoBehaviour
{
    // 速度变量 控制
    [SerializeField] Vector2 scroVelocity;

    public Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // 通过协程模仿Update 实现卷轴无限运动的效果
    IEnumerator Start()
    {
        while(GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scroVelocity * Time.deltaTime;
            yield return null;
        }
    }


}

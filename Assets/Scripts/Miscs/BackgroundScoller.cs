using System.Collections;
using UnityEngine;
// 简单滚动
public class BackgroundScoller : MonoBehaviour
{
    // 速度变量 控制
    [SerializeField] Vector2 scroVelocity;

    public Material material;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while(GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scroVelocity * Time.deltaTime;
            yield return null;
        }
    }


}

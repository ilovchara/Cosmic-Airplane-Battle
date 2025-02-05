using UnityEngine;
// 简单滚动
public class BackgroundScoller : MonoBehaviour
{
    // 速度变量 控制
    [SerializeField] Vector2 scroVelocity;

    public Material material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += scroVelocity * Time.deltaTime;
    }
}

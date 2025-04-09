using System.Collections;
using UnityEngine;

// 让游戏对象自动旋转的类
public class AutoRotate : MonoBehaviour
{
    // 旋转速度，单位为度每秒
    [SerializeField] float speed = 360f;
    
    // 旋转的角度轴（X, Y, Z轴的旋转分量）
    [SerializeField] Vector3 angle;

    // 当游戏对象启用时调用
    void OnEnable()
    {
        // 启动旋转协程
        StartCoroutine(RotateCoroutine());
    }

    // 旋转协程
    IEnumerator RotateCoroutine()
    {
        // 无限循环，使游戏对象持续旋转
        while (true)
        {
            // 根据指定的角度轴和速度旋转游戏对象
            transform.Rotate(angle * speed * Time.deltaTime);
            
            // 等待下一帧继续执行
            yield return null;
        }
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// 这个类用于获取视野中的位置，并提供相关功能如限制玩家移动范围、随机生成敌人位置等
public class Viewport : Singleton<Viewport>
{
    // 定义限制窗口
    float minX;
    float maxX;
    float minY;
    float maxY;
    float middleX;
    
    public float MaxX => maxX; // 声明只读属性，访问私有变量

    // 初始化限定世界位置
    void Start()
    {
        Camera mainCamera = Camera.main;

        // 获取视口左下角和右上角的世界坐标
        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f));

        // 获取视口中点的 x 坐标
        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;
    }

    /// <summary>
    /// 限制玩家移动的范围
    /// </summary>
    /// <param name="playerPosition">玩家的当前位置</param>
    /// <param name="paddingX">X 轴的边距</param>
    /// <param name="paddingY">Y 轴的边距</param>
    /// <returns>限制在可移动范围内的位置</returns>
    public Vector3 PlayerMoveablePosition(Vector3 playerPosition, float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

        return position;
    }

    /// <summary>
    /// 随机生成敌人的出生位置，位于屏幕右侧之外
    /// </summary>
    /// <param name="paddingX">X 轴的边距</param>
    /// <param name="paddingY">Y 轴的边距</param>
    /// <returns>敌人出生的随机位置</returns>
    public Vector3 RandomEnemySpawnPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = maxX + paddingX;
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    /// <summary>
    /// 在屏幕右半部分随机生成一个位置
    /// </summary>
    /// <param name="paddingX">X 轴的边距</param>
    /// <param name="paddingY">Y 轴的边距</param>
    /// <returns>屏幕右半部分的随机位置</returns>
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(middleX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    /// <summary>
    /// 随机生成一个敌人移动的目标位置，位于屏幕范围内
    /// </summary>
    /// <param name="paddingX">X 轴的边距</param>
    /// <param name="paddingY">Y 轴的边距</param>
    /// <returns>敌人移动的随机目标位置</returns>
    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using RangeAttribute = UnityEngine.RangeAttribute;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    #region FIELDS
    
    //========================================
    // [ Health Settings ]
    //========================================
    [Header("--- Health Settings ---")]
    [SerializeField] private bool regenrateHealth = true;
    [SerializeField] private float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] private float healthRegeneratePercent;
    [SerializeField] private StartsBar_HUD startsBar_HUD;
    private WaitForSeconds waitHealthRegenerateTime;
    private Coroutine healthRegenerateCoroutine;

    //========================================
    // [ Input Settings ]
    //========================================
    [Header("--- Input Settings ---")]
    [SerializeField] private PlayerInput input;

    //========================================
    // [ Movement Settings ]
    //========================================
    [Header("--- Movement Settings ---")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationTime = 3f;
    [SerializeField] private float decelerationTime = 3f;
    [SerializeField] private float tiltAngle;
    private new Rigidbody2D rigidbody;
    private Coroutine moveCoroutine;
    private float paddingX;
    private float paddingY;

    //========================================
    // [ Combat Settings ]
    //========================================
    [Header("--- Fire Settings ---")]
    [SerializeField] private GameObject projectile1;
    [SerializeField] private GameObject projectile2;
    [SerializeField] private GameObject projectile3;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] private Transform muzzleTop;
    [SerializeField] private Transform muzzleMiddle;
    [SerializeField] private Transform muzzleBottom;
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField, Range(0, 2)] private int weaponPower = 0;
    [SerializeField] private AudioData projectileLaunchSFX;
    private WaitForSeconds waitForFireInterval;
    private WaitForSeconds waitForOverdriveFireInterval;

    //========================================
    // [ Dodge Settings ]
    //========================================
    [Header("--- Dodge Settings ---")]
    [SerializeField] private AudioData dodgeSFX;
    [SerializeField] private int dodgeEnergyCost = 25;
    [SerializeField] private float maxRoll = 720f;
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);
    private bool isDodging = false;
    private float currentRoll;
    private float dodgeDuration;

    //========================================
    // [ Overdrive Settings ]
    //========================================
    [Header("--- OVERDRIVE ---")]
    [SerializeField] private GameObject projectileOverdrive;
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;
    private bool isOverdriving = false;

    //========================================
    // [ System References ]
    //========================================
    private new Collider2D collider;
    MissileSystem missile;
    readonly float slowMotionDuration = 0.5f;
    readonly float slowMotionDurationDoDge = 0.25f;
    readonly float InvincibleTime = 1f;
    private WaitForSeconds waitInvincibleTime;

    #endregion

    #region PROPERTIES
    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2;
    #endregion

    #region UNITY EVENT FUNCTIONS

    /// <summary>
    /// 初始化HUD和输入系统
    /// </summary>
    void Start()
    {
        startsBar_HUD.Initialize(health, maxHealth);
        input.EnableGameplayInput();
    }

    /// <summary>
    /// 组件获取和基础设置
    /// </summary>
    void Awake()
    {
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();
        rigidbody = GetComponent<Rigidbody2D>();
        dodgeDuration = maxRoll / rollSpeed;
        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    /// <summary>
    /// 注册输入事件
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;
        input.onLaunchMissile += OnLauchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdirveOff;
    }

    /// <summary>
    /// 注销输入事件
    /// </summary>
    [System.Obsolete]
    private void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;
        input.onLaunchMissile -= OnLauchMissile;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdirveOff;
    }
    #endregion

    #region HEALTH SYSTEM

    /// <summary>
    /// 承受伤害并触发无敌状态
    /// </summary>
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        startsBar_HUD.UpdateStats(health, maxHealth);
        if (gameObject.activeSelf)
        {
            StartCoroutine(InvincibleCoroutine());
            if (regenrateHealth)
            {
                if (healthRegenerateCoroutine != null) StopCoroutine(healthRegenerateCoroutine);
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    /// <summary>
    /// 恢复生命值
    /// </summary>
    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        startsBar_HUD.UpdateStats(health, maxHealth);
    }

    /// <summary>
    /// 玩家死亡处理
    /// </summary>
    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        startsBar_HUD.UpdateStats(health, maxHealth);
        base.Die();
    }

    /// <summary>
    /// 无敌状态协程
    /// </summary>
    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;
        yield return waitInvincibleTime;
        collider.isTrigger = false;
    }
    #endregion

    #region MOVEMENT SYSTEM

    /// <summary>
    /// 处理移动输入
    /// </summary>
    private void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed));
        StartCoroutine(MoveRangeLimitCoroutine());

        if (moveInput.y > 0) StartCoroutine(SmoothTiltCoroutine(tiltAngle));
        else if (moveInput.y < 0) StartCoroutine(SmoothTiltCoroutine(-tiltAngle));
        else StartCoroutine(SmoothTiltCoroutine(0));
    }

    /// <summary>
    /// 停止移动处理
    /// </summary>
    private void StopMove()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero));
        StopCoroutine(nameof(MoveRangeLimitCoroutine));
        StartCoroutine(SmoothTiltCoroutine(0));
    }

    /// <summary>
    /// 平滑移动协程
    /// </summary>
    private IEnumerator MoveCoroutine(float time, Vector2 moveVelocity)
    {
        float t = 0f;
        Vector2 initialVelocity = rigidbody.linearVelocity;
        while (t < time)
        {
            t += Time.fixedDeltaTime;
            rigidbody.linearVelocity = Vector2.Lerp(initialVelocity, moveVelocity, t / time);
            yield return null;
        }
        rigidbody.linearVelocity = moveVelocity;
    }

    /// <summary>
    /// 飞机倾斜效果协程
    /// </summary>
    private IEnumerator SmoothTiltCoroutine(float targetTilt)
    {
        float currentTilt = transform.eulerAngles.x;
        if (currentTilt > 180) currentTilt -= 360;

        float time = 0.2f;
        float t = 0f;
        float lerpAngle;

        while (t < time)
        {
            t += Time.deltaTime;
            lerpAngle = Mathf.Lerp(currentTilt, targetTilt, t / time);
            transform.eulerAngles = new Vector3(lerpAngle, transform.eulerAngles.y, transform.eulerAngles.z);
            yield return null;
        }
        transform.eulerAngles = new Vector3(targetTilt, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    /// <summary>
    /// 移动边界限制协程
    /// </summary>
    private IEnumerator MoveRangeLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);
            yield return null;
        }
    }
    #endregion

    #region COMBAT SYSTEM

    /// <summary>
    /// 开始持续射击
    /// </summary>
    private void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    /// <summary>
    /// 停止射击
    /// </summary>
    private void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    /// <summary>
    /// 射击协程（根据武器强度生成不同弹幕）
    /// </summary>
    IEnumerator FireCoroutine()
    {
        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile3, muzzleBottom.position);
                    break;
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
        }
    }
    #endregion

    #region ABILITY SYSTEM

    /// <summary>
    /// 闪避技能触发
    /// </summary>
    void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(nameof(DodgeCoroutine));
    }

    /// <summary>
    /// 闪避技能协程（包含无敌状态和变形效果）
    /// </summary>
    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        collider.isTrigger = true;
        currentRoll = 0f;
        TimeController.Instance.BulletTime(slowMotionDurationDoDge, slowMotionDurationDoDge);
        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);
            yield return null;
        }
        collider.isTrigger = false;
        isDodging = false;
    }

    /// <summary>
    /// 超载模式开关
    /// </summary>
    void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;
        PlayerOverdrive.on.Invoke();
    }

    /// <summary>
    /// 激活超载模式
    /// </summary>
    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    /// <summary>
    /// 关闭超载模式
    /// </summary>
    void OverdirveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    /// <summary>
    /// 导弹发射处理
    /// </summary>
    void OnLauchMissile()
    {
        missile.Launch(muzzleMiddle);
    }

    /// <summary>
    /// 获取导弹
    /// </summary>
    public void PickUpMissile()
    {
        missile.PickUp();
    }
    #endregion

    #region POWER SYSTEM

    /// <summary>
    /// 增强武器强度
    /// </summary>
    public void PowerUp()
    {
        weaponPower = Mathf.Min(++weaponPower, 2);
    }

    /// <summary>
    /// 降低武器强度
    /// </summary>
    void PowerDown()
    {
        weaponPower = Mathf.Max(--weaponPower, 0);
    }
    #endregion
}
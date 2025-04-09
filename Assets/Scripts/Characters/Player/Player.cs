using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using RangeAttribute = UnityEngine.RangeAttribute;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    #region FIELDS
    [Header("--- Health Settings ---")]
    [SerializeField] private bool regenrateHealth = true;
    [SerializeField] private float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] private float healthRegeneratePercent;
    [SerializeField] private StartsBar_HUD startsBar_HUD;

    [Header("--- Input Settings ---")]
    [SerializeField] private PlayerInput input;

    [Header("--- Movement Settings ---")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationTime = 3f;
    [SerializeField] private float decelerationTime = 3f;
    [SerializeField] private float tiltAngle;

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

    [Header("--- Dodge Settings ---")]
    [SerializeField] private AudioData dodgeSFX;
    [SerializeField] private int dodgeEnergyCost = 25;
    [SerializeField] private float maxRoll = 720f;
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("--- OVERDRIVE ---")]
    [SerializeField] private GameObject projectileOverdrive;
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;

    // 判断技能状态变量
    private bool isDodging = false;
    private bool isOverdriving = false;
    private float currentRoll;
    private float dodgeDuration;

    // 控制协程等待时间
    private WaitForSeconds waitForFireInterval;
    private WaitForSeconds waitForOverdriveFireInterval;
    private WaitForSeconds waitHealthRegenerateTime;
    private WaitForSeconds waitInvincibleTime;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    // 存储 Move 和 healthRegenerate 的协程变量
    private Coroutine moveCoroutine;
    private Coroutine healthRegenerateCoroutine;

    // 子弹时间 
    readonly float slowMotionDuration = 0.5f;
    readonly float slowMotionDurationDoDge = 0.25f;
    // 无敌时间
    readonly float InvincibleTime = 1f;

    // 设置限制区域
    private float paddingX;
    private float paddingY;
    MissileSystem missile;
    #endregion

    #region UNITY EVENT FUNCTIONS
    void Start()
    {
        startsBar_HUD.Initialize(health, maxHealth);
        input.EnableGameplayInput();  // Enable player input

    }

    void Awake()
    {

        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();
        rigidbody = GetComponent<Rigidbody2D>();
        // 子弹时间 持续时间
        dodgeDuration = maxRoll / rollSpeed;

        // 射击间隔 - 过载时间射击间隔 - 玩家恢复生命时间间隔
        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);

        // 根据玩家对象的priot来限制边界的移动
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    protected override void OnEnable()
    {
        // 在输入系统中 订阅当前类实现的所有行为函数
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

    [System.Obsolete]
    private void OnDisable()
    {
        // 在输入系统中 订阅当前类实现的所有行为函数
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

    #region PROERTIES 凋落物
    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2;

    #endregion

    #region HEALTH

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        startsBar_HUD.UpdateStats(health, maxHealth);
        if (gameObject.activeSelf)
        {
            // 无敌伤害
            StartCoroutine(InvincibleCoroutine());
            if (regenrateHealth)
            {

                if (healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }

    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        startsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        startsBar_HUD.UpdateStats(health, maxHealth);
        base.Die();
    }

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;

        yield return waitInvincibleTime;

        collider.isTrigger = false;
    }
    #endregion

    #region Movement


    private void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed));
        StartCoroutine(MoveRangeLimitCoroutine());

        if (moveInput.y > 0)
        {
            StartCoroutine(SmoothTiltCoroutine(tiltAngle));
        }
        else if (moveInput.y < 0)
        {
            StartCoroutine(SmoothTiltCoroutine(-tiltAngle));
        }
        else
        {
            StartCoroutine(SmoothTiltCoroutine(0));
        }
    }

    private void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero));
        StopCoroutine(nameof(MoveRangeLimitCoroutine));
        StartCoroutine(SmoothTiltCoroutine(0));
    }
    // 移动
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
    // 平滑倾斜角度
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
    // 限制移动
    private IEnumerator MoveRangeLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);
            yield return null;
        }
    }

    #endregion

    #region Fire

    private void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    private void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

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
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
        }
    }




    #endregion

    #region DODGE 

    void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(nameof(DodgeCoroutine));
    }

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

    #endregion

    #region OVERDRIVE
    void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    void OverdirveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    #endregion

    # region MISSILE
    void OnLauchMissile()
    {
        missile.Launch(muzzleMiddle);
    }

    public void PickUpMissile()
    {
        missile.PickUp();
    }

    #endregion

    #region WEAPON POWER

    public void PowerUp()
    {
        weaponPower = Mathf.Min(++weaponPower, 2);
    }

    void PowerDown()
    {
        //* 写法1
        // weaponPower--;
        // weaponPower = Mathf.Clamp(weaponPower, 0, 2);
        //* 写法2
        // weaponPower = Mathf.Max(weaponPower - 1, 0);
        //* 写法3
        // weaponPower = Mathf.Clamp(weaponPower, --weaponPower, 0);
        //* 写法4
        weaponPower = Mathf.Max(--weaponPower, 0);
    }

    #endregion
}

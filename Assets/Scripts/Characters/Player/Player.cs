using System.Collections;
using NUnit.Framework;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [Header("--- Health Settings ---")]
    [SerializeField] private bool regenrateHealth = true;
    [SerializeField] private float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] private float healthRegeneratePercent;
    [SerializeField] private StartsBar_HUD startsBar_HUD;

    [Header("--- Input Settings ---")]
    [SerializeField] private PlayerInput input;

    [Header("--- Movement Settings ---")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float paddingX;
    [SerializeField] private float paddingY;
    [SerializeField] private float accelerationTime = 3f;
    [SerializeField] private float decelerationTime = 3f;
    [SerializeField] private float tiltAngle;

    [Header("--- Fire Settings ---")]
    [SerializeField] private GameObject projectile1;
    [SerializeField] private GameObject projectile2;
    [SerializeField] private GameObject projectile3;
    [SerializeField] private Transform muzzleTop;
    [SerializeField] private Transform muzzleMiddle;
    [SerializeField] private Transform muzzleBottom;
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField, Range(0, 2)] private int weaponPower = 0;
    [SerializeField] private AudioData projectileLaunchSFX;

    [SerializeField] private GameObject projectileOverdrive;

    [Header("--- Dodge Settings ---")]
    [SerializeField] private AudioData dodgeSFX;
    [SerializeField] private int dodgeEnergyCost = 25;
    [SerializeField] private float maxRoll = 720f;
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("--- OVERDRIVE ---")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;


    private bool isDodging = false;
    private bool isOverdriving = false;
    private float currentRoll;
    private float dodgeDuration;

    private WaitForSeconds waitForFireInterval;
    private WaitForSeconds waitForOverdriveFireInterval;
    private WaitForSeconds waitHealthRegenerateTime;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    private Coroutine moveCoroutine;
    private Coroutine healthRegenerateCoroutine;


    void Start()
    {
        startsBar_HUD.Initialize(health, maxHealth);
        input.EnableGameplayInput();  // Enable player input

    }

    void Awake()
    {

        collider = GetComponent<Collider2D>();

        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0f;  // Disable gravity
        dodgeDuration = maxRoll / rollSpeed;

        waitForFireInterval = new WaitForSeconds(fireInterval);  // Set fire interval
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);  // Set health regeneration time
    }

    [System.Obsolete]
    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdirveOff;
    }

    [System.Obsolete]
    private void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;

        input.onOverdrive -= Overdrive;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdirveOff;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        startsBar_HUD.UpdateStats(health, maxHealth);

        if (gameObject.activeSelf && regenrateHealth)
        {
            if (healthRegenerateCoroutine != null)
            {
                StopCoroutine(healthRegenerateCoroutine);
            }
            healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        startsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        startsBar_HUD.UpdateStats(health, maxHealth);
        base.Die();
    }

    #region Movement

    [System.Obsolete]
    private void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed));
        StartCoroutine(MovePositionLimitCoroutine());

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

    [System.Obsolete]
    private void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero));
        StopCoroutine(nameof(MovePositionLimitCoroutine));
        StartCoroutine(SmoothTiltCoroutine(0));
    }

    [System.Obsolete]
    private IEnumerator MoveCoroutine(float time, Vector2 moveVelocity)
    {
        float t = 0f;
        Vector2 initialVelocity = rigidbody.velocity;

        while (t < time)
        {
            t += Time.fixedDeltaTime;
            rigidbody.velocity = Vector2.Lerp(initialVelocity, moveVelocity, t / time);
            yield return null;
        }

        rigidbody.velocity = moveVelocity;
    }

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

    private IEnumerator MovePositionLimitCoroutine()
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

        StartCoroutine(nameof(FireCoroutine));
    }

    private void StopFire()
    {
        StopCoroutine(nameof(FireCoroutine));
    }

IEnumerator FireCoroutine()
{
    while (true)
    {
        switch (weaponPower)
        {
            case 0:
                PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                break;
            case 1:
                PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleTop.position);
                PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleBottom.position);
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

    #region  OVERDRIVE
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
    }

    void OverdirveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    #endregion




}

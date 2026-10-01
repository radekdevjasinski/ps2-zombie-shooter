using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class GunController : MonoBehaviour
{
    private const string ShootSoundName = "shoot";
    private const string EnemyHitSoundName = "zombie_hit";
    private const float BloodLifetime = 5f;

    private static readonly int ShootTrigger = Animator.StringToHash("shoot");
    private static readonly Vector3 ScreenCenterViewportPoint = new Vector3(0.5f, 0.5f, 0f);

    [Header("Shoot")]
    public float cooldown;
    public Animator fpsAnimator;
    public ParticleSystem muzzleFlash;
    public float range = 100f;
    public GameObject bloodPrefab;
    public GunFlashLight gunFlashLight;
    public LayerMask hitMask;

    private Camera mainCamera;
    private FPSCamera fpsCamera;
    private ObjectPool<ParticleSystem> bloodPool;
    private WaitForSeconds bloodLifetimeWait;
    private float nextShotTime;

    void Awake()
    {
        bloodLifetimeWait = new WaitForSeconds(BloodLifetime);
        bloodPool = new ObjectPool<ParticleSystem>(
            CreateBlood,
            blood => blood.gameObject.SetActive(true),
            blood => blood.gameObject.SetActive(false),
            blood => Destroy(blood.gameObject));
    }

    void Start()
    {
        mainCamera = Camera.main;
        fpsCamera = mainCamera.GetComponent<FPSCamera>();
    }

    void Update()
    {
        if (fpsCamera.lockMovement || PauseMenu.IsGamePaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextShotTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        nextShotTime = Time.time + cooldown;
        PlayShotEffects();

        Ray ray = mainCamera.ViewportPointToRay(ScreenCenterViewportPoint);
        if (!Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            return;
        }

        EnemyAI enemy = hit.collider.GetComponentInParent<EnemyAI>();
        if (enemy == null || enemy.IsDead)
        {
            return;
        }

        enemy.Kill(hit.point, ray.direction);
        StartCoroutine(PlayBlood(hit.point));
        AudioManager.Instance.PlaySFX(EnemyHitSoundName);
    }

    private void PlayShotEffects()
    {
        fpsAnimator.SetTrigger(ShootTrigger);
        muzzleFlash.Play();
        gunFlashLight.TriggerFlash();
        AudioManager.Instance.PlaySFX(ShootSoundName);
    }

    private ParticleSystem CreateBlood()
    {
        return Instantiate(bloodPrefab).GetComponent<ParticleSystem>();
    }

    private IEnumerator PlayBlood(Vector3 position)
    {
        ParticleSystem blood = bloodPool.Get();
        blood.transform.position = position;
        blood.Play();

        yield return bloodLifetimeWait;

        bloodPool.Release(blood);
    }
}

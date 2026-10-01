using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour
{
    private const string DamageSoundName = "damage";
    private const int MaxOverlappingColliders = 128;

    [Header("PlayerHealth")]
    public float maxHP = 100;
    public Image endImage;

    [Header("Damage Settings")]
    public float damageRange = 5f;
    public float damageAmount = 1f;
    public LayerMask damageLayer;

    [Header("Regain Settings")]
    public float regainTime = 3f;
    public float regainAmount = 1f;

    [Header("Damage Sound")]
    public float damageSoundInterval = 0.5f;

    [Header("Game Over")]
    [SerializeField] private float endScreenFadeDuration = 3f;
    [SerializeField] private string menuSceneName = "Menu";

    public event Action<float> HealthFractionChanged;
    public event Action Died;

    public float HealthFraction => health.Fraction;

    private readonly Collider[] overlappingColliders = new Collider[MaxOverlappingColliders];
    private readonly HashSet<EnemyAI> attackingEnemies = new HashSet<EnemyAI>();

    private Health health;
    private PlayerMovement movement;
    private FPSCamera fpsCamera;
    private float lastDamageTime = float.NegativeInfinity;
    private float nextDamageSoundTime;
    private bool isDead;

    void Awake()
    {
        health = new Health(maxHP);
        movement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        fpsCamera = Camera.main.GetComponent<FPSCamera>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        int attackingEnemyCount = CountAttackingEnemies();
        if (attackingEnemyCount > 0)
        {
            TakeDamage(damageAmount * attackingEnemyCount * Time.deltaTime);
        }
        else if (Time.time - lastDamageTime >= regainTime)
        {
            Regenerate();
        }
    }

    private int CountAttackingEnemies()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, damageRange, overlappingColliders, damageLayer);
        attackingEnemies.Clear();
        for (int i = 0; i < colliderCount; i++)
        {
            EnemyAI enemy = overlappingColliders[i].GetComponentInParent<EnemyAI>();
            if (enemy != null && !enemy.IsDead)
            {
                attackingEnemies.Add(enemy);
            }
        }
        return attackingEnemies.Count;
    }

    private void TakeDamage(float amount)
    {
        health.TakeDamage(amount);
        lastDamageTime = Time.time;
        HealthFractionChanged?.Invoke(health.Fraction);
        PlayDamageSound();

        if (health.IsDepleted)
        {
            Die();
        }
    }

    private void PlayDamageSound()
    {
        if (Time.time < nextDamageSoundTime)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(DamageSoundName);
        nextDamageSoundTime = Time.time + damageSoundInterval;
    }

    private void Regenerate()
    {
        if (health.Current >= health.Max)
        {
            return;
        }

        health.Heal(regainAmount * Time.deltaTime);
        HealthFractionChanged?.Invoke(health.Fraction);
    }

    private void Die()
    {
        isDead = true;
        movement.movementEnabled = false;
        fpsCamera.lockMovement = true;
        Died?.Invoke();
        StartCoroutine(ShowEndScreenThenLoadMenu());
    }

    private IEnumerator ShowEndScreenThenLoadMenu()
    {
        endImage.gameObject.SetActive(true);
        Color endColor = endImage.color;

        float elapsedTime = 0f;
        while (elapsedTime < endScreenFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            endColor.a = Mathf.Clamp01(elapsedTime / endScreenFadeDuration);
            endImage.color = endColor;
            yield return null;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(menuSceneName);
    }
}

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySound : MonoBehaviour
{
    private const float PlayChance = 0.5f;
    private const float MinHearingDistance = 1f;
    private const float MaxHearingDistance = 30f;

    public List<AudioClip> randomClips;
    public float minInterval = 7f;
    public float maxInterval = 15f;
    public EnemyAI enemyAI;

    private AudioSource audioSource;
    private float nextSoundTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = MinHearingDistance;
        audioSource.maxDistance = MaxHearingDistance;
    }

    void OnEnable()
    {
        enemyAI.Died += HandleEnemyDied;
    }

    void OnDisable()
    {
        enemyAI.Died -= HandleEnemyDied;
    }

    void Update()
    {
        if (Time.time < nextSoundTime)
        {
            return;
        }

        nextSoundTime = Time.time + Random.Range(minInterval, maxInterval);
        if (Random.value < PlayChance)
        {
            PlayRandomClip();
        }
    }

    private void PlayRandomClip()
    {
        if (randomClips.Count == 0 || audioSource.isPlaying)
        {
            return;
        }

        audioSource.PlayOneShot(randomClips[Random.Range(0, randomClips.Count)]);
    }

    private void HandleEnemyDied(EnemyAI deadEnemy)
    {
        audioSource.Stop();
        enabled = false;
    }
}

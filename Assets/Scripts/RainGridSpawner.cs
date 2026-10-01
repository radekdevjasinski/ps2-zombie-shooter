using System.Collections.Generic;
using UnityEngine;

public class RainGridSpawner : MonoBehaviour
{
    private const float RainHeight = 25f;
    private const float ActivationCheckInterval = 0.5f;

    public GameObject rainParticlePrefab;
    public GameObject player;
    public float gridSpacing = 20f;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float activationDistance = 20f;

    private readonly List<ParticleSystem> rainParticleSystems = new List<ParticleSystem>();
    private float nextActivationCheckTime;

    void Start()
    {
        GenerateGrid();
    }

    void Update()
    {
        if (Time.time < nextActivationCheckTime)
        {
            return;
        }

        nextActivationCheckTime = Time.time + ActivationCheckInterval;
        UpdateActiveRain();
    }

    private void GenerateGrid()
    {
        Vector3 center = new Vector3(gridWidth * gridSpacing / 2, 0, gridHeight * gridSpacing / 2);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(x * gridSpacing, RainHeight, z * gridSpacing) - center;
                rainParticleSystems.Add(CreateStoppedRain(position));
            }
        }
    }

    private ParticleSystem CreateStoppedRain(Vector3 position)
    {
        GameObject rainObject = Instantiate(rainParticlePrefab, position, Quaternion.identity, transform);
        ParticleSystem rainParticleSystem = rainObject.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule main = rainParticleSystem.main;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        rainParticleSystem.Stop();

        return rainParticleSystem;
    }

    private void UpdateActiveRain()
    {
        Vector3 playerPosition = player.transform.position;
        float sqrActivationDistance = activationDistance * activationDistance;

        foreach (ParticleSystem rainParticleSystem in rainParticleSystems)
        {
            float sqrDistance = (playerPosition - rainParticleSystem.transform.position).sqrMagnitude;
            bool shouldPlay = sqrDistance <= sqrActivationDistance;

            if (shouldPlay && !rainParticleSystem.isPlaying)
            {
                rainParticleSystem.Play();
            }
            else if (!shouldPlay && rainParticleSystem.isPlaying)
            {
                rainParticleSystem.Stop();
            }
        }
    }
}

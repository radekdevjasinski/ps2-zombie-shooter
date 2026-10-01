using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class KillCount : MonoBehaviour
{
    [SerializeField] private EnemiesSpawner enemiesSpawner;

    private TMP_Text text;
    private int count;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        ShowCount();
    }

    void OnEnable()
    {
        enemiesSpawner.EnemyKilled += AddKill;
    }

    void OnDisable()
    {
        enemiesSpawner.EnemyKilled -= AddKill;
    }

    private void AddKill()
    {
        count++;
        ShowCount();
    }

    private void ShowCount()
    {
        text.text = count.ToString();
    }
}

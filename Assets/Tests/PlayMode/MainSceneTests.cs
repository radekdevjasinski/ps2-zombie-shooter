using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MainSceneTests
{
    private const string MainSceneName = "Main";
    private const float SmokeRunSeconds = 5f;

    [UnitySetUp]
    public IEnumerator LoadMainScene()
    {
        yield return SceneManager.LoadSceneAsync(MainSceneName);
    }

    [UnityTest]
    [Timeout(60000)]
    public IEnumerator MainScene_RunsWithoutLoggingErrors()
    {
        yield return new WaitForSeconds(SmokeRunSeconds);

        Assert.IsNotNull(Object.FindFirstObjectByType<EnemyAI>(), "Spawner should have spawned an enemy.");
    }

    [UnityTest]
    [Timeout(60000)]
    public IEnumerator KilledEnemy_IncrementsKillCount()
    {
        EnemyAI enemy = null;
        yield return new WaitUntil(() => (enemy = Object.FindFirstObjectByType<EnemyAI>()) != null);

        enemy.Kill(enemy.transform.position, Vector3.forward);
        yield return null;

        TMP_Text killCountText = Object.FindFirstObjectByType<KillCount>().GetComponent<TMP_Text>();
        Assert.IsTrue(enemy.IsDead);
        Assert.AreEqual("1", killCountText.text);
    }
}

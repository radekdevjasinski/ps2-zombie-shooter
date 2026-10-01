using System;
using NUnit.Framework;

public class HealthTests
{
    [Test]
    public void NewHealth_StartsFull()
    {
        Health health = new Health(100f);

        Assert.AreEqual(100f, health.Current);
        Assert.AreEqual(1f, health.Fraction);
        Assert.IsFalse(health.IsDepleted);
    }

    [Test]
    public void Constructor_RejectsNonPositiveMax()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Health(0f));
    }

    [Test]
    public void TakeDamage_ReducesCurrentHealth()
    {
        Health health = new Health(100f);

        health.TakeDamage(25f);

        Assert.AreEqual(75f, health.Current);
        Assert.AreEqual(0.75f, health.Fraction);
    }

    [Test]
    public void TakeDamage_BeyondCurrentHealth_ClampsAtZeroAndDepletes()
    {
        Health health = new Health(100f);

        health.TakeDamage(150f);

        Assert.AreEqual(0f, health.Current);
        Assert.IsTrue(health.IsDepleted);
    }

    [Test]
    public void Heal_DoesNotExceedMax()
    {
        Health health = new Health(100f);
        health.TakeDamage(10f);

        health.Heal(50f);

        Assert.AreEqual(100f, health.Current);
    }
}

using System;
using UnityEngine;

public class Health
{
    public float Max { get; }
    public float Current { get; private set; }

    public bool IsDepleted => Current <= 0f;
    public float Fraction => Current / Max;

    public Health(float max)
    {
        if (max <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(max), max, "Max health must be positive.");
        }

        Max = max;
        Current = max;
    }

    public void TakeDamage(float amount)
    {
        Current = Mathf.Max(0f, Current - amount);
    }

    public void Heal(float amount)
    {
        Current = Mathf.Min(Max, Current + amount);
    }
}

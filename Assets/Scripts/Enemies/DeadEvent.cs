using UnityEngine;
using System;

public static class DeadEvent
{
    public static event Action OnPlayerDead;

    public static void PlayerDead()
    {
        OnPlayerDead?.Invoke();
    }
}

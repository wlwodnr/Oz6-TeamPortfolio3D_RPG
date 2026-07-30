using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnHitManager : MonoBehaviour
{
    public static OnHitManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private Dictionary<EffectType, IOnHitHandler> _handlers = new Dictionary<EffectType, IOnHitHandler>()
    {
        { EffectType.LifeSteal, new LifeStealHandler() }
    };

    public void ApplyEffect(EffectType type, PlayerModel player, IGameObjectEntity target, int effectValue, int stack)
    {
        if (_handlers.TryGetValue(type, out var handler))
        {
            handler.Execute(player, target, effectValue, stack); 
        }
    }
}
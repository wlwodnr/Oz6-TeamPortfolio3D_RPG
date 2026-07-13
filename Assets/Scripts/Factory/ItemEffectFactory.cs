using NUnit.Framework.Internal;
using UnityEngine;

public static class ItemEffectFactory 
{
    public static IHitEffect Create(ItemBase itemData, EffectType type)
    {
        if(itemData is OnHitEffectItem item)
        {
            switch(type)
            {
                case EffectType.LifeSteal:
                    var effect = new LifeStealEffect(item.value);
                    return effect;
                default:
                    return null;
            }
        }
        return null;
    }
}

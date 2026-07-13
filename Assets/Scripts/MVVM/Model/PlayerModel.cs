using System.Collections.Generic;
using UnityEngine;

public class PlayerModel 
{
    private Stats _stats;

    // itemId - 갯수
    private Dictionary<string,int> _inventory = new Dictionary<string, int>();    
    private Dictionary<string,int> _equipInventory = new Dictionary<string,int>();

    //itemId - 데이터
    private Dictionary<string, StatUpItem> _statItems = new Dictionary<string, StatUpItem>();  //추후 삭제가능성 높음
    private Dictionary<string, IHitEffect> _activeHitEffects = new Dictionary<string, IHitEffect>();

    public void Additem(string itemId)
    {
        var itemData = ItemDataBase.GetItemData(itemId);
        if(itemData is IEquipable equipable)
        {
            switch(equipable.EffectType)
            {
                case EffectType.StatUp:
                    _stats.AddModifier(itemId);
                    break;
                case EffectType.LifeSteal:

            }
        }
    }


}

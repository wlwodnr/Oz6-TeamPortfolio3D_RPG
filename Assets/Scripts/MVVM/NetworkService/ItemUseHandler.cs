using System.Collections.Generic;
using UnityEngine;

public class ItemUseHandler
{
    public void UseItemFunction(string itemUseType, List<string> useItemParamList)
    {
        if (useItemParamList == null || useItemParamList.Count == 0)
        {
            return;
        }

        if (itemUseType == "RandomItemBox")
        {

        }
        else if (itemUseType == "StatChangeAtk")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                int statChangeVal = int.Parse(str);
                //var playerComponent = GetLocalPlayer();
                //playerComponent.AddAtk(statChangeVal);
            }

        }
        else if (itemUseType == "StatChangeHp")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                int statChangeVal = int.Parse(str);
                //var playerComponent = GetLocalPlayer();
                //playerComponent.AddHp(statChangeVal);
            }
        }
        else if (itemUseType == "SummonMonster")
        {
            if (useItemParamList.Count > 0)
            {
                string str = useItemParamList[0];
                var strArr = str.Split(":");
                if (strArr.Length > 1)
                {
                    string monsterDataId = strArr[0];
                    int monsterSummonCount = int.Parse(strArr[1]);

                    for (int i = 0; i < monsterSummonCount; i++)
                    {
                        //var playerComponent = GetLocalPlayer();
                        //DaniTechGameObjectManager.Inst.CreateMonsterObject(monsterDataId, playerComponent.transform).Forget();
                    }
                }
            }


        }
    }
}

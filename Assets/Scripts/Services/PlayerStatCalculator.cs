using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatCalculator
{
    private PlayerModel _playerModel;
    public PlayerStatCalculator(PlayerModel playerModel)
    {
        _playerModel = playerModel;
    }

    public List<float> SkillMultiAtkCalculate(List<float> list)
    {
        float playerAtk = _playerModel.GetStatValue(StatType.AttackPower);
        List<float> atkList = new List<float>();
        foreach(var multi in list)
        {
            atkList.Add(playerAtk *  multi);
        }
        return atkList;
    }
}

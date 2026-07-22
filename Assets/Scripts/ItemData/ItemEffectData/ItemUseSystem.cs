using System.Collections.Generic;
using UnityEngine;

namespace ItemUseSystem
{
    public enum ItemUseType
    {
        None,
        RandomItemBox,
        StatChangeAtk,
        StatChangeHp,
        StatChangeSpeed    }

    public interface IItemEffect
    {
        void Apply(GameObject target, List<string> parameters);
    }

    public class StatChangeAtkEffect : IItemEffect
    {
        public void Apply(GameObject target, List<string> parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            if (int.TryParse(parameters[0], out int atkValue))
            {
                // 대상 공격력 변경 처리
                // if (target != null && target.TryGetComponent<IStatHandler>(out var stat))
                // {
                //     stat.AddAtk(atkValue);
                // }
            }
        }
    }

    public class StatChangeHpEffect : IItemEffect
    {
        public void Apply(GameObject target, List<string> parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            if (int.TryParse(parameters[0], out int hpValue))
            {
                // 대상 체력 회복/변경 처리
                // if (target != null && target.TryGetComponent<IHealable>(out var healable))
                // {
                //     healable.Heal(hpValue);
                // }
            }
        }
    }

    public class StatChangeSpeedEffect : IItemEffect
    {
        public void Apply(GameObject target, List<string> parameters)
        {
            if (parameters == null || parameters.Count == 0) return;

            if (int.TryParse(parameters[0], out int hpValue))
            {
                // 대상 체력 회복/변경 처리
                // if (target != null && target.TryGetComponent<IHealable>(out var healable))
                // {
                //     healable.Heal(hpValue);
                // }
            }
        }
    }
}

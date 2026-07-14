using System;
using UnityEngine;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    private int _currentHp;
    private bool _isDead;
    private MonsterData _monsterData;

    public bool IsDead {  get { return _isDead; } }
    public int CurrentHp { get { return _currentHp; } }

    public event Action OnDeadEvent;

    public void InitStatus(MonsterData monsterData)
    {
        _monsterData = monsterData;
        _currentHp = _monsterData.BaseHp;
        _isDead = false;

    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        //TO Do: GameUTil 만들기 혹은 연동 기다리기
        //int finalDamage = GameUtil.CalcCharacterFinalDamage();

        //_currentHP -= finalDamage;
        //Debug.Log($"[{gameObject.name}] 피격당함! 최종 데미지: {finalDamage}, 남은 HP: {_currentHp} (공격자: { damageInfo.Attacker.name})");

        //TO DO:피격 애니메이션 및 이펙트 처리 요청
        //



        if (_currentHp <= 0)
        {
            SetDead();
        }

    }

    private void SetDead()
    {
        _isDead = true;
        _currentHp = 0;
        Debug.Log($"[{gameObject.name}] HP가 0 이하가 되어 사망 처리되었습니다.");

        OnDeadEvent?.Invoke();
    }

    public void ResetStatus()
    {
        _isDead = false;
        if(_monsterData != null)
        {
            _currentHp = _monsterData.BaseHp;

        }
        OnDeadEvent = null;
    }


}

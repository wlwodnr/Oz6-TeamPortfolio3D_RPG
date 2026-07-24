using UnityEngine;

public class PlayerEntity : MonoBehaviour, IGameObjectEntity, IDamageable
{
    [SerializeField] private int _instanceId = -1;
    [SerializeField] private string _playerDataId;
    private bool _isDead;
    public bool IsDead { get { return _isDead; } }

    public int InstanceId
    {
        get 
        { 
            return _instanceId; 
        }
    }

    public string PlayerDataId
    {
        get 
        { 
            return _playerDataId; 
        }
    }

    public void InitEntity(int instanceId, string dataId)
    {
        _instanceId = instanceId;
        _playerDataId = dataId;

        gameObject.SetActive(true);
    }

    public void ResetEntity()
    {
        _instanceId = -1;
        _playerDataId = string.Empty;
    }
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        var playerService = NetworkManager.Inst.LocalPlayerService;
        float playerDefense = playerService.GetPlayerDefense();
        float dmg = damageInfo.BaseDamage;
        if(damageInfo.IsCritical)
        {
            dmg = dmg * 2;
        }

        dmg = dmg * (100 / (100 + playerDefense));

        playerService.RequestDamagePlayerHp(dmg);
        // 플레이어가 사망시, PlayerModel에서 Service로 이벤트or메서드 호출로 알리고 Serivce에서 사망처리 하기
    }
}
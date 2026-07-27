using UnityEngine;

public class BossEntity : MonoBehaviour
{
    [SerializeField] private EnemyStatus Status_Boss;

    public bool IsDead
    {
        get
        {
            return Status_Boss != null && Status_Boss.IsDead;
        }
    }

    private void Awake()
    {
        if (Status_Boss == null)
        {
            Status_Boss = GetComponent<EnemyStatus>();
        }
        if (Status_Boss == null)
        {
            Status_Boss = GetComponentInChildren<EnemyStatus>(true);
        }
        if (Status_Boss == null)
        {
            Debug.LogWarning($"BossEntity: [{gameObject.name}] EnemyStatus가 없어 보스 사망 상태를 확인할 수 없습니다.", this);
        }
    }
}

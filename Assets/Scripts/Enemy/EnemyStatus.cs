using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyStatus : MonoBehaviour, IDamageable
{
    //빌드용 임시 체력
    [SerializeField] private int _temporaryMaxHp = 30;

    [Header("피격 피드백")]
    [SerializeField] private List<Renderer> _hitFeedbackRendererList = new List<Renderer>();
    [SerializeField] private Color _hitFeedbackColor = Color.red;
    [Min(0f)]
    [SerializeField] private float _hitFeedbackDuration = 0.12f;

    private static readonly int BaseColorPropertyId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");

    private int _currentHp;
    private bool _isDead;
    private MonsterData _monsterData;

    private int _enemyAttack;
    private float _enemyMoveSpeed;
    private float _detectRange;
    private float _attackRange;
    private float _stopDistance;
    private readonly List<HitFeedbackMaterialState> _hitFeedbackMaterialStateList = new List<HitFeedbackMaterialState>();
    private CancellationTokenSource _hitFeedbackCancellationTokenSource;

    public bool IsDead {  get { return _isDead; } }
    public int CurrentHp { get { return _currentHp; } }
    public int MaxHp { get { return _monsterData != null ? _monsterData.BaseHp : _temporaryMaxHp; } }

    public int BaseAttack { get { return _enemyAttack; } }
    public float MoveSpeed { get { return _enemyMoveSpeed; } }
    public float DetectRange { get { return _detectRange; } }
    public float AttackRange { get { return _attackRange; } }
    public float StopDistance { get { return _stopDistance; } }

    public event Action OnDeadEvent;

    private void Awake()
    {
        CacheHitFeedbackMaterialStates();
    }

    private void OnEnable()
    {
        ResetStatus();
    }

    private void OnDisable()
    {
        CancelHitFeedback(true);
    }

    public void InitStatus(MonsterData monsterData)
    {
        _monsterData = monsterData;

        ResetStatus();

    }

    public void ReinforceAttack()
    {
        _enemyAttack = _enemyAttack + 20;
    }

    public void AttackPlayer()
    {
        if (GameObjectManager.Instance == null)
        {
            Debug.LogWarning("GameObjectManager.Instance가 존재하지 않습니다.");
            return;
        }

        int playerInstanceId = GameObjectManager.Instance.PlayerInstanceId;
        GameObject playerObject = GameObjectManager.Instance.GetGameObjectCanBeNull(playerInstanceId);

        Vector3 knockbackDir = Vector3.zero;
        knockbackDir = transform.forward;
        knockbackDir.y = 0f;
        float knockbackForce = 1f;
        IGameObjectEntity targetEntity = playerObject.GetComponentInParent<IGameObjectEntity>();

        float finalAtkDamage = _enemyAttack;
        int finalCalculatedDamage = Mathf.RoundToInt(finalAtkDamage);


        DamageInfo dmgInfo = new(
                finalCalculatedDamage,
                false,
                playerObject.transform.position,
                knockbackDir,
                knockbackForce,
                playerObject

            );

        GameObjectManager.Instance.RequestTakeDamage(targetEntity.InstanceId, dmgInfo);

    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead) return;

        if (damageInfo == null)
        {
            Debug.LogWarning($"[{gameObject.name}] DamageInfo가 null이어서 데미지를 처리할 수 없습니다.");
            return;
        }

        int appliedDamage = damageInfo.BaseDamage;
        if (damageInfo.IsCritical)
        {
            appliedDamage = appliedDamage * 2;
        }


        if (appliedDamage <= 0)
        {
            Debug.LogWarning(
                $"[{gameObject.name}] 유효하지 않은 데미지입니다. " +
                $"Damage: {appliedDamage}"
            );

            return;
        }

        _currentHp = Mathf.Max(0, _currentHp - appliedDamage);

        PlayHitFeedback();
        Debug.Log($"[{name}] TakeDamage() 실행 확인");
        transform.GetComponent<Rigidbody>().AddForce(damageInfo.KnockbackDir * 10f, ForceMode.Impulse);


        if (_currentHp <= 0)
        {
            SetDead();
        }

    }

    private void SetDead()
    {
        if (_isDead == true)
        {
            return;
        }

        _isDead = true;
        _currentHp = 0;
        Debug.Log($"[{gameObject.name}] HP가 0 이하가 되어 사망 처리되었습니다.");

        OnDeadEvent?.Invoke();
    }

    public void ResetStatus()
    {
        CancelHitFeedback(true);
        _isDead = false;
        if(_monsterData != null)
        {
            _currentHp = _monsterData.BaseHp;
            _attackRange = _monsterData.AttackRange;
            _detectRange = _monsterData.DetectRange;
            _enemyAttack = _monsterData.BaseAttack;
            _enemyMoveSpeed = _monsterData.MoveSpeed;
            _stopDistance = _monsterData.StopDistance;
        }
        else
        {
            _currentHp = Mathf.Max(_temporaryMaxHp);
        }
        Debug.Log($"[{gameObject.name}] 상태가 초기화되었습니다.");
    }

    public void PrepareStatusForPool()
    {
        CancelHitFeedback(true);
        _monsterData = null;
        _currentHp = 0;
        _isDead = false;
        _enemyAttack = 0;
        _enemyMoveSpeed = 0f;
        _detectRange = 0f;
        _attackRange = 0f;
        _stopDistance = 0f;
    }

    private void CacheHitFeedbackMaterialStates()
    {
        _hitFeedbackMaterialStateList.Clear();

        foreach (Renderer targetRenderer in _hitFeedbackRendererList)
        {
            if (targetRenderer == null)
            {
                continue;
            }

            Material[] sharedMaterialArray = targetRenderer.sharedMaterials;
            for (int materialIndex = 0; materialIndex < sharedMaterialArray.Length; materialIndex++)
            {
                Material sharedMaterial = sharedMaterialArray[materialIndex];
                if (sharedMaterial == null)
                {
                    continue;
                }

                bool hasBaseColor = sharedMaterial.HasProperty(BaseColorPropertyId);
                bool hasColor = sharedMaterial.HasProperty(ColorPropertyId);
                if (hasBaseColor == false && hasColor == false)
                {
                    continue;
                }

                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                targetRenderer.GetPropertyBlock(propertyBlock, materialIndex);
                Color originalBaseColor = hasBaseColor ? GetCurrentColor(propertyBlock, sharedMaterial, BaseColorPropertyId) : Color.white;
                Color originalColor = hasColor ? GetCurrentColor(propertyBlock, sharedMaterial, ColorPropertyId) : Color.white;
                _hitFeedbackMaterialStateList.Add(new HitFeedbackMaterialState(targetRenderer, materialIndex, propertyBlock, hasBaseColor, hasColor, originalBaseColor, originalColor));
            }
        }
    }

    private Color GetCurrentColor(MaterialPropertyBlock propertyBlock, Material sharedMaterial, int propertyId)
    {
        if (propertyBlock.HasColor(propertyId))
        {
            return propertyBlock.GetColor(propertyId);
        }

        return sharedMaterial.GetColor(propertyId);
    }

    private void PlayHitFeedback()
    {
        if (_hitFeedbackDuration <= 0f || _hitFeedbackMaterialStateList.Count == 0)
        {
            return;
        }

        CancelHitFeedback(false);
        SetHitFeedbackColor(_hitFeedbackColor);
        _hitFeedbackCancellationTokenSource = new CancellationTokenSource();
        RestoreHitFeedbackAfterDelayAsync(_hitFeedbackCancellationTokenSource).Forget();
    }

    private async UniTask RestoreHitFeedbackAfterDelayAsync(CancellationTokenSource cancellationTokenSource)
    {
        bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(_hitFeedbackDuration), cancellationToken: cancellationTokenSource.Token).SuppressCancellationThrow();
        if (isCanceled || _hitFeedbackCancellationTokenSource != cancellationTokenSource)
        {
            return;
        }

        _hitFeedbackCancellationTokenSource = null;
        cancellationTokenSource.Dispose();
        RestoreHitFeedbackColor();
    }

    private void CancelHitFeedback(bool restoreColor)
    {
        CancellationTokenSource cancellationTokenSource = _hitFeedbackCancellationTokenSource;
        _hitFeedbackCancellationTokenSource = null;

        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        if (restoreColor)
        {
            RestoreHitFeedbackColor();
        }
    }

    private void SetHitFeedbackColor(Color targetColor)
    {
        foreach (HitFeedbackMaterialState materialState in _hitFeedbackMaterialStateList)
        {
            if (materialState.TargetRenderer == null)
            {
                continue;
            }

            materialState.TargetRenderer.GetPropertyBlock(materialState.PropertyBlock, materialState.MaterialIndex);
            if (materialState.HasBaseColor)
            {
                materialState.PropertyBlock.SetColor(BaseColorPropertyId, targetColor);
            }

            if (materialState.HasColor)
            {
                materialState.PropertyBlock.SetColor(ColorPropertyId, targetColor);
            }

            materialState.TargetRenderer.SetPropertyBlock(materialState.PropertyBlock, materialState.MaterialIndex);
        }
    }

    private void RestoreHitFeedbackColor()
    {
        foreach (HitFeedbackMaterialState materialState in _hitFeedbackMaterialStateList)
        {
            if (materialState.TargetRenderer == null)
            {
                continue;
            }

            materialState.TargetRenderer.GetPropertyBlock(materialState.PropertyBlock, materialState.MaterialIndex);
            if (materialState.HasBaseColor)
            {
                materialState.PropertyBlock.SetColor(BaseColorPropertyId, materialState.OriginalBaseColor);
            }

            if (materialState.HasColor)
            {
                materialState.PropertyBlock.SetColor(ColorPropertyId, materialState.OriginalColor);
            }

            materialState.TargetRenderer.SetPropertyBlock(materialState.PropertyBlock, materialState.MaterialIndex);
        }
    }

    private sealed class HitFeedbackMaterialState
    {
        public Renderer TargetRenderer { get; }
        public int MaterialIndex { get; }
        public MaterialPropertyBlock PropertyBlock { get; }
        public bool HasBaseColor { get; }
        public bool HasColor { get; }
        public Color OriginalBaseColor { get; }
        public Color OriginalColor { get; }

        public HitFeedbackMaterialState(Renderer targetRenderer, int materialIndex, MaterialPropertyBlock propertyBlock, bool hasBaseColor, bool hasColor, Color originalBaseColor, Color originalColor)
        {
            TargetRenderer = targetRenderer;
            MaterialIndex = materialIndex;
            PropertyBlock = propertyBlock;
            HasBaseColor = hasBaseColor;
            HasColor = hasColor;
            OriginalBaseColor = originalBaseColor;
            OriginalColor = originalColor;
        }
    }
}






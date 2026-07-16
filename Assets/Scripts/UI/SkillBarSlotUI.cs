using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private Text _cooldownText;

    private const float HARDCODED_COOLDOWN = 10f; // 데이터 매니저 연동 후 이 줄과 관련 하드코딩 제거

    private string _skillId;
    private float _totalCooldown;
    private float _remainingCooldown;
    private bool _isOnCooldown;

    private void Awake()
    {
        SetCooldownVisible(false);
    }

    public void Initialize(string skillId)
    {
        _skillId = skillId;

        // var skillData = DataManager.Instance.GetActiveSkillData(skillId);
        // if (skillData != null)
        // {
        //     _totalCooldown = skillData.Cooldown; // 실제 필드명 확인 후 수정
        //     _iconImage.sprite = skillData.Icon;  // 아이콘 필드명도 확인 필요
        // }

        _totalCooldown = HARDCODED_COOLDOWN;
    }

    private void Update()
    {
        if (!_isOnCooldown) return;

        _remainingCooldown -= Time.deltaTime;

        if (_remainingCooldown <= 0f)
        {
            _remainingCooldown = 0f;
            _isOnCooldown = false;
            SetCooldownVisible(false);
            return;
        }

        UpdateCooldownVisual();
    }

    public void TriggerUse()
    {
        if (_isOnCooldown) return;

        _remainingCooldown = _totalCooldown;
        _isOnCooldown = true;
        SetCooldownVisible(true);
        UpdateCooldownVisual();
    }

    private void UpdateCooldownVisual()
    {
        _cooldownOverlay.fillAmount = _remainingCooldown / _totalCooldown;
        _cooldownText.text = Mathf.CeilToInt(_remainingCooldown).ToString();
    }

    private void SetCooldownVisible(bool visible)
    {
        _cooldownOverlay.gameObject.SetActive(visible);
        _cooldownText.gameObject.SetActive(visible);
    }

    public bool IsOnCooldown => _isOnCooldown;
}

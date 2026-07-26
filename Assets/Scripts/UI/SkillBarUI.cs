using UnityEngine;

public class SkillBarUI : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] _slots = new SkillSlotUI[10];

    // TODO: 실제로는 각 슬롯에 배치된 스킬 ID 목록으로 교체 (인벤토리 장착 스킬 등에서 가져오기)
    [SerializeField] private string[] _skillIds = new string[10];

    private readonly KeyCode[] _keyBindings = new KeyCode[]
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };

    private void Start()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].Initialize(_skillIds[i]);
        }
    }

    private void Update()
    {
        //게임 플레이 상태가 아닌, 일시정지나 게임 타이틀 등의 상황에서 입력 차단
        if(InputManager.Instance == null)
        {
            return;
        }
        if(InputManager.Instance.CanProcessGameplayInput == false)
        {
            return;
        }

        for (int i = 0; i < _keyBindings.Length; i++)
        {
            if (Input.GetKeyDown(_keyBindings[i]))
            {
                _slots[i].TriggerUse();
            }
        }
    }
}

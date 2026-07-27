using UnityEngine;

public class MenuLayoutUI : UIBase
{
    [Header("Buttons")]
    [SerializeField] private UIButton Btn_Inventory;
    [SerializeField] private UIButton Btn_SettingPopup;
    [SerializeField] private UIButton Btn_CloseSelf;
    private void OnEnable()
    {
        Btn_Inventory.BindOnClickButtonEvent(OnClickOpenInventory);
        Btn_SettingPopup.BindOnClickButtonEvent(OnClickOpenSettingPopup);
        Btn_CloseSelf.BindOnClickButtonEvent(OnclickCloseSelf);
    }

    private void OnClickOpenSettingPopup()
    {
        UIManager.Instance.OpenSettingUI();
    }

    private void OnClickOpenInventory()
    {
        UIManager.Instance.OpenContentUI(UIType.InventoryUI);
    }
    private void OnclickCloseSelf()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.MenuLayoutUI);
        InputManager.Instance.SetCursorAndInputState(false);
    }
}
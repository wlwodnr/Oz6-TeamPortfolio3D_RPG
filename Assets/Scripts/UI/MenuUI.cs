using UnityEngine;

public class MenuButtonUI : UIBase
{
    [Header("Buttons")]
    [SerializeField] private UIButton Btn_Menu;

    private bool _isSettingUIOpen = false;

    private void OnEnable()
    {
        Btn_Menu.BindOnClickButtonEvent(OnClickOpenMenuPopup);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenMenuPopup();
        }
    }

    private void OnClickOpenMenuPopup()
    {
        OpenMenuPopup();
        _isSettingUIOpen = true;
    }
    private void OpenMenuPopup()
    {
        if (_isSettingUIOpen == true)
        {
            UIManager.Instance.CloseSettingUI();
        }
        else
        {
            UIManager.Instance.OpenSettingUI();
        }
    }
}

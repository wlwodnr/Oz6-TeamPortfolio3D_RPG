using UnityEngine;

public class StartTitleUI : UIBase
{
    [SerializeField] private UIButton Button_Start;
    [SerializeField] private UIButton Button_End;

    private void OnEnable()
    {
        Button_Start.BindOnClickButtonEvent(OnClickStartButton);
        Button_End.BindOnClickButtonEvent(OnClickEndButton);
    }

    private void OnClickStartButton()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.StartTitleUI);

        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.PlayerProfileUI);
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.TestUI);
    }
    
    private void OnClickEndButton()
    {
        Application.Quit();
    }
}
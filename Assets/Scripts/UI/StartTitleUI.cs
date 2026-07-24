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
        //GameManager와 연동
        if(GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.StartTitleUI);

        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.PlayerProfileUI);
        UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.TestUI);

        //여기서 한번 열고난 후에 옵션에 따라 키고 끄고를 결정. 혹시 모를 버그를 방지하기 위해 두번 킨다거나 등
        UIManager.Instance.OpenTestUI();
    }
    
    private void OnClickEndButton()
    {
        Application.Quit();
    }
}
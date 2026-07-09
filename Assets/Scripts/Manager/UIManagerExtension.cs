using UnityEngine;

public enum UIRootType
{
    None = 0,
    BackGroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}

public enum UIType
{
    StartTitleUI,
    HudMainUI,
    PlayerProfileUI,
    PlayerStatInfoUI,

    TestUI
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"UIPrefabs/{uiRootType}/{uiType}";
        return path;
    }

    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.MainUI, UIType.StartTitleUI);
        //uiManager.OpenUI(UIRootType.MainUI, UIType.PlayerProfileUI);      테스트용 바로 나오게
        //uiManager.OpenUI(UIRootType.VeryFrontUI, UIType.TestUI);
    }

    public static UIBase OpenPlayerStatInfoUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.PlayerStatInfoUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return null;
        }
        return uiBase;
    }
    public static void ClosePlayerStatInfoUI(this UIManager uiManager)
    {
        uiManager.ClosePopupUI(UIType.PlayerStatInfoUI);
    }

    public static void OpenPlayerProfileUI(this UIManager uiManager, string characterDataId)
    {
        var uiBase = uiManager.OpenUI(UIRootType.MainUI, UIType.PlayerProfileUI);
        if (uiBase == null)
        {
            Debug.LogWarning("UI가 생성되지 않았습니다");
            return;
        }

        //if (uiBase is PlayerProfileUI profileUI)
        //{
        //    profileUI.RefreshCharacterUI(characterDataId);
        //}
    }

    
}
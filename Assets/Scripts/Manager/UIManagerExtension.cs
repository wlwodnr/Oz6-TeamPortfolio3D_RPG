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
        uiManager.OpenPlayerProfileUI();
        uiManager.OpenTestUI();
    }

    // ------------------------------------------------------------------
    // veryfrontUI

    public static UIBase OpenTestUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.TestUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return null;
        }
        return uiBase;
    }
    public static void CloseTestUI(this UIManager uiManager)
    {
        uiManager.ClosePopupUI(UIType.TestUI);
    }



    // ------------------------------------------------------------------
    // popupUI

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

    // ------------------------------------------------------------------
    // mainUI

    public static void OpenPlayerProfileUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenMainUI(UIType.PlayerProfileUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }
    public static void ClosePlayerProfileUI(this UIManager uiManager)
    {
        uiManager.CloseMainUI(UIType.PlayerProfileUI);
    }

    // ------------------------------------------------------------------
    // contentUI



    // ------------------------------------------------------------------
    // BackgroundUI


}
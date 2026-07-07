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
    PlayerStatInfoUI
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
        uiManager.OpenStartLoginUI();
    }

    // ------------------------------------------------------------------
    // veryfrontUI
    public static void OpenStartLoginUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenVeryFrontUI(UIType.PlayerProfileUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }
    public static void CloseStartLoginUI(this UIManager uiManager)
    {
        uiManager.CloseVeryFrontUI( UIType.PlayerProfileUI);
    }


    public static UIBase OpenLoadingUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenVeryFrontUI(UIType.PlayerStatInfoUI);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return null;
        }
        return uiBase;
    }
    public static void CloseLoadingUI(this UIManager uiManager)
    {
        uiManager.CloseVeryFrontUI(UIType.PlayerStatInfoUI);
    }

    // ------------------------------------------------------------------
    // popupUI


    // ------------------------------------------------------------------
    // mainUI


    // ------------------------------------------------------------------
    // contentUI


    // ------------------------------------------------------------------
    // BackgroundUI

}
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
    MonsterHudUI,

    DialogueUI,
    InventoryUI,
    QuestUI,
    LoadingUI,

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
    }


    public static void OpenLoadingUI(this UIManager uiManager, string characterDataId)
    {
        var uiBase = uiManager.OpenUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
        if (uiBase == null)
        {
            Debug.LogWarning("UI가 생성되지 않았습니다");
            return;
        }
    }
    public static void CloseLoadingUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
    }

    public static DialogueUI OpenDialogueUI(this UIManager uiManager)
    {
        UIBase uiBase = uiManager.OpenContentUI(UIType.DialogueUI);

        if (uiBase == null)
        {
            Debug.LogWarning("DialogueUI를 생성할수 없습니다");
            return null;
        }

        if (uiBase is DialogueUI dialogueUI)
        {
            return dialogueUI;
        }

        Debug.LogWarning("생성된 UI가 DialogueUI 타입이 아닙니다");
        return null;
    }

    public static void CloseDialogueUI(this UIManager uIManager)
    {
        uIManager.CloseContentUI(UIType.DialogueUI);
    }

    public static void OpenQuestUI(this UIManager uiManager)
    {
        uiManager.OpenUI(UIRootType.MainUI, UIType.QuestUI);
    }

    public static void CloseQuestUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.MainUI, UIType.QuestUI);
    }
}
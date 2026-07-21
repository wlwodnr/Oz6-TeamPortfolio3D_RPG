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
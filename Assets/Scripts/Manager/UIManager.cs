using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//커서를 키는 UI를 등록
//UI가 키고 꺼질 때 커서를 켜야 하는 UI일 경우 InputManager에게 커서를 키도록 매서드를 호출

public enum TestUIOption
{
    ShowTestUIAndLockCursor,
    ShowTestUIAndUnlockCursor,
    HideTestUIAndLockCursor
}


public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas Canvas_BgRoot;
    [SerializeField] Canvas Canvas_MainRoot;
    [SerializeField] Canvas Canvas_ContentRoot;
    [SerializeField] Canvas Canvas_PopupRoot;
    [SerializeField] Canvas Canvas_VeryFrontRoot;

    public static UIManager Instance { get; set; }

    // 생성과 제거에 관한 부분 -> Instancing과 가비지컬렉터와 연관이 있는 애
    private Dictionary<UIType, UIBase> _createdUIDic = new Dictionary<UIType, UIBase>();
    // 활성과 비활성에 관한 부분 -> SetActive
    private HashSet<UIType> _openedUIDic = new HashSet<UIType>();


    [Header("TestUI 옵션")]
    [SerializeField] private TestUIOption _testUIOption = TestUIOption.ShowTestUIAndLockCursor;

    private TestUIOption _appliedTestUIOption;
    private bool _isTestUIRequested = false;
    private bool _isTestUICursorActive = false;


    private void Awake()
    {
        Instance = this;

        _appliedTestUIOption = _testUIOption;
    }

    private void Start()
    {
        this.ShowStartupUIOnGameStart();
    }

    private void Update()
    {
        if(_isTestUIRequested && _appliedTestUIOption != _testUIOption)
        {
            ApplyTestUIOption();
        }
    }

    public UIBase OpenUI(UIRootType uiRootType, UIType uiType, bool isInitialHide = false)
    {
        // 딱히 요청이 있진 않고 오픈만 하면 되는 UI에서 사용
        var openedUI = GetCreatedUI(uiRootType, uiType);

        if (openedUI == null) 
        { Debug.LogError($"[UIManager] 에러 발생! {uiType} 프리팹을 로드하지 못했거나 UIBase 상속 스크립트가 없습니다.");
            return null;
        }

        bool isSetActiveOnOpen = (isInitialHide == false); // 열었을 때 기본적으로 숨겨서 열 것인지 체크
        if (_openedUIDic.Contains(uiType) == false)
        {
            openedUI.gameObject.SetActive(isSetActiveOnOpen);
            _openedUIDic.Add(uiType);

            //JU 추가: 커서를 키는 조건 - UI가 켜져있고, 해당 UI가 커서를 켜야 하는 UI인 경우 킨다.
            if(isSetActiveOnOpen && IsCursorAndInputControlUI(uiType))
            {
                InputManager.Instance?.SetCursorAndInputState(true);
            }
        }

        return openedUI;
    }

    public void CloseUI(UIRootType uiRootType, UIType uiType)
    {
        if (_openedUIDic.Contains(uiType))
        {
            var openedUi = _createdUIDic[uiType];
            //닫기 전, 실제로 활성화 된 상태였는지 체크
            bool wasActive = openedUi.gameObject.activeSelf;

            openedUi.gameObject.SetActive(false);
            _openedUIDic.Remove(uiType);

            if(wasActive && IsCursorAndInputControlUI(uiType))
            {
                InputManager.Instance?.SetCursorAndInputState(false);
            }

            if(uiType == UIType.TestUI && _isTestUICursorActive)
            {
                InputManager.Instance?.SetCursorAndInputState(false);
                _isTestUICursorActive = false;
            }
        }
    }

    private Transform GetRootTransform(UIRootType uiRootType)
    {
        Transform root = null;
        switch (uiRootType) 
        {
            case UIRootType.BackGroundUI:
                root = Canvas_BgRoot.transform;
                break;
            case UIRootType.MainUI:
                root = Canvas_MainRoot.transform;
                break;
            case UIRootType.ContentUI:
                root = Canvas_ContentRoot.transform;
                break;
            case UIRootType.PopupUI:
                root = Canvas_PopupRoot.transform;
                break;
            case UIRootType.VeryFrontUI:
                root = Canvas_VeryFrontRoot.transform;
                break;
        }
        return root;
    }

    private void CreateUI(UIRootType uiRootType, UIType uiType)
    {
        if (_createdUIDic.ContainsKey(uiType) == false)
        {
            string path = this.GetUIPath(uiRootType, uiType);
            GameObject loadedObj = (GameObject)Resources.Load(path);
            Transform root = GetRootTransform(uiRootType);
            GameObject gObj = Instantiate(loadedObj, root, false);
            if (gObj != null)
            {
                var uiBase = gObj.GetComponent<UIBase>();
                _createdUIDic.Add(uiType, uiBase);
            }
        }
    }

    private UIBase GetCreatedUI(UIRootType uiRootType, UIType uiType)
    {
        if (_createdUIDic.ContainsKey(uiType) == false)
        {
            CreateUI(uiRootType, uiType);
        }
        return _createdUIDic[uiType];
    }


    public UIBase GetOpenUI(UIRootType uiRootType, UIType uiType)
    {
        return GetCreatedUI(uiRootType, uiType);
    }

    public UIBase OpenContentUI(UIType uiType)
    {
        return OpenUI(UIRootType.ContentUI, uiType);
    }

    public UIBase OpenPopupUI(UIType uiType)
    {
        return OpenUI(UIRootType.PopupUI, uiType);
    }

    public void CloseContentUI(UIType uiType)
    {
        CloseUI(UIRootType.ContentUI, uiType);
    }

    public void ClosePopupUI(UIType uiType)
    {
        CloseUI(UIRootType.PopupUI, uiType);
    }

    private bool IsCursorAndInputControlUI(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.StartTitleUI:
            case UIType.DialogueUI:
            case UIType.InventoryUI:
            case UIType.QuestUI:
            case UIType.PlayerStatInfoUI:
                {
                    return true;
                }
            default: return false;
        }
    }

    public void OpenTestUI()
    {
        _isTestUIRequested = true;
        ApplyTestUIOption();
    }

    private void ApplyTestUIOption()
    {
        switch(_testUIOption)
        {
            case TestUIOption.ShowTestUIAndLockCursor:
                {
                    if(_openedUIDic.Contains(UIType.TestUI) == false)
                    {
                        OpenUI(UIRootType.VeryFrontUI, UIType.TestUI);
                    }

                    if(_isTestUICursorActive && InputManager.Instance != null)
                    {
                        InputManager.Instance?.SetCursorAndInputState(false);
                        _isTestUICursorActive = false;
                    }
                    break;
                }
            case TestUIOption.ShowTestUIAndUnlockCursor:
                {
                    if(_openedUIDic.Contains(UIType.TestUI) == false)
                    {
                        OpenUI(UIRootType.VeryFrontUI, UIType.TestUI);
                    }
                    if(_isTestUICursorActive == false && InputManager.Instance != null)
                    {
                        InputManager.Instance?.SetCursorAndInputState(true);
                        _isTestUICursorActive = true;
                    }
                    break;
                }
            case TestUIOption.HideTestUIAndLockCursor:
                {
                    if(_openedUIDic.Contains(UIType.TestUI))
                    {
                        CloseUI(UIRootType.VeryFrontUI, UIType.TestUI);
                    }
                    else if(_isTestUICursorActive)
                    {
                        InputManager.Instance?.SetCursorAndInputState(false);
                        _isTestUICursorActive = false;
                    }
                    break;
                }
        }
        _appliedTestUIOption = _testUIOption;

    }
}
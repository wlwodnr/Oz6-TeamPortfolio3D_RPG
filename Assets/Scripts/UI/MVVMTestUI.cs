using UnityEngine;
using UnityEngine.UI;

public class MVVMTestUI : UIBase
{
    [SerializeField] private InputField InputField_ChangeName;
    [SerializeField] private UIButton Button_GiveExp;
    [SerializeField] private UIButton Button_AddStatAtk;
    [SerializeField] private UIButton Button_AddStatHP;
    [SerializeField] private UIButton Button_AddStatMP;
    [SerializeField] private UIButton button_AddItem;
    [SerializeField] private UIButton button_Inventory;

    private void Awake()
    {
        //InputField_ChangeName.onSubmit.AddListener(OnSubmit_ChangeName);
        Button_GiveExp.BindOnClickButtonEvent(OnClick_GiveExp);
        Button_AddStatAtk.BindOnClickButtonEvent(OnClick_AddStatAtk);
        Button_AddStatHP.BindOnClickButtonEvent(OnClick_AddStatHP);
        Button_AddStatMP.BindOnClickButtonEvent(OnClick_AddStatMP);
        button_AddItem.BindOnClickButtonEvent(OnClick_AddTestItem);
        button_Inventory.BindOnClickButtonEvent(OnClick_OpenInventory);
    }

    private void OnSubmit_ChangeName(string newName)
    {
        NetworkManager.Inst.LocalPlayerService.RequestChangePlayerName(newName);
    }

    private void OnClick_GiveExp()
    {
        NetworkManager.Inst.LocalPlayerService.RequestGiveExpToLocalPlayer(30f);
    }

    private void OnClick_AddStatAtk()
    {
        //NetworkManager.Inst.LocalPlayerService.RequestAddStatAtkPower(50);
    }
    private void OnClick_AddStatHP()
    {
        NetworkManager.Inst.LocalPlayerService.RequestChangePlayerHp(10f);
    }
    private void OnClick_AddStatMP()
    {
        NetworkManager.Inst.LocalPlayerService.RequestChangePlayerMp(10f);
    }

    private void OnClick_AddTestItem()
    {
        NetworkManager.Inst.InventoryService.ReuestAddItem("Consumable_01", 1);
    }

    private void OnClick_OpenInventory()
    {
        UIManager.Instance.OpenContentUI(UIType.InventoryUI);
    }
}
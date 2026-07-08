using UnityEngine;
using UnityEngine.UI;

public class DaniTech_MVVMTestUI : UIBase
{
    [SerializeField] private UIButton Button_GiveExp;
    [SerializeField] private InputField InputField_ChangeName;

    [SerializeField] private UIButton Button_AddStatAtk;

    private void Awake()
    {
        //Button_GiveExp.BindOnClickButtonEvent(OnClick_GiveExp);
        //InputField_ChangeName.onSubmit.AddListener(OnSubmit_ChangeName);
        Button_AddStatAtk.BindOnClickButtonEvent(OnClick_AddStatAtk);
    }

    private void OnClick_GiveExp()
    {
        NetworkManager.Inst.LocalPlayerService.RequestGiveExpToLocalPlayer(30);
    }

    private void OnClick_AddStatAtk()
    {
        NetworkManager.Inst.LocalPlayerService.RequestAddStatAtk(50);
    }


    private void OnSubmit_ChangeName(string newName)
    {
        NetworkManager.Inst.LocalPlayerService.RequestChangePlayerName(newName);
    }
}
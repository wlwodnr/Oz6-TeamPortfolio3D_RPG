using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "ItemEffect/HealEffect")]
public class HealEffect : ItemEffect
{
    public int HealAmount;
    public override void Apply()
    {
        // 회복 로직 추가하기
        NetworkManager.Inst.LocalPlayerService.RequestChangePlayerHp(HealAmount);
    }
}
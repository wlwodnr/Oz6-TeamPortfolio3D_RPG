using UnityEngine;

public interface IDamageable
{
    bool IsDead {  get; }

    void TakeDamage(DamageInfo damageInfo);
}

public class DamageInfo
{
    public int BaseDamage;
    public bool IsCritical;
    public Vector3 HitPoint;
    public Vector3 KnockbackDir;
    public float KnockbackForce;
    public GameObject Attacker;

    public DamageInfo(int basedamage, bool isCritical, Vector3 hitPoint, Vector3 knockbackDir, float knockbackForce, GameObject attacker)
    {
        BaseDamage = basedamage;
        IsCritical = isCritical;
        HitPoint = hitPoint;
        KnockbackDir = knockbackDir;
        KnockbackForce = knockbackForce;
        Attacker = attacker;
    }
}

using UnityEngine;

public class MeleeWeapon : WeaponSystem
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger entered");
        OnWeaponDamage(other.gameObject);
    }
}

using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] private int _attack;

    [SerializeField] private int _physicLayer;
    [SerializeField] private GameObject _rootOwner;


    private void Start()
    {
        _rootOwner = gameObject.transform.root.gameObject;
        _physicLayer = _rootOwner.layer;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer != _physicLayer) 
        {
            Debug.Log("!!!!!!!!!!! LET's GOOOOO !!!!!!!!!!!");
            int playerAttack = 0;
            if (_rootOwner.TryGetComponent<I_Stat>(out I_Stat stats))
            {
                playerAttack = (int)stats.GetAttack();
            }
            Debug.Log("Player's attack: " + playerAttack);

            _rootOwner.GetComponent<DamageDeallerComponent>()?.Damage(other.gameObject, 
                new Struct_DamageData { 
                    DamageAmount= _attack + playerAttack,
                    DamageType = Enum_DamageTypes.Physic,
                    Responce = Enum_DamageResponses.NoResponse,
                    isBlockable = true,
                    isInneviåtable = false
                });
        }
    }
}

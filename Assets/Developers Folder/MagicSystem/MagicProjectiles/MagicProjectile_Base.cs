using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MagicProjectile_Base : MonoBehaviour
{
    public float CastTime { get; protected set; }
    float CastTimer;
    public int ManaCost { get; protected set; }
    
    Vector3 _direction;
    private float _speed;
    
    private Struct_DamageData _damageData;
    
    private GameObject _owner;
    
    Vector3 _scale;

    private float TimeToLive;
    private float creationTime;


   
    public void Initialize(GameObject owner, float castTime, int manaCost, Vector3 direction, 
        float speed, Struct_DamageData damageData,  float TTL=10f)
    {
        CastTime = castTime;
        ManaCost = manaCost;
        _direction = direction;
        _speed = speed;
        _damageData = damageData;
        
        
        _owner = owner;
        
        CastTimer = Time.time;

        _scale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        
        TimeToLive = TTL;
        creationTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (Time.time-creationTime >= TimeToLive) Destroy(gameObject);
        if (Time.time-CastTimer >= CastTime)
        {
            transform.localScale = _scale;
            transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.localScale = _scale * ((Time.time - CastTimer)/ CastTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EntityCoreSystem entityCoreSystem))
        {
            _owner.GetComponent<EntityCoreSystem>().GetDamageCalculationSystem().Damage(_owner, 
                other.gameObject, _damageData);
            GetComponent<AudioSource>().Play();//
            Destroy(gameObject);
            
        }
    }

  
}

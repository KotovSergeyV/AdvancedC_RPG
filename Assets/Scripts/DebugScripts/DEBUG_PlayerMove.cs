using System.Collections;
using UnityEngine;

public class DEBUG_PlayerMove : MonoBehaviour, I_Stat
{

    [SerializeField] bool _stunFlag = false;
    [SerializeField] Vector3 _direction = new Vector3(1, 0, 0);
    [SerializeField] int _speed = 5;

    [Space]
    [SerializeField] int _defence = 5;
    [SerializeField] int _agility = 5;
    [SerializeField] int _attack = 5;
    [SerializeField] int _luck = 5;
    


    [SerializeField] I_EntityStates _states;

    public int GetAgility() { return _agility; }
    public int GetAttack() { return _attack; }
    public int GetLuck() { return _luck; }
    public int GetDefence() { return _defence; }


    void Start()
    {
        _states = gameObject.GetComponent<I_EntityStates>();
        _states.OnStateChanged += HandleStateChange;
    }

    private void HandleStateChange(Enum_EntityStates newState)
    {
        switch (newState)
        {
            case Enum_EntityStates.Stunned:
                StartCoroutine(StunRoutine(3f));
                break;

            case Enum_EntityStates.SmallStunned:
                StartCoroutine(StunRoutine(1f));
                break;
            default:
                break;
        }
    }

    private IEnumerator StunRoutine(float stunDuration)
    {
        Debug.Log("Entity is stunned!");
        _stunFlag = true;

        yield return new WaitForSeconds(stunDuration);
        _states.SetEntityState(Enum_EntityStates.Idle);
        _stunFlag = false;
        
    }

    private void FixedUpdate()
    {
        if (!_stunFlag) { Move(); }
    }

    void Move()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }
}

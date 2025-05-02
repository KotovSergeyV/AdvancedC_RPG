using UnityEngine;

public class CivilianAI : EnemyAIBase
{
    private new void Start()
    {
        base.Start();
        _currentState = AI_States.Idle;
        isFriendly = true;
    }

    private new void Update()
    {
        base.Update();

        switch (_currentState)
        {
            case AI_States.Idle:
                StopMoving();
                break;
            case AI_States.Fallback:
                RunBack();
                break;
            case AI_States.Dead:
                Dead();
                break;
        }
    }

}

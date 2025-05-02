using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense_Sight : MonoBehaviour
{
    public event Action playerEnterInRange;
    public event Action playerExitOutOfRange ;
    public event Action playerLost;

    [SerializeField] float rangeOfView = 10f;
    [SerializeField] float fieldOfViewAngle = 110f;
    [SerializeField] Transform player;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] bool canSee;

    BehaviorController behaviourController = new BehaviorController();

    List<Instruction> BehaviorInstructions = new List<Instruction>(); 

    private void Start()
    {
        playerEnterInRange += delegate { };
        playerExitOutOfRange += delegate { };

        Action[] a = new Action[] { See };
        Action[] b = new Action[] { NoSee };
        Action[] ca = new Action[] { Lost1 };
        Action[] cb = new Action[] { Lost2 };

        BehaviorTask a1 = new BehaviorTask();
        a1.Initialize(Priority.High, a);
        BehaviorTask b1 = new BehaviorTask();
        b1.Initialize(Priority.Low, b);

        BehaviorTask c1 = new BehaviorTask();
        c1.Initialize(Priority.Basic, ca);
        BehaviorTask_Delayed c2 = new BehaviorTask_Delayed();
        c2.Initialize(Priority.Basic, 3, cb);
        BehaviorTaskSequence c_seq = new BehaviorTaskSequence();
        c_seq.Initialize(new List<IBehaviorNode>() { c1, c2 });

        //  (callback) => trigger event += callback. Only like this. No other way.
        BehaviorInstructions.Add(new Instruction((callback) => playerEnterInRange += callback, a1));
        BehaviorInstructions.Add(new Instruction((callback) => playerExitOutOfRange += callback, b1));
        BehaviorInstructions.Add(new Instruction((callback) => playerLost += callback, c_seq));

        behaviourController.Initialize(BehaviorInstructions);

    }

    private void Update()
    {
        if (IsPlayerInSight())
        {
            if (!canSee)
            {
                canSee = true;
                playerEnterInRange.Invoke();
            }
        }
        else
        {
            if (canSee)
            {
                canSee = false;
                playerExitOutOfRange.Invoke();
                StartCoroutine(LostTimeCount());
            }
        }
    }

    bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > rangeOfView) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);
        if (angle < fieldOfViewAngle / 2f)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstructionMask))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator LostTimeCount()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("playerLost.Invoke");
        playerLost.Invoke();
    }

    void See()
    {
        Debug.Log("I see you!!!");
    }
    void NoSee()
    {
        Debug.Log("WHERE ARE YOU, SCUM?!!");
    }

    void Lost1()
    {
        Debug.Log("Well, another one has escaped...");
    }
    void Lost2()
    {
        Debug.Log("Argh. My dinner escaped again...");
    }

}

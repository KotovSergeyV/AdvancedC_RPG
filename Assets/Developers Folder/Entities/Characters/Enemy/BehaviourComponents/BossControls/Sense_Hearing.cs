using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense_Hearing : MonoBehaviour
{
    public event Action playerEnterInHearRange;
    public event Action playerExitOutOfHearRange ;
    public event Action playerLost;


    [SerializeField] private float hearRange;
    [SerializeField] private Transform player;
    [SerializeField] private bool isHearing;

    BehaviorController behaviourController = new BehaviorController();

    List<Instruction> BehaviorInstructions = new List<Instruction>(); 

    private void Start()
    {
        playerEnterInHearRange += delegate { };
        playerExitOutOfHearRange += delegate { };

        Action[] a = new Action[] { Hear };
        Action[] b = new Action[] { NoHear };
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
        BehaviorInstructions.Add(new Instruction((callback) => playerEnterInHearRange += callback, a1));
        BehaviorInstructions.Add(new Instruction((callback) => playerExitOutOfHearRange += callback, b1));
        BehaviorInstructions.Add(new Instruction((callback) => playerLost += callback, c_seq));

        behaviourController.Initialize(BehaviorInstructions);


    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= hearRange && !isHearing) 
        {
            isHearing = !isHearing;
            playerEnterInHearRange.Invoke();
    
        }
        else if (Vector3.Distance(transform.position, player.position) > hearRange && isHearing)
        {
            isHearing = !isHearing;
            playerExitOutOfHearRange.Invoke();

            StartCoroutine(LostTimeCount());
        }
    }

    IEnumerator LostTimeCount()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("playerLost.Invoke");
        playerLost.Invoke();
    }

    void Hear()
    {
        Debug.Log("I hear you, trash!!!");
    }
    void NoHear()
    {
        Debug.Log("Silence! Finnaly!");
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Sense_Sight : MonoBehaviour
{
    public event Action playerEnterInRange;
    public event Action playerExitOutOfRange ;

    public Action onSeeAction;
    public Action onNonSeeAction;

    [SerializeField] float rangeOfView;
    [SerializeField] Transform player;
    [SerializeField] bool canSee;

    BehaviorController behaviourController = new BehaviorController();

    List<Instruction> BehaviorInstructions = new List<Instruction>(); 

    private void Start()
    {
        playerEnterInRange += delegate { };
        playerExitOutOfRange += delegate { };
        onSeeAction += See;
        onNonSeeAction += NoSee;

        Action[] a = new Action[] { onSeeAction };
        Action[] b = new Action[] { onNonSeeAction };

        BehaviorTask a1 = new BehaviorTask();
        a1.Initialize(Priority.High, a);
        BehaviorTask b1 = new BehaviorTask();
        b1.Initialize(Priority.Low, b);

        //  (callback) => trigger event += callback. Only like this. No other way.
        BehaviorInstructions.Add(new Instruction((callback) => playerEnterInRange += callback, a1));
        BehaviorInstructions.Add(new Instruction((callback) => playerExitOutOfRange += callback, b1));

        behaviourController.Initialize(BehaviorInstructions);


    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= rangeOfView && !canSee) 
        {
            canSee = !canSee;
            playerEnterInRange.Invoke();
    
        }
        else if (Vector3.Distance(transform.position, player.position) > rangeOfView && canSee)
        {
            canSee = !canSee;
            playerExitOutOfRange.Invoke();
        }
    }

    void See()
    {
        Debug.Log("I see you!!!");
    }
    void NoSee()
    {
        Debug.Log("WHERE ARE YOU, SCUM?!!");
    }

}

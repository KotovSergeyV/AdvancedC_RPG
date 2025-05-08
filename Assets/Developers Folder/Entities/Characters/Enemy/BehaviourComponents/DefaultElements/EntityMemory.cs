using System;
using System.Threading.Tasks;
using UnityEngine;

public class EntityMemory
{
    private bool _memoWasOverwritten; 
    private bool _memoIsHolding; 
    public async void WriteMemory(Action action, float duration)
    {
        if (_memoIsHolding) { _memoWasOverwritten = true; }

        _memoIsHolding = true;

        await Task.Delay((int)(duration*1000));
        if (!_memoWasOverwritten)
        {
            action.Invoke();
            Debug.Log("forgot");
        }
        else
        {
            _memoWasOverwritten = false;
        }
    }

}

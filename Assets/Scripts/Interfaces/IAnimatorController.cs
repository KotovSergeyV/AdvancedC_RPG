using UnityEngine;

internal interface IAnimatorController
{
    public void PlayAnimation(string animationName, bool state);
    public void TriggerAnimation(string animationName);
    public void SetFloatAnimation(float x, float y);
}
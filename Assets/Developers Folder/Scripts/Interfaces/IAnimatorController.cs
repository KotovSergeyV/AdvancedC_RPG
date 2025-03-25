using UnityEngine;

internal interface IAnimatorController
{
    public void PlayWalkAnimation(bool state);
    public void PlayRunAnimation(bool state);
    public void PlayAttackAnimation();
    public void SetFloatToAnimation(float x, float y);
    public void PlayAttackAnimationByTrigger();
    public void PlayJumpAnimation();
    public void PlayDeathAnimation(bool state);
    public void PlayHitAnimation();
}
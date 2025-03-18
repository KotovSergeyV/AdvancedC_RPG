using UnityEngine;

public class AnimatorController : MonoBehaviour, IAnimatorController
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayWalkAnimation(bool state)
    {
        _animator.SetBool("isWalking", state);
    }

    public void PlayRunAnimation(bool state)
    {
        _animator.SetBool("isRunning", state);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetInteger("AttackClip", Random.Range(1, 4));
    }

    public void SetFloatToAnimation(float x, float y)
    {
        _animator.SetFloat("X", x);
        _animator.SetFloat("Y", y);
    }

    public void SetAttackClipToZero()
    {
        _animator.SetInteger("AttackClip", 0);
    }

    public void PlayAttackAnimationByTrigger()
    {
        _animator.SetTrigger("AttackTrigger");
    }

    public void PlayJumpAnimation()
    {
        _animator.SetTrigger("Jump");
    }
}

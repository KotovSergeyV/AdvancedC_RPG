using UnityEngine;

public class AnimatorController : MonoBehaviour, IAnimatorController
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName, bool state)
    {
        animator.SetBool(animationName, state);
    }

    public void TriggerAnimation(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    public void SetFloatAnimation(float x, float y)
    {
        animator.SetFloat("X", x);
        animator.SetFloat("Y", y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini1Animation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public Dictionary<string, float> animationDurations = new Dictionary<string, float>();

    private void Start()
    {
        // Get the Animator Controller
        var controller = animator.runtimeAnimatorController;
        if (controller != null)
        {
            foreach (var clip in controller.animationClips)
            {
                if (clip != null)
                {
                    // Debug.Log("Animation Clip: " + clip.name + " Length: " + clip.length);
                    animationDurations[clip.name] = clip.length;
                }
            }
        }
    }


    public IEnumerator PlayPrepareAnimations(string selectedMedal, string animation1, string animation2, float delayBetween = 0f)
    {
        // Play the first animation
        animator.SetBool(selectedMedal, true);
        yield return WaitForAnimation(animation1);
        animator.SetBool(selectedMedal, false);

        // // Optional delay before the second animation
        // yield return new WaitForSeconds(delayBetween);
        
        // // Play the second animation
        // animator.SetTrigger(animation2);
        // yield return WaitForAnimation(animation2);
    }

    public void SetSuccessJump(bool result)
    {
        animator.SetBool("success", result);
    }

    public void SetGameOverCelebration(bool gameoverCheck)
    {
        animator.SetBool("gameOver", gameoverCheck);
    }

    // public void TriggerCollideFalling()
    // {
    //     animator.SetTrigger("collideFalling");
    // }

    public void TriggerFailRecovery(){
        animator.SetTrigger("failRecovery");
    }

    private IEnumerator WaitForAnimation(string animationName)
    {
        // Wait until the animation is finished
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && 
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
    }
}

using UnityEngine;
using System.Collections;

public class PetAnimationController : MonoBehaviour
{
    public static PetAnimationController Instance;

    [Header("Animation Components")]
    public Animator petAnimator;
    
    [Header("Triggers")]
    public string eatTrigger = "Eat";
    public string backToSleepTrigger = "BackToSleep";
    
    [Header("Timing")]
    public float eatAnimationDuration = 3.0f; // Customizable 3s duration

    private Coroutine animationCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Triggers the Eat animation and returns to Sleep after a delay.
    /// </summary>
    public void PlayEatAnimation()
    {
        Debug.Log("PetAnimationController: PlayEatAnimation called.");

        if (petAnimator == null)
        {
            Debug.LogWarning("PetAnimator is not assigned on PetAnimationController!");
            return;
        }

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        animationCoroutine = StartCoroutine(EatSequence());
    }

    private IEnumerator EatSequence()
    {
        // Reset triggers to ensure a clean state change
        petAnimator.ResetTrigger(eatTrigger);
        petAnimator.ResetTrigger(backToSleepTrigger);

        // Start eating
        petAnimator.SetTrigger(eatTrigger);

        yield return new WaitForSeconds(eatAnimationDuration);

        // Reset Eat and go back to sleep
        petAnimator.ResetTrigger(eatTrigger);
        petAnimator.SetTrigger(backToSleepTrigger);
        
        animationCoroutine = null;
    }
}

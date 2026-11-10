using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class EatHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float holdTime = 1f;
    public float fadeTime = 0.5f;
    
    private float timer = 0f;
    private bool isHolding = false;
    private bool isEating = false;
    private Image foodImage;

    void Start()
    {
        foodImage = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isEating) return;
        FoodTimer foodTimer = GetComponent<FoodTimer>();
        if (foodTimer != null && !foodTimer.isPlaced) return;
        
        isHolding = true;
        timer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        timer = 0f;
    }

    void Update()
    {
        if (isHolding && !isEating)
        {
            timer += Time.deltaTime;
            if (timer >= holdTime)
            {
                StartCoroutine(EatSequence());
            }
        }
    }

    IEnumerator EatSequence()
    {
        isEating = true;
        isHolding = false;
        
        // Play click sound when eating food
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        // Notify Pet Page (Assuming a manager or static method)
        FoodTimer foodTimer = GetComponent<FoodTimer>();
        if (foodTimer != null)
        {
            PetManager.Instance.OnFoodEaten(foodTimer.foodData, foodTimer.GetCurrentStage());
        }

        // Fade out
        float elapsed = 0f;
        Color startColor = foodImage.color;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            foodImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
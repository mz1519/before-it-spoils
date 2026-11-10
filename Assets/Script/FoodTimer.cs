using UnityEngine;
using UnityEngine.UI;

public class FoodTimer : MonoBehaviour
{
    public FoodData foodData;
    
    [Header("Testing Settings")]
    public float freshDuration = 5f;
    public float staleDuration = 5f;

    private float timeElapsed = 0f;
    private int currentStage = -1;
    private Image foodImage;
    private float initialHeight;
    public bool isPlaced = false;

    void Awake()
    {
        EnsureComponents();
    }

    void Start()
    {
        EnsureComponents();
        UpdateStage(0);
    }

    private void EnsureComponents()
    {
        if (foodImage == null) foodImage = GetComponent<Image>();
        if (foodImage != null)
        {
            foodImage.color = Color.white;
            foodImage.preserveAspect = true;
            
            if (initialHeight <= 0)
            {
                initialHeight = foodImage.rectTransform.sizeDelta.y;
                if (initialHeight <= 0) initialHeight = 110f;
            }
        }
    }

    private void OnValidate()
    {
        if (foodData != null)
        {
            EnsureComponents();
            if (foodImage != null && foodData.freshSprite != null)
            {
                foodImage.sprite = foodData.freshSprite;
            }
        }
    }

    void Update()
    {
        if (!isPlaced || foodData == null) return;

        timeElapsed += Time.deltaTime;

        int newStage;
        if (timeElapsed < freshDuration) 
            newStage = 0;        
        else if (timeElapsed < (freshDuration + staleDuration)) 
            newStage = 1;        
        else 
            newStage = 2;        

        if (newStage != currentStage)
            UpdateStage(newStage);
    }

    void UpdateStage(int stage)
    {
        currentStage = stage;
        EnsureComponents();
        if (foodImage == null || foodData == null) return;

        switch (stage)
        {
            case 0:
                foodImage.sprite = foodData.freshSprite;
                break;
            case 1:
                foodImage.sprite = foodData.staleSprite;
                break;
            case 2:
                foodImage.sprite = foodData.rottenSprite;
                break;
        }

        if (foodImage.sprite != null)
        {
            float aspectRatio = foodImage.sprite.rect.width / foodImage.sprite.rect.height;
            foodImage.rectTransform.sizeDelta = new Vector2(initialHeight * aspectRatio, initialHeight);
        }
    }

    public int GetCurrentStage() { return currentStage; }
    public bool IsFresh() { return currentStage == 0; }
    public bool IsRotten() { return currentStage == 2; }
}
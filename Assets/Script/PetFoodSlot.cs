using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetFoodSlot : MonoBehaviour
{
    public Image foodIcon;
    public TextMeshProUGUI foodNameText;

    public bool isOccupied = false;
    public FoodData storedFoodData;
    public int storedStage;

    void Start()
    {
        ClearSlot();
    }

    public void SetFood(FoodData data, int stage)
    {
        storedFoodData = data;
        storedStage = stage;
        isOccupied = true;

        if (foodIcon == null)
            foodIcon = transform.Find("FoodIcon")?.GetComponent<Image>();

        if (foodNameText == null)
            foodNameText = transform.Find("FoodName")?.GetComponent<TextMeshProUGUI>();

        if (foodIcon != null)
        {
            switch (stage)
            {
                case 0:
                    foodIcon.sprite = data.freshSprite;
                    break;
                case 1:
                    foodIcon.sprite = data.staleSprite;
                    break;
                case 2:
                    foodIcon.sprite = data.rottenSprite;
                    break;
            }

            foodIcon.enabled = true;
            foodIcon.color = Color.white;
            foodIcon.preserveAspect = true;
            foodIcon.gameObject.SetActive(true);
        }

        if (foodNameText != null)
        {
            foodNameText.text = data.foodName;
            foodNameText.gameObject.SetActive(true);
        }
    }

    public void ClearSlot()
    {
        isOccupied = false;
        storedFoodData = null;
        storedStage = 0;

        if (foodIcon == null)
            foodIcon = transform.Find("FoodIcon")?.GetComponent<Image>();

        if (foodNameText == null)
            foodNameText = transform.Find("FoodName")?.GetComponent<TextMeshProUGUI>();

        if (foodIcon != null)
        {
            foodIcon.sprite = null;
            foodIcon.color = new Color(1, 1, 1, 0);
            foodIcon.enabled = false;
            foodIcon.gameObject.SetActive(false);
        }

        if (foodNameText != null)
        {
            foodNameText.text = "";
            foodNameText.gameObject.SetActive(false);
        }
    }
}
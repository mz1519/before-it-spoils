using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CookingPotManager : MonoBehaviour
{
    public static CookingPotManager Instance;

    [Header("Drop Area")]
    public RectTransform potDropArea;

    [Header("Cooking Settings")]
    public int ingredientsNeeded = 2;
    public float singleCookingAnimationLength = 1.0f;
    public int cookingLoopCount = 2;

    [Header("Animation")]
public Animator potAnimator;
    public string startCookingTrigger = "StartCooking";
    public string backToIdleTrigger = "BackToIdle";

    [Header("Dish Bubble")]
    public DishBubbleUI dishBubbleUI;

    [Header("Dish Sprites")]
    public Sprite beefStewSprite;
    public Sprite chickenStewSprite;
    public Sprite fishStewSprite;

    public Sprite saladSprite;
    public Sprite smoothieSprite;

    public Sprite eggSoupSprite;
    public Sprite eggPuddingSprite;
    public Sprite eggBowlSprite;
    public Sprite pancakeSprite;
    public Sprite boiledEggSprite;

    public Sprite grilledBeefSprite;
    public Sprite grilledChickenSprite;
    public Sprite grilledFishSprite;

    public Sprite darkMatterSprite;
    public Sprite mysterySoupSprite;
    public Sprite poopFoodSprite;

    private List<CookedIngredient> ingredients = new List<CookedIngredient>();
    private bool isCooking = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (potAnimator == null)
            potAnimator = GetComponent<Animator>();

        if (dishBubbleUI != null)
            dishBubbleUI.Hide();
    }

    public bool IsPointInsidePot(Vector2 screenPosition, Camera eventCamera)
    {
        if (potDropArea == null || isCooking) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            potDropArea,
            screenPosition,
            eventCamera
        );
    }

    public void AddIngredient(FoodData data, int stage)
    {
        if (data == null || isCooking) return;

        ingredients.Add(new CookedIngredient(data, stage));

        Debug.Log("Added to pot: " + data.foodName +
                  " | Category: " + data.category +
                  " | Stage: " + stage);

        if (ingredients.Count >= ingredientsNeeded)
        {
            StartCoroutine(CookingSequence());
        }
    }

    private IEnumerator CookingSequence()
    {
        isCooking = true;

        // Play the cook sound effect when cooking starts
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCook();

        DishResult result = GetRecipeResult();

        // Ensure DishBubble is enabled and shows "Cooking..." state
        if (dishBubbleUI != null)
        {
            Vector3 spawnPos = transform.position;
            if (potDropArea != null) spawnPos = potDropArea.position;
            dishBubbleUI.ShowCookingState(spawnPos);
        }

        if (potAnimator != null)
        {
            potAnimator.ResetTrigger(backToIdleTrigger);
            potAnimator.SetTrigger(startCookingTrigger);
        }

        float totalCookingTime = 2.0f; // Total cooking wait time is 2 seconds total
        yield return new WaitForSeconds(totalCookingTime);

        if (potAnimator != null)
{
            potAnimator.ResetTrigger(startCookingTrigger);
            potAnimator.SetTrigger(backToIdleTrigger);
        }

        ShowDishResult(result);

        ingredients.Clear();
        isCooking = false;
    }

    private void ShowDishResult(DishResult result)
    {
        Debug.Log("Cooked: " + result.dishName +
                  " | Growth: " + result.growthValue +
                  " | Mood: " + result.moodValue);

        if (dishBubbleUI != null)
        {
            // Ensure the GameObject is active so the script can run and start coroutines
            dishBubbleUI.gameObject.SetActive(true);

            Vector3 spawnPos = transform.position;

            if (potDropArea != null)
                spawnPos = potDropArea.position;

            dishBubbleUI.ShowDish(result, spawnPos);
        }
    }

    private DishResult GetRecipeResult()
    {
        // 1. Rotten ingredient has the highest priority.
        // Any rotten food becomes Poop Food.
        if (HasRottenIngredient())
        {
            return new DishResult(
                "Poop Food",
                "A suspicious result from spoiled ingredients.",
                poopFoodSprite,
                -30,
                -80
            );
        }

        bool hasUnfresh = HasUnfreshIngredient();

        int vegetableCount = CountCategory(FoodCategory.Vegetable);
        int proteinCount = CountCategory(FoodCategory.Protein);
        int fruitCount = CountCategory(FoodCategory.Fruit);
        int dairyCount = CountCategory(FoodCategory.Dairy);
        int eggCount = CountCategory(FoodCategory.Egg);

        DishResult baseResult;

        // 2. Protein + Vegetable = [ProteinName] Stew
        if (proteinCount >= 1 && vegetableCount >= 1)
        {
            FoodData protein = GetFirstIngredientByCategory(FoodCategory.Protein);
            string proteinName = GetDisplayName(protein);
            Sprite stewSprite = GetStewSpriteByProtein(protein);

            baseResult = new DishResult(
                proteinName + " Stew",
                "A warm meal made before it spoiled.",
                stewSprite,
                120,
                80
            );
        }

        // 3. Egg + Vegetable = [VegetableName] Egg Soup
        else if (eggCount >= 1 && vegetableCount >= 1)
        {
            FoodData vegetable = GetFirstIngredientByCategory(FoodCategory.Vegetable);
            string vegetableName = GetDisplayName(vegetable);

            baseResult = new DishResult(
                vegetableName + " Egg Soup",
                "A gentle soup made with egg and rescued greens.",
                eggSoupSprite,
                90,
                90
            );
        }

        // 4. Egg + Protein = [ProteinName] Egg Bowl
        else if (eggCount >= 1 && proteinCount >= 1)
        {
            FoodData protein = GetFirstIngredientByCategory(FoodCategory.Protein);
            string proteinName = GetDisplayName(protein);

            baseResult = new DishResult(
                proteinName + " Egg Bowl",
                "A filling meal with warm protein and egg.",
                eggBowlSprite,
                110,
                70
            );
        }

        // 5. Egg + Dairy = Creamy Egg Pudding
        else if (eggCount >= 1 && dairyCount >= 1)
        {
            baseResult = new DishResult(
                "Creamy Egg Pudding",
                "A soft little dessert from dairy and egg.",
                eggPuddingSprite,
                50,
                150
            );
        }

        // 6. Egg + Fruit = [FruitName] Pancake
        else if (eggCount >= 1 && fruitCount >= 1)
        {
            FoodData fruit = GetFirstIngredientByCategory(FoodCategory.Fruit);
            string fruitName = GetDisplayName(fruit);

            baseResult = new DishResult(
                fruitName + " Pancake",
                "A tiny sweet dish made from fruit and egg.",
                pancakeSprite,
                60,
                120
            );
        }

        // 7. Egg + Egg = Soft Boiled Eggs
        else if (eggCount >= 2)
        {
            baseResult = new DishResult(
                "Soft Boiled Eggs",
                "Simple, warm, and surprisingly comforting.",
                boiledEggSprite,
                70,
                70
            );
        }

        // 8. Vegetable + Vegetable = Garden Salad
        else if (vegetableCount >= 2)
        {
            baseResult = new DishResult(
                "Garden Salad",
                "Fresh greens, neatly rescued.",
                saladSprite,
                80,
                100
            );
        }

        // 9. Fruit + Dairy = [FruitName] Smoothie
        else if (fruitCount >= 1 && dairyCount >= 1)
        {
            FoodData fruit = GetFirstIngredientByCategory(FoodCategory.Fruit);
            string fruitName = GetDisplayName(fruit);

            baseResult = new DishResult(
                fruitName + " Smoothie",
                "A soft drink made from saved ingredients.",
                smoothieSprite,
                60,
                130
            );
        }

        // 10. Protein + Protein = Grilled [ProteinName]
        else if (proteinCount >= 2)
        {
            FoodData protein = GetFirstIngredientByCategory(FoodCategory.Protein);
            string proteinName = GetDisplayName(protein);
            Sprite grilledSprite = GetGrilledSpriteByProtein(protein);

            baseResult = new DishResult(
                "Grilled " + proteinName,
                "Simple, warm, and full of energy.",
                grilledSprite,
                120,
                60
            );
        }

        // 11. Fruit only = Fruit Mix
        else if (fruitCount >= 1)
        {
            FoodData fruit = GetFirstIngredientByCategory(FoodCategory.Fruit);
            string fruitName = GetDisplayName(fruit);

            baseResult = new DishResult(
                fruitName + " Mix",
                "A small sweet snack from the fridge.",
                smoothieSprite,
                30,
                100
            );
        }

        // 12. Other / unrecognized combination = Mystery Soup
        else
        {
            baseResult = new DishResult(
                "Mystery Soup",
                "A strange little meal from whatever was left.",
                mysterySoupSprite,
                20,
                20
            );
        }

        // If one ingredient is stale / near-expiry, reduce reward by half.
        if (hasUnfresh)
        {
            baseResult.dishName = "Left-over " + baseResult.dishName;
            baseResult.description = "Still edible, but not as fresh as before.";

            baseResult.growthValue = Mathf.RoundToInt(baseResult.growthValue * 0.5f);
            baseResult.moodValue = Mathf.RoundToInt(baseResult.moodValue * 0.5f);
            baseResult.joyValue = baseResult.moodValue;
        }

        return baseResult;
    }

    private bool HasRottenIngredient()
    {
        foreach (CookedIngredient ingredient in ingredients)
        {
            if (ingredient.stage == 2)
                return true;
        }

        return false;
    }

    private bool HasUnfreshIngredient()
    {
        foreach (CookedIngredient ingredient in ingredients)
        {
            if (ingredient.stage == 1)
                return true;
        }

        return false;
    }

    private int CountCategory(FoodCategory category)
    {
        int count = 0;

        foreach (CookedIngredient ingredient in ingredients)
        {
            if (ingredient.data != null && ingredient.data.category == category)
                count++;
        }

        return count;
    }

    private FoodData GetFirstIngredientByCategory(FoodCategory category)
    {
        foreach (CookedIngredient ingredient in ingredients)
        {
            if (ingredient.data != null && ingredient.data.category == category)
                return ingredient.data;
        }

        return null;
    }

    private string GetFirstIngredientDisplayNameByCategory(FoodCategory category)
    {
        FoodData data = GetFirstIngredientByCategory(category);
        return GetDisplayName(data);
    }

    private string GetDisplayName(FoodData data)
    {
        if (data == null)
            return "Mystery";

        if (!string.IsNullOrEmpty(data.cookedDisplayName))
            return data.cookedDisplayName;

        return data.foodName;
    }

    private Sprite GetStewSpriteByProtein(FoodData protein)
    {
        string proteinName = GetDisplayName(protein).ToLower();

        if (proteinName.Contains("beef"))
            return beefStewSprite;

        if (proteinName.Contains("fish") || proteinName.Contains("salmon"))
            return fishStewSprite;

        if (proteinName.Contains("chicken"))
            return chickenStewSprite;

        return chickenStewSprite;
    }

    private Sprite GetGrilledSpriteByProtein(FoodData protein)
    {
        string proteinName = GetDisplayName(protein).ToLower();

        if (proteinName.Contains("beef"))
            return grilledBeefSprite;

        if (proteinName.Contains("fish") || proteinName.Contains("salmon"))
            return grilledFishSprite;

        if (proteinName.Contains("chicken"))
            return grilledChickenSprite;

        return grilledChickenSprite;
    }
}

public class CookedIngredient
{
    public FoodData data;
    public int stage;

    public CookedIngredient(FoodData data, int stage)
    {
        this.data = data;
        this.stage = stage;
    }
}

public class DishResult
{
    public string dishName;
    public string description;
    public Sprite dishSprite;

    public int growthValue;

    // Use moodValue for the new pet mood system.
    public int moodValue;

    // Keep joyValue for compatibility with older DishBubbleUI code.
    public int joyValue;

    public DishResult(string dishName, string description, Sprite dishSprite, int growth, int mood)
    {
        this.dishName = dishName;
        this.description = description;
        this.dishSprite = dishSprite;

        this.growthValue = growth;
        this.moodValue = mood;
        this.joyValue = mood;
    }
}
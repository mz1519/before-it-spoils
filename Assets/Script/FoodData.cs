using UnityEngine;

public enum FoodCategory
{
    Vegetable,
    Protein,
    Fruit,
    Dairy,
    Egg,
    Grain,
    Other
}

[CreateAssetMenu(fileName = "FoodData", menuName = "Food/FoodData")]
public class FoodData : ScriptableObject
{
    public string foodName;

    [Header("Cooking Info")]
    public FoodCategory category;
    public string cookedDisplayName;

    public int staleDays;
    public int rottenDays;

    public Sprite freshSprite;
    public Sprite staleSprite;
    public Sprite rottenSprite;
}
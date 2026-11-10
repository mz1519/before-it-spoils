using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetStatsManager : MonoBehaviour
{
    public static PetStatsManager Instance;

    [Header("Stats")]
    public int growth = 0;
    public int mood = 0;
    public int level = 1;

    public int maxGrowth = 300;
    public int maxMood = 300;

    [Header("UI")]
    public Image growthFill;
    public Image moodFill;
    public TextMeshProUGUI growthText;
    public TextMeshProUGUI moodText;
    public TextMeshProUGUI levelText;

    [Header("Pet Visuals")]
    public Image petImage;
    public Sprite level1PetSprite;
    public Sprite level2PetSprite;
    public Sprite level3PetSprite;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        UpdatePetVisual();
    }

    public void AddStats(int growthAmount, int moodAmount)
    {
        growth += growthAmount;
        mood += moodAmount;

        if (growth < 0) growth = 0;
        if (mood < 0) mood = 0;

        CheckLevelUp();

        if (mood > maxMood)
            mood = maxMood;

        UpdateUI();

        Debug.Log("Growth " + growthAmount + ", Mood " + moodAmount);
    }

    private void CheckLevelUp()
    {
        while (growth >= maxGrowth)
        {
            growth -= maxGrowth;
            level++;

            Debug.Log("Level Up! Current Level: " + level);

            UpdatePetVisual();
        }
    }

    private void UpdateUI()
    {
        if (growthFill != null)
            growthFill.fillAmount = (float)growth / maxGrowth;

        if (moodFill != null)
            moodFill.fillAmount = (float)mood / maxMood;

        if (growthText != null)
            growthText.text = growth + " / " + maxGrowth;

        if (moodText != null)
            moodText.text = mood + " / " + maxMood;

        if (levelText != null)
            levelText.text = "Lv. " + level;
    }

    private void UpdatePetVisual()
    {
        if (petImage == null) return;

        if (level >= 3 && level3PetSprite != null)
        {
            petImage.sprite = level3PetSprite;
        }
        else if (level >= 2 && level2PetSprite != null)
        {
            petImage.sprite = level2PetSprite;
        }
        else if (level1PetSprite != null)
        {
            petImage.sprite = level1PetSprite;
        }
    }
}
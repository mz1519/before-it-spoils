using UnityEngine;

public class PetManager : MonoBehaviour
{
    public static PetManager Instance;

    [Header("Pages")]
    public GameObject fridgePage;
    public GameObject petPage;

    [Header("Food Slots on Pet Page")]
    public PetFoodSlot[] foodSlots;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnFoodEaten(FoodData data, int stage)
    {
        if (data == null)
        {
            Debug.LogWarning("FoodData is null.");
            return;
        }

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (foodSlots[i] != null && !foodSlots[i].isOccupied)
            {
                foodSlots[i].SetFood(data, stage);

                Debug.Log($"Added {data.foodName} to Slot {i + 1}, Stage = {stage}");

                // 使用 PageSwiper 翻页，而不是直接关闭页面，确保系统逻辑正常运行
                PageSwiper swiper = Object.FindAnyObjectByType<PageSwiper>();
                if (swiper != null)
                {
                    swiper.GoToPage(2); // 2 是 Pet 所在的页面索引
                }
                else
                {
                    // 退而求其次的保险逻辑
                    if (fridgePage != null) fridgePage.SetActive(false);
                    if (petPage != null) petPage.SetActive(true);
                }

                return;
            }
        }

        Debug.Log("No empty food slots left on Pet page.");
    }
}
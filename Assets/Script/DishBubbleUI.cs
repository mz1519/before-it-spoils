using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DishBubbleUI : MonoBehaviour
{
    [System.Serializable]
    public class ElementLayoutSettings
    {
        public Vector2 size = new Vector2(360, 25);
        public Vector3 scale = Vector3.one;
        public Vector2 offset = Vector2.zero;
    }

    [Header("Components")]
    public GameObject bubbleRoot;
    public Image dishImage;
    public TextMeshProUGUI dishNameText;
    public TextMeshProUGUI dishDescriptionText;
    public RectTransform statsRow;
    public RectTransform menuBackground;

    [Header("Stats")]
    public TextMeshProUGUI growthText;
    public TextMeshProUGUI moodText;
    public Image growthIcon;
    public Image moodIcon;

    [Header("Container Layout")]
    public Vector2 containerSize = new Vector2(400, 180);
    public RectOffset containerPadding;
    public float verticalSpacing = 4f;

    [Header("Element Layouts")]
    public ElementLayoutSettings dishNameLayout = new ElementLayoutSettings { size = new Vector2(360, 25) };
    public ElementLayoutSettings dishImageLayout = new ElementLayoutSettings { size = new Vector2(360, 60) };
    public ElementLayoutSettings statsRowLayout = new ElementLayoutSettings { size = new Vector2(360, 25) };
    public ElementLayoutSettings descriptionLayout = new ElementLayoutSettings { size = new Vector2(360, 36) };
    public ElementLayoutSettings backgroundLayout = new ElementLayoutSettings { size = new Vector2(400, 180) };

    [Header("Animation")]
    public float animationDuration = 0.5f;
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;

    [Header("Auto Feeding")]
    public bool autoFeedAfterDelay = true;
    public float displayDuration = 3.3f;
    public Transform petMouthTarget;

    private Coroutine animationCoroutine;
    private DishResult currentDish;

    private void Reset()
{
        containerPadding = new RectOffset(10, 10, 10, 10);
    }

    void Awake()
    {
        if (containerPadding == null) containerPadding = new RectOffset(10, 10, 10, 10);
        ApplyCustomLayout();
        Hide();
    }

    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            UnityEditor.EditorApplication.delayCall += () => {
                if (this != null) ApplyCustomLayout();
            };
        }
    #endif

    public void ApplyCustomLayout()
    {
        if (bubbleRoot == null) return;

        // 1. Configure Container
        RectTransform rootRT = bubbleRoot.GetComponent<RectTransform>();
        if (rootRT != null)
        {
            rootRT.sizeDelta = containerSize;
        }

        VerticalLayoutGroup vlg = bubbleRoot.GetComponent<VerticalLayoutGroup>();
        if (vlg != null)
        {
            if (containerPadding == null) containerPadding = new RectOffset(10, 10, 10, 10);
            vlg.padding = containerPadding;
            vlg.spacing = verticalSpacing;
        }

        // 2. Configure Children
        ApplyElementSettings(dishNameText != null ? dishNameText.rectTransform : null, dishNameLayout);
        ApplyElementSettings(dishImage != null ? dishImage.rectTransform : null, dishImageLayout);
        ApplyElementSettings(statsRow, statsRowLayout);
        ApplyElementSettings(dishDescriptionText != null ? dishDescriptionText.rectTransform : null, descriptionLayout);
        ApplyElementSettings(menuBackground, backgroundLayout);
    }

    private void ApplyElementSettings(RectTransform rt, ElementLayoutSettings settings)
    {
        if (rt == null || settings == null) return;

        rt.sizeDelta = settings.size;
        rt.localScale = settings.scale;
        rt.anchoredPosition = settings.offset;
    }

    public void ShowCookingState(Vector3 position)
    {
        transform.SetAsLastSibling();
        if (bubbleRoot != null) bubbleRoot.transform.SetAsLastSibling();

        gameObject.SetActive(true);
        if (bubbleRoot != null) bubbleRoot.SetActive(true);
        
        // Ensure all children are active
        if (dishNameText != null) dishNameText.gameObject.SetActive(true);
        if (dishImage != null) dishImage.gameObject.SetActive(true);
        if (dishDescriptionText != null) dishDescriptionText.gameObject.SetActive(true);
        if (menuBackground != null) menuBackground.gameObject.SetActive(true);
        if (statsRow != null) statsRow.gameObject.SetActive(true);

        RectTransform rt = bubbleRoot != null ? bubbleRoot.GetComponent<RectTransform>() : GetComponent<RectTransform>();
        if (rt != null) rt.position = position;

        if (dishNameText != null) dishNameText.text = "Cooking...";
        if (dishDescriptionText != null) dishDescriptionText.text = "Smells good!";
        if (statsRow != null) statsRow.gameObject.SetActive(false);
        if (dishImage != null) dishImage.enabled = false;

        ApplyCustomLayout();
        
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(ScaleInAnimation(startScale, targetScale));
    }

    public void ShowDish(DishResult result, Vector3 startPos)
    {
        transform.SetAsLastSibling();
        if (bubbleRoot != null) bubbleRoot.transform.SetAsLastSibling();

        currentDish = result;
        gameObject.SetActive(true); // Ensure container is active
        if (bubbleRoot != null) bubbleRoot.SetActive(true);
        
        // Ensure all components are active
        if (dishNameText != null) dishNameText.gameObject.SetActive(true);
        if (dishImage != null) dishImage.gameObject.SetActive(true);
        if (dishDescriptionText != null) dishDescriptionText.gameObject.SetActive(true);
        if (menuBackground != null) menuBackground.gameObject.SetActive(true);
        if (statsRow != null) statsRow.gameObject.SetActive(true);
        
        ApplyCustomLayout();
        
        RectTransform rt = bubbleRoot != null ? bubbleRoot.GetComponent<RectTransform>() : GetComponent<RectTransform>();
        if (rt != null) rt.position = startPos;

        if (dishNameText != null) dishNameText.text = result.dishName;
        if (dishDescriptionText != null) dishDescriptionText.text = result.description;
        
        if (growthText != null) growthText.text = FormatReward(result.growthValue);
        if (moodText != null) moodText.text = FormatReward(result.moodValue);

        if (dishImage != null)
        {
            dishImage.sprite = result.dishSprite;
            dishImage.enabled = result.dishSprite != null;
            dishImage.color = Color.white;
        }

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DishSequence());
    }

    private string FormatReward(int value)
    {
        if (value >= 0) return "+" + value;
        return value.ToString();
    }

    private IEnumerator DishSequence()
    {
        // Scale in
        yield return StartCoroutine(ScaleInAnimation(startScale, targetScale));

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Auto Feed
        if (autoFeedAfterDelay && currentDish != null)
        {
            FeedPet();
        }

        // Scale out
        yield return StartCoroutine(ScaleInAnimation(targetScale, startScale));

        Hide();
    }

    private void FeedPet()
    {
        if (PetStatsManager.Instance != null && currentDish != null)
        {
            PetStatsManager.Instance.AddStats(currentDish.growthValue, currentDish.moodValue);

            // Trigger pet animation
            if (PetAnimationController.Instance != null)
            {
                PetAnimationController.Instance.PlayEatAnimation();
            }
            else
            {
                Debug.LogWarning("PetAnimationController.Instance is null! Cannot trigger Eat animation.");
            }

            // Start food travel animation
            if (dishImage != null && dishImage.sprite != null && dishImage.enabled)
            {
                StartCoroutine(AnimateFoodToPet(dishImage.sprite, dishImage.rectTransform.position));
            }
        }
        else
        {
            Debug.LogWarning("PetStatsManager.Instance or currentDish is null! Cannot feed pet.");
        }
    }

    private IEnumerator AnimateFoodToPet(Sprite foodSprite, Vector3 startPos)
    {
        // Create a temporary ghost image for the travel animation
        GameObject ghost = new GameObject("FoodTravelGhost");
        ghost.transform.SetParent(bubbleRoot.transform.parent, false);
        ghost.transform.position = startPos;
        ghost.transform.SetAsLastSibling();

        UnityEngine.UI.Image ghostImage = ghost.AddComponent<UnityEngine.UI.Image>();
        ghostImage.sprite = foodSprite;
        ghostImage.preserveAspect = true;
        ghostImage.raycastTarget = false;

        RectTransform ghostRT = ghost.GetComponent<RectTransform>();
        ghostRT.sizeDelta = dishImage.rectTransform.sizeDelta;

        // Hide the original image in the bubble so it doesn't double up or scale weirdly
        dishImage.enabled = false;

        float elapsed = 0;
        float duration = 0.8f; // Animation travel time

        // Determine target position (Mouth or Pet center)
        Vector3 endPos = petMouthTarget != null ? petMouthTarget.position : startPos;
        if (petMouthTarget == null && PetAnimationController.Instance != null && PetAnimationController.Instance.petAnimator != null)
        {
            endPos = PetAnimationController.Instance.petAnimator.transform.position;
        }

        // Mid-point offset for curve path
        Vector3 controlPoint = (startPos + endPos) / 2f + Vector3.up * 150f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Quadratic Bezier curve
            Vector3 pos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPos;
            ghostRT.position = pos;

            // Scale smaller as it approaches
            ghostRT.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.3f, t);

            // Fade to disappear
            Color c = ghostImage.color;
            c.a = 1f - t;
            ghostImage.color = c;

            yield return null;
        }

        Destroy(ghost);
    }

    private IEnumerator ScaleInAnimation(Vector3 from, Vector3 to)
    {
        RectTransform rt = bubbleRoot != null ? bubbleRoot.GetComponent<RectTransform>() : GetComponent<RectTransform>();
        if (rt == null) yield break;

        rt.localScale = from;
        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            t = t * t * (3f - 2f * t);
            rt.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        rt.localScale = to;
    }

    public void Hide()
    {
        currentDish = null;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (bubbleRoot != null && bubbleRoot != gameObject)
            bubbleRoot.SetActive(false);
        else
        {
            // If bubbleRoot is this object, we can't disable it or the script stops.
            // We should disable children instead.
            foreach (Transform child in transform) child.gameObject.SetActive(false);
        }
    }
}
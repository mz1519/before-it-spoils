using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("页面设置")]
    public RectTransform pageContainer;
    public int totalPages = 3;
    public int startPage = 1;

    [Header("滑动设置")]
    public float swipeThreshold = 50f;
    public float animDuration = 0.35f;

    public int currentPage; 
    private float pageWidth; 
    private Vector2 dragStartPos;
    private bool isAnimating = false;
    private Vector2 animStartPos;
    private Vector2 animTargetPos;
    private float animTimer = 0f;

    private Canvas rootCanvas;

    void Start()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        UpdatePageWidth();
        currentPage = startPage;
        if (pageContainer != null)
            pageContainer.anchoredPosition = new Vector2(-currentPage * pageWidth, 0);
    }

    private void UpdatePageWidth()
    {
        if (pageContainer != null)
            pageWidth = pageContainer.rect.width;
        else
            pageWidth = GetComponent<RectTransform>().rect.width;

        if (pageWidth <= 0) pageWidth = 390f; // Fallback
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UpdatePageWidth();
        dragStartPos = eventData.position;
        isAnimating = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        float scale = rootCanvas != null ? rootCanvas.scaleFactor : 1.0f;

        float delta = (eventData.position.x - dragStartPos.x) / scale;
        delta = Mathf.Clamp(delta, -pageWidth, pageWidth);

        // If on first page, block dragging right
        if (currentPage == 0 && delta > 0)
            delta /= 3f;

        // If on last page, block dragging left
        if (currentPage == totalPages - 1 && delta < 0)
            delta /= 3f;

        float targetX = -currentPage * pageWidth + delta;
        pageContainer.anchoredPosition = new Vector2(targetX, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        float scale = rootCanvas != null ? rootCanvas.scaleFactor : 1.0f;

        float delta = (eventData.position.x - dragStartPos.x) / scale;

        if (delta < -swipeThreshold && currentPage < totalPages - 1)
            currentPage++;
        else if (delta > swipeThreshold && currentPage > 0)
            currentPage--;

        StartAnim(-currentPage * pageWidth);
        Debug.Log("delta: " + delta + " currentPage: " + currentPage);
    }

   // Unused variables removed
   public void GoToPage(int pageIndex)
   {
       if (pageIndex < 0 || pageIndex >= totalPages) return;
       currentPage = pageIndex;
       StartAnim(-currentPage * pageWidth);
   }

    void StartAnim(float targetX)
    {
        animStartPos = pageContainer.anchoredPosition;
        animTargetPos = new Vector2(targetX, 0);
        animTimer = 0f;
        isAnimating = true;
    }

    void Update()
    {
        if (!isAnimating) return;

        animTimer += Time.deltaTime;
        float t = Mathf.Clamp01(animTimer / animDuration);
        float ease = 1f - Mathf.Pow(1f - t, 3f);

        pageContainer.anchoredPosition = Vector2.Lerp(animStartPos, animTargetPos, ease);

        if (t >= 1f) isAnimating = false;
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedFoodDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Canvas rootCanvas;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        FoodTimer timer = GetComponent<FoodTimer>();
        if (timer != null && !timer.isPlaced) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Play click sound when releasing the drag
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
    }
}
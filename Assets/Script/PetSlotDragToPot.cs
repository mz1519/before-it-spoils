using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PetSlotDragToPot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private PetFoodSlot slot;
    private Canvas rootCanvas;
    private RectTransform rootCanvasRect;

    private GameObject dragGhost;
    private Image dragGhostImage;

    private PageSwiper pageSwiper;
    private bool isForwardingToSwiper = false;

    void Awake()
    {
        slot = GetComponent<PetFoodSlot>();
        rootCanvas = GetComponentInParent<Canvas>();
        pageSwiper = GetComponentInParent<PageSwiper>();

        if (rootCanvas != null)
            rootCanvasRect = rootCanvas.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null || !slot.isOccupied || slot.foodIcon == null)
        {
            if (pageSwiper != null)
            {
                isForwardingToSwiper = true;
                pageSwiper.OnBeginDrag(eventData);
            }
            return;
        }

        isForwardingToSwiper = false;
        CreateDragGhost();
        MoveGhost(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isForwardingToSwiper)
        {
            if (pageSwiper != null) pageSwiper.OnDrag(eventData);
            return;
        }

        if (dragGhost == null)
            return;

        MoveGhost(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isForwardingToSwiper)
        {
            if (pageSwiper != null) pageSwiper.OnEndDrag(eventData);
            isForwardingToSwiper = false;
            return;
        }

        if (dragGhost != null)
            Destroy(dragGhost);

        // Play click sound when releasing the drag
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        if (slot == null || !slot.isOccupied)
            return;

        if (CookingPotManager.Instance != null &&
            CookingPotManager.Instance.IsPointInsidePot(eventData.position, eventData.pressEventCamera))
        {
            CookingPotManager.Instance.AddIngredient(slot.storedFoodData, slot.storedStage);
            slot.ClearSlot();
        }
        else
        {
            Debug.Log("Ingredient was not dropped into the pot.");
        }
    }

    private void CreateDragGhost()
    {
        dragGhost = new GameObject("DraggingFoodGhost");
        dragGhost.transform.SetParent(rootCanvas.transform, false);
        dragGhost.transform.SetAsLastSibling();

        dragGhostImage = dragGhost.AddComponent<Image>();
        dragGhostImage.sprite = slot.foodIcon.sprite;
        dragGhostImage.color = Color.white;
        dragGhostImage.preserveAspect = true;
        dragGhostImage.raycastTarget = false;

        RectTransform ghostRT = dragGhost.GetComponent<RectTransform>();
        ghostRT.sizeDelta = slot.foodIcon.rectTransform.sizeDelta;
    }

    private void MoveGhost(PointerEventData eventData)
    {
        if (rootCanvasRect == null || dragGhost == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        dragGhost.GetComponent<RectTransform>().localPosition = localPoint;
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("设置")]
    public Canvas rootCanvas;
    public RectTransform fridgeZone;
    public PageSwiper pageSwiperObj;
    public FoodData foodDataRef;

    private GameObject dragCopy;
    private RectTransform dragRect;
    private bool hasNavigated = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragCopy = Instantiate(gameObject, rootCanvas.transform);
        dragRect = dragCopy.GetComponent<RectTransform>();
        
        // 确保缩放正确且不被父节点锚点拉伸
        dragRect.localScale = Vector3.one;
        dragRect.anchorMin = new Vector2(0.5f, 0.5f);
        dragRect.anchorMax = new Vector2(0.5f, 0.5f);
        
        dragCopy.transform.SetAsLastSibling();
        
        UnityEngine.UI.Image img = dragCopy.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.color = new Color(1, 1, 1, 0.8f);
            img.preserveAspect = true; // 保持比例
        }
        
        Destroy(dragCopy.GetComponent<FoodDragHandler>());
        hasNavigated = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCopy == null) return;

        // 使用 ScreenPointToLocalPointInRectangle 获得相对于 Canvas 的坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        dragRect.localPosition = localPoint;

        if (!hasNavigated && pageSwiperObj != null)
        {
            float edgeThreshold = 60f;
            if (eventData.position.x > Screen.width - edgeThreshold)
            {
                pageSwiperObj.GoToPage(1);
                hasNavigated = true;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragCopy == null) return;

        // Play click sound when releasing the drag
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        if (pageSwiperObj != null && pageSwiperObj.currentPage == 1)
        {
            // 使用 true 保持世界变换，防止缩放突变
            dragCopy.transform.SetParent(fridgeZone, true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                fridgeZone,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );
            
            // 设置位置并确保锚点和中心点正确
            dragRect.anchorMin = new Vector2(0.5f, 0.5f);
            dragRect.anchorMax = new Vector2(0.5f, 0.5f);
            dragRect.pivot = new Vector2(0.5f, 0.5f);
            dragRect.localPosition = localPoint;
            
            UnityEngine.UI.Image img = dragCopy.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = Color.white;
            }
            
            FoodTimer timer = dragCopy.GetComponent<FoodTimer>();
            if (timer == null) timer = dragCopy.AddComponent<FoodTimer>();
            timer.foodData = foodDataRef;
            timer.isPlaced = true;

            if (dragCopy.GetComponent<PlacedFoodDrag>() == null)
                dragCopy.AddComponent<PlacedFoodDrag>();
                
            if (dragCopy.GetComponent<EatHandler>() == null)
                dragCopy.AddComponent<EatHandler>();

            Debug.Log("放入冰箱，开始计时！");
        }
        else
        {
            Destroy(dragCopy);
        }

        dragCopy = null;
    }
}
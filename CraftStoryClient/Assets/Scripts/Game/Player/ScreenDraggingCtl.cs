using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenDraggingCtl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerDownHandler, IPointerUpHandler
{
    public float offsetX;
    public float offsetY;

    private bool isDrag;
    private bool isClick;
    private bool isClicking;

    Vector2 startPos;
    PointerEventData eventData;
    private RaycastHit _cacheRaycastHit;

    float clickingTime;

    private void Update()
    {
        if (isClick)
        {
            clickingTime += Time.deltaTime;

            if (clickingTime > 0.2f && !isDrag)
            {
                isClicking = true;
                OnClicking(eventData.position);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");

        isDrag = true;
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");

        Vector2 pointerPos = eventData.position - startPos;

        offsetX = pointerPos.x;
        offsetY = pointerPos.y;

        PlayerCtl.E.CameraCtl.CameraRotate(offsetX, offsetY);

        startPos = eventData.position;
        offsetX = 0;
        offsetY = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");

        isDrag = false;
        isClick = false;
        isClicking = false;
        clickingTime = 0;

        offsetX = 0;
        offsetY = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        //Debug.Log(eventData.position);

        this.eventData = eventData;

        isClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");

        if (isDrag)
            return;

        if (!isClicking)
            OnClick(eventData.position);

        if (isClicking)
            PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;

        isClick = false;
        isClicking = false;
        clickingTime = 0;

        //PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;
    }

    public void OnClicking(Vector2 pos)
    {
        var obj = RayCastHits(pos);
        if (obj == null)
            return;

        PlayerCtl.E.OnClicking(Time.deltaTime, obj);
        PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.BreackBlock;
    }

    private void OnClick(Vector2 pos)
    {
        var obj = RayCastHits(pos);
        if (obj == null)
            return;

        var createPos = _cacheRaycastHit.normal + _cacheRaycastHit.collider.transform.position;
        PlayerCtl.E.OnClick(obj, createPos);
    }

    /// <summary>
    /// Rayを飛ばしてチェック
    /// </summary>
    public GameObject RayCastHits(Vector2 position)
    {
        // スクリーン座標を元にRayを取得
        var ray = Camera.main.ScreenPointToRay(position);

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 5, false);
#endif

        if (!Physics.Raycast(ray, out _cacheRaycastHit))
            return null;

        return _cacheRaycastHit.collider.gameObject;
    }
}
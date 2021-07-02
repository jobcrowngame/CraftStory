using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenDraggingCtl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerDownHandler, IPointerUpHandler
{
    public float offsetX;
    public float offsetY;

    private bool isDrag;
    private bool isClick;
    private bool isDoubleTouch;
    private bool IsClicking
    {
        get => isClicking;
        set
        {
            isClicking = value;
        }
    }
    private bool isClicking;

    Vector2 startPos;
    PointerEventData eventData;
    private RaycastHit _cacheRaycastHit;
    private GameObject clickingObj;

    float clickingTime;
    float baseDistance = 0;

    private void Update()
    {
        //isDoubleTouch = Input.touchCount == 2;

        if (isClick)
        {
            clickingTime += Time.deltaTime;

            if (clickingTime > 0.2f && !isDrag)
            {
                IsClicking = true;
                OnClicking(eventData.position);
            }
        }

        //if (Input.touchCount == 2)
        //{
        //    if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[1].phase == TouchPhase.Ended)
        //    {
        //        baseDistance = 0;
        //    }
        //    else
        //    {
        //        if (baseDistance == 0)
        //        {
        //            baseDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
        //        }
        //        else
        //        {
        //            var newDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
        //            PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(newDistance - baseDistance);
        //        }
        //    }
        //}
        //else
        //{
            
        //}
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Logger.Log("OnBeginDrag");
        if (isDoubleTouch)
            return;

        isDrag = true;
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Logger.Log("OnDrag");

        if (isDoubleTouch)
            return;

        Vector2 pointerPos = eventData.position - startPos;

        offsetX = pointerPos.x;
        offsetY = pointerPos.y;

        PlayerCtl.E.CameraCtl.CameraRotate(offsetX, offsetY);
        PlayerCtl.E.BlueprintPreviewCtl.CameraRotate(offsetX, offsetY);

        startPos = eventData.position;
        offsetX = 0;
        offsetY = 0;

        PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Logger.Log("OnEndDrag");

        if (isDoubleTouch)
            return;

        isDrag = false;
        isClick = false;
        IsClicking = false;
        clickingTime = 0;

        offsetX = 0;
        offsetY = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDoubleTouch)
            return;

        //Logger.Log("OnPointerDown");
        //Logger.Log(eventData.position);

        this.eventData = eventData;

        isClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Logger.Log("OnPointerUp");

        if (isDrag || isDoubleTouch)
            return;

        if (!IsClicking)
            OnClick(eventData.position);

        if (IsClicking)
        {
            PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;
            CancelClicking();
        }

        isClick = false;
        IsClicking = false;
        clickingTime = 0;
    }

    public void OnClicking(Vector2 pos)
    {
        clickingObj = RayCastHits(pos);
        if (clickingObj == null)
            return;

        PlayerCtl.E.OnClicking(Time.deltaTime, clickingObj);
    }
    public void CancelClicking()
    {
        if (clickingObj != null)
        {
            var cell = clickingObj.GetComponent<MapBlock>();
            if (cell != null && DataMng.E.MapData.IsHome)
            {
                cell.CancelClicking();
            }

            var recource = clickingObj.GetComponent<EntityResources>();
            if (recource != null)
            {
                recource.CancelClicking();
            }
        }
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
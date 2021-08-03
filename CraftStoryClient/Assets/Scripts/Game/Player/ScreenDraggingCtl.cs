using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenDraggingCtl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerDownHandler, IPointerUpHandler
{

    private bool isDrag;
    private bool isClick;
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Logger.Log("OnBeginDrag");

        isDrag = true;
        startPos = eventData.position;
    }

    
    public void OnDrag(PointerEventData eventData)
    {
        //Logger.Log("OnDrag");

        DebugLG.E.Add("pointerId: " + eventData.pointerId);

        if (eventData.pointerId == 0) touch1 = eventData.position;
        if (eventData.pointerId == 1) touch2 = eventData.position;

        if (touch1 != Vector2.zero && touch2 != Vector2.zero)
        {
            var newDistance = Vector2.Distance(touch1, touch2);
            var changeCameraV = curDistance - newDistance > 0 ? 1 : -1;
            DebugLG.E.Add(changeCameraV);
            PlayerCtl.E.CameraCtl.ChangeCameraPos(changeCameraV);
            curDistance = newDistance;
        }

        if (eventData.pointerId == 0 || eventData.pointerId == -1)
        {
            Vector2 pointerPos = eventData.position - startPos;
            if(PlayerCtl.E.CameraCtl != null) PlayerCtl.E.CameraCtl.CameraRotate(pointerPos.x, pointerPos.y);
            if (PlayerCtl.E.BlueprintPreviewCtl != null) PlayerCtl.E.BlueprintPreviewCtl.CameraRotate(pointerPos.x, pointerPos.y);

            startPos = eventData.position;

            PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Logger.Log("OnEndDrag");

        isDrag = false;
        isClick = false;
        IsClicking = false;
        clickingTime = 0;
    }

    Vector2 touch1;
    Vector2 touch2;
    float curDistance = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        //Logger.Log("OnPointerDown");
        //Logger.Log(eventData.position);

        this.eventData = eventData;
        isClick = true;



        if (eventData.pointerId == 0) touch1 = eventData.position;
        if (eventData.pointerId == 1) touch2 = eventData.position;
        if (touch1 != Vector2.zero && touch2 != Vector2.zero)
            curDistance = Vector2.Distance(touch1, touch2);

        DebugLG.E.Add("Click Count: " + eventData.clickCount);
        DebugLG.E.Add(eventData.ToString());
        if (eventData.clickCount == 3)
        {
            UICtl.E.OpenUI<DebugUI>(UIType.Debug);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Logger.Log("OnPointerUp");

        if (isDrag)
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

        if (eventData.pointerId == 0)
        {
            touch1 = Vector2.zero;
        }
        if (eventData.pointerId == 1)
        {
            touch2 = Vector2.zero;
        }
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
            var cell = clickingObj.GetComponent<EntityBase>();
            if (cell != null && DataMng.E.MapData.IsHome)
            {
                cell.CancelClicking();
            }
        }
    }

    private void OnClick(Vector2 pos)
    {
        var obj = RayCastHits(pos);
        if (obj == null)
            return;

        var createPos = _cacheRaycastHit.normal + _cacheRaycastHit.collider.transform.position;

        DirectionType dType = DirectionType.foward;
        CheckTouchPos(_cacheRaycastHit.collider.transform.position, createPos , out dType);
        PlayerCtl.E.OnClick(obj, createPos, dType);
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

    private void CheckTouchPos(Vector3 p1, Vector3 p2, out DirectionType touchType)
    {
        touchType = DirectionType.back;

        if (p1.y < p2.y)
        {
            touchType = DirectionType.up;
        }
        else if (p1.y > p2.y)
        {
            touchType = DirectionType.down;
        }
        else
        {
            if (p1.z == p2.z)
            {
                touchType = p1.x > p2.x ? DirectionType.left : DirectionType.right;
            }
            else
            {
                touchType = p1.z > p2.z ? DirectionType.back : DirectionType.foward;
            }
        }
    }
}

public enum DirectionType
{
    up,
    down,
    foward,
    back,
    right,
    left
}
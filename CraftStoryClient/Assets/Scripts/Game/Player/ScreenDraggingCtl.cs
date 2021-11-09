using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// プレイヤー命令コンソール
/// </summary>
public class ScreenDraggingCtl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerDownHandler, IPointerUpHandler
{

    private bool isDrag; // スクロールしているのタグ
    private bool isClick; // クリックしているのタグ
    private bool IsClicking
    {
        get => isClicking;
        set
        {
            isClicking = value;
        }
    }
    private bool isClicking;// 長い時間クリックしているのタグ

    Vector2 startPos; // スクロール始点
    PointerEventData eventData; 
    private RaycastHit _cacheRaycastHit; // レザー
    private GameObject clickingObj; // クリックしたGameObject

    float clickingTime; // 長い時間クリックした時間

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

    /// <summary>
    /// スクロール開始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Logger.Log("OnBeginDrag");

        if (PlayerCtl.E.Lock)
            return;

        isDrag = true;
        startPos = eventData.position;


        if (eventData.pointerId == 0) touch1 = eventData.position;
        if (eventData.pointerId == 1) touch2 = eventData.position;
        if (touch1 != Vector2.zero && touch2 != Vector2.zero)
            curDistance = Vector2.Distance(touch1, touch2);
    }

    /// <summary>
    /// スクロール中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerCtl.E.Lock || PlayerCtl.E.Character.IsDied)
            return;

        if (eventData.pointerId == 0) touch1 = eventData.position;
        if (eventData.pointerId == 1) touch2 = eventData.position;

        if (touch1 != Vector2.zero && touch2 != Vector2.zero)
        {
            var newDistance = Vector2.Distance(touch1, touch2);
            var changeCameraV = curDistance - newDistance > 0 ? -1 : 1;
            PlayerCtl.E.CameraCtl.ChangeCameraPos(changeCameraV);
            curDistance = newDistance;
        }
        else
        {
            Vector2 pointerPos = eventData.position - startPos;
            if (PlayerCtl.E.CameraCtl != null)
            {
                PlayerCtl.E.CameraCtl.CameraRotate(pointerPos.y, pointerPos.x);
                PlayerCtl.E.CameraCtl.CancelLockUn();
            }
            if (PlayerCtl.E.BlueprintPreviewCtl != null) PlayerCtl.E.BlueprintPreviewCtl.CameraRotate(pointerPos.y, pointerPos.x);

            startPos = eventData.position;
        }
    }

    /// <summary>
    /// スクロール終了
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (PlayerCtl.E.Lock)
            return;

        isDrag = false;
        isClick = false;
        IsClicking = false;
        clickingTime = 0;

        if (eventData.pointerId == 0) touch1 = Vector2.zero;
        if (eventData.pointerId == 1) touch2 = Vector2.zero;
    }

    Vector2 touch1; // トーチ点　１
    Vector2 touch2; // トーチ点　２
    float curDistance = 0; // トーチ点１，２間の距離

    /// <summary>
    /// トーチ開始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerCtl.E.Lock)
            return;

        this.eventData = eventData;
        isClick = true;
    }

    /// <summary>
    /// トーチ終了
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayerCtl.E.Lock)
            return;

        if (isDrag)
            return;

        if (!IsClicking)
            OnClick(eventData.position);

        if (IsClicking)
        {
            PlayerCtl.E.Character.Behavior = BehaviorType.Waiting;
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

    /// <summary>
    /// 長い時間クリック場合ロジック
    /// </summary>
    /// <param name="pos"></param>
    public void OnClicking(Vector2 pos)
    {
        if (PlayerCtl.E.Lock)
            return;

        clickingObj = RayCastHits(pos);
        if (clickingObj == null)
            return;

        PlayerCtl.E.OnClicking(Time.deltaTime, clickingObj);
    }
    /// <summary>
    /// 長い時間クリックキャンセルロジック
    /// </summary>
    public void CancelClicking()
    {
        if (PlayerCtl.E.Lock)
            return;

        if (clickingObj != null)
        {
            var cell = clickingObj.GetComponent<EntityBase>();
            if (cell != null && DataMng.E.RuntimeData.MapType == MapType.Home)
            {
                cell.CancelClicking();
            }
        }
    }

    /// <summary>
    /// クリック場合ロジック
    /// </summary>
    /// <param name="pos"></param>
    private void OnClick(Vector2 pos)
    {
        if (PlayerCtl.E.Lock)
            return;

        var obj = RayCastHits(pos);
        if (obj == null)
            return;


        var createPos = _cacheRaycastHit.normal + _cacheRaycastHit.point;
        int x = Mathf.RoundToInt(createPos.x);
        int y = Mathf.RoundToInt(createPos.y);
        int z = Mathf.RoundToInt(createPos.z);
        createPos = new Vector3(x, y, z);

        Direction dType = Direction.foward;
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

    /// <summary>
    /// トーチした向きを判断
    /// </summary>
    /// <param name="p1">トーチGameObject座標</param>
    /// <param name="p2">インスタンス座標</param>
    /// <param name="touchType"></param>
    private void CheckTouchPos(Vector3 p1, Vector3 p2, out Direction touchType)
    {
        touchType = Direction.back;

        if (p1.y < p2.y)
        {
            touchType = Direction.up;
        }
        else if (p1.y > p2.y)
        {
            touchType = Direction.down;
        }
        else
        {
            if (p1.z == p2.z)
            {
                touchType = p1.x > p2.x ? Direction.left : Direction.right;
            }
            else
            {
                touchType = p1.z > p2.z ? Direction.back : Direction.foward;
            }
        }
    }
}

/// <summary>
/// エンティティの向き
/// </summary>
public enum Direction
{
    up,
    down,
    foward,
    back,
    right,
    left
}
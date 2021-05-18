using UnityEngine;
using UnityEngine.EventSystems;

class ScreenDraggingCtl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerDownHandler, IPointerUpHandler
{
    public float offsetX;
    public float offsetY;

    private bool isDrag;
    private bool isClick;
    private bool isClicking;

    Vector2 startPos;

    float clickingTime;

    private void Update()
    {
        if (isClick)
        {
            clickingTime += Time.deltaTime;

            if (clickingTime > 0.5f)
            {
                OnClicking();
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

        PlayerEntity.E.Rotate(offsetX * 0.1f, offsetY * 0.1f);

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

        isClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");

        if (isDrag)
            return;

        if (!isClicking)
            PlayerEntity.E.CreateCube();

        isClick = false;
        isClicking = false;
        clickingTime = 0;
    }

    public void OnClicking()
    {
        isClicking = true;
        PlayerEntity.E.OnClicking(Time.deltaTime);
    }
}
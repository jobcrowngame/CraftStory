using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ScreenDraggingCtl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public float offsetX;
    public float offsetY;

    //private float rotateSpeed = 0.1f;

    Vector2 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Vector2 pointerPos = (eventData.position - startPos).normalized;
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
        offsetX = 0;
        offsetY = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount > 1)
        {
            PlayerEntity.E.CreateCube();

            eventData.clickCount = 0;
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIMouseEvents : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    public delegate void UIPointerEvent(PointerEventData eventData);
    public event UIPointerEvent OnDragging = delegate { };
    public event UIPointerEvent OnStartDrag = delegate { };
    public event UIPointerEvent OnStopDrag = delegate { };
    public event UIPointerEvent OnPointerEnterUI = delegate { };
    public event UIPointerEvent OnPointerExitUI = delegate { };
    public event UIPointerEvent OnPointerUpUI = delegate { };
    public event UIPointerEvent OnPointerDownUI = delegate { };

    bool m_isDragging;
    public bool m_IsDragging
    {
        get { return m_isDragging; }
    }

    bool m_isHovered;
    public bool m_IsHovered
    {
        get { return m_isHovered; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_isHovered = true;
        OnPointerEnterUI(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_isHovered = false;
        OnPointerExitUI(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragging(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDragging = true;
        OnStartDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_isDragging = false;
        OnStopDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpUI(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownUI(eventData);
    }
}
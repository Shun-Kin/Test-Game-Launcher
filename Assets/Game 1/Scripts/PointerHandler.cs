using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerHandler : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent onPointerDown;


    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown.Invoke();
    }
}

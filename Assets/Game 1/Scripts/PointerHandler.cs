using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private UnityEvent onPointerDown;


    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown.Invoke();
    }
}

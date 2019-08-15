using UnityEngine;
using UnityEngine.EventSystems;

public class Speed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static bool down;
  
    public void OnPointerDown(PointerEventData eventData)
    {
        down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        down = false;
    }
  
}

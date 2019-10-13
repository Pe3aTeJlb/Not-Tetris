using UnityEngine;
using UnityEngine.EventSystems;

public class Speed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static bool down;

    void Start() {
        down = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        down = false;
    }
  
}

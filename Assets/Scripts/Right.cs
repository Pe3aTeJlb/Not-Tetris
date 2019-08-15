using UnityEngine;
using UnityEngine.EventSystems;

public class Right : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static bool use;

    public void OnPointerDown(PointerEventData eventData)
    {
        use = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        use = false;
    }

}

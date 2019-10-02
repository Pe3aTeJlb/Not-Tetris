using UnityEngine;

public class Terminator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
    }

}

using UnityEngine;

public class GameOverDetector : MonoBehaviour
{
    public GameObject root;

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "floor" || collision.gameObject.tag == "shadow") {
           root.GetComponent<Not_Tetris>().GameOver();
        }

    }

}

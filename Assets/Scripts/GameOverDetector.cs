using UnityEngine;

public class GameOverDetector : MonoBehaviour
{
    public GameObject root, trigger_root;
    private string cache;

    public void OnTriggerStay2D(Collider2D collision)
    {
        cache = collision.gameObject.tag;

        if (cache == "floor" || cache == "fragment") {
              IntersectionCalculator.IsGameOver = true;
              root.GetComponent<Not_Tetris>().GameOver();
        }

    }

}

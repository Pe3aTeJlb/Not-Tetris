using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    int n = 0;
    public Rigidbody2D rb;


    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "floor" || collision.gameObject.tag == "shadow")
        {
            if (n < 1)
            {
                Not_Tetris.SpawnNewTetramino();
                this.tag = "floor";
                rb.gravityScale = 1;
                this.enabled = false;
                n++;
            }

        }

    }

}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Not_Tetris : MonoBehaviour
{
    public static GameObject root;

    public int moveSpeed, rotSpeed, startSpeed, accelSpeed, angularSpeed;
    public static int s_startSpeed;

    public bool PC;

    public GameObject[] Tetrominoes2;
    public static GameObject[] Tetrominoes;
    public static GameObject tetramino, spawn;

    public GameObject[] Preview;
    public static int nextTetramino;

    public static Rigidbody2D rb;
    public static int serialName = 0;

    public Text score_text, topScore_text, level_text, line_text;
    public static Text static_score_text, static_level_text, static_line_text;
    public static int Lines, Score, topScore, Level;

    public GameObject floor, terminator;
    public static bool gameover = false;

    public static bool StaticRotation;

    public float RotationSpeed;

    void Start()
    {

        gameover = false;
        Level = 1;
        Score = 0;
        Lines = 0;
        topScore = PlayerPrefs.GetInt("PhysicsHighScore", 0);

        static_line_text = line_text;
        static_level_text = level_text;
        static_score_text = score_text;

        topScore_text.text = "" + topScore;
        static_line_text.text = "" + Lines;
        static_level_text.text = "-";
        static_score_text.text = "" + Score;

        s_startSpeed = startSpeed;
        spawn = GameObject.FindGameObjectWithTag("spawn");
        Tetrominoes = Tetrominoes2;

        root = this.gameObject;
        nextTetramino = Random.Range(0, Tetrominoes.Length);
        //nextTetramino = 0;
        Preview[nextTetramino].SetActive(true);
        SpawnNewTetramino();

    }

    void Update()
    {

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Left.use == true)
        {
            rb.AddForce(Vector3.left * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Right.use == true) {
            rb.AddForce(Vector3.right * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.Z) || Input.GetKeyDown(KeyCode.Z) || TLeft.use == true)
        {
            if (StaticRotation == false) { rb.AddTorque(moveSpeed); }
            else if (StaticRotation == true) {tetramino.transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);}
        }
        else if (Input.GetKey(KeyCode.X) || Input.GetKeyDown(KeyCode.X) || TRight.use == true)
        {
            if (StaticRotation == false) { rb.AddTorque(-moveSpeed); }
            else if (StaticRotation == true) { tetramino.transform.Rotate(0, 0,-RotationSpeed*Time.deltaTime); }
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Speed.down == true)
        {
            rb.AddForce(Vector3.down * accelSpeed);
        }
        else { rb.velocity = new Vector2(rb.velocity.x, -3); }

        if (rb.angularVelocity > angularSpeed) { rb.angularVelocity = angularSpeed; }
        if (rb.angularVelocity < -angularSpeed) { rb.angularVelocity = -angularSpeed; }

        /*
        line_text.text = "" + Lines;
        level_text.text = "" + Level;
        score_text.text = "" + Score;
        */

    }

    public void NewTetromino()
    {

        Preview[nextTetramino].SetActive(false);
        nextTetramino = Random.Range(0, Tetrominoes.Length);
        //nextTetramino = 0;
        Preview[nextTetramino].SetActive(true);

    }

    public static void SpawnNewTetramino()
    {

        if (gameover == false)
        {

            tetramino = Instantiate(Tetrominoes[nextTetramino], spawn.transform.position, Quaternion.identity);
            tetramino.name = serialName.ToString();
            serialName++;
            rb = tetramino.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector3.down * s_startSpeed);

            root.GetComponent<Not_Tetris>().NewTetromino();
        }

    }

    public static void UpdateUI() {

        static_line_text.text = "" + Lines;
        //static_level_text.text = "" + Level;
        static_score_text.text = "" + Score;
        

    }

    public void GameOver()
    {

        if (Score > topScore)
        {
            PlayerPrefs.SetInt("PhysicsHighScore", Score);
        }
        StartCoroutine(Over());

    }

    public IEnumerator Over()
    {

        terminator.SetActive(false);
        gameover = true;
        rb.gameObject.tag = "shadow";
        rb.gravityScale = 1;
        rb = null;
        floor.SetActive(false);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);

    }

}

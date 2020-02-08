using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Classic_Tetris : MonoBehaviour 
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime;
    public static int height = 21;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];

    public bool PC;

    public GameObject[] Tetrominoes;
    public GameObject tetramino, spawn;
    public int nextTetramino;
    public GameObject[] Preview;

    public int needLines;

    public Text score_text, topScore_text, level_text, line_text;

    public int Lines, topScore, Level , Score;

    void Start()
    {
 
        Level = 0;
        needLines = 10;
        topScore =  PlayerPrefs.GetInt("HighScore", 0);
        topScore_text.text = "" + topScore;
        line_text.text = "" + Lines;
        level_text.text = "" + Level;
        score_text.text = "" + Score;

        nextTetramino = Random.Range(0, Tetrominoes.Length);
        Preview[nextTetramino].SetActive(true);
        SpawnNewTetramino();

        for (int i = 0; i < Menu.Level; i++) {
            LevelUp();
        }
        
    }

    void Update()
    {

        if(Time.time - previousTime > (Speed.down ? fallTime / 10 : fallTime))
        {

                tetramino.transform.position += new Vector3(0, -1, 0);
                if (!ValidMove())
                {
                    tetramino.transform.position -= new Vector3(0, -1, 0);

                    if (AboveGrid() == true)
                    {

                        GameOver();
                    }
                    else if (AboveGrid() == false)
                    {

                        AddToGrid();
                        CheckForLines();
                        SpawnNewTetramino();
                    }

                }
                previousTime = Time.time;
        }
       
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                tetramino.transform.position += new Vector3(-1, 0, 0);
                if (!ValidMove())
                    tetramino.transform.position -= new Vector3(-1, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                tetramino.transform.position += new Vector3(1, 0, 0);
                if (!ValidMove())
                    tetramino.transform.position -= new Vector3(1, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                //rotate !
                tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
                if (!ValidMove())
                    tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                //rotate !
                tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
                if (!ValidMove())
                    tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }

            if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
            {
                tetramino.transform.position += new Vector3(0, -1, 0);
                if (!ValidMove())
                {
                    tetramino.transform.position -= new Vector3(0, -1, 0);

                    if (AboveGrid() == true)
                    {
                       
                        GameOver(); }
                    else if(AboveGrid() == false){
                        
                        AddToGrid();
                        CheckForLines();
                        SpawnNewTetramino();
                    }

                }
                previousTime = Time.time;
            }

    }

    void NewTetromino()
    {

        Preview[nextTetramino].SetActive(false);
        nextTetramino = Random.Range(0, Tetrominoes.Length);
        Preview[nextTetramino].SetActive(true);
      
    }

    public void SpawnNewTetramino()
    {

        if (nextTetramino == 0) { rotationPoint = new Vector3(0.5f, 0.5f, 0); }
        else if (nextTetramino == 1) { rotationPoint = new Vector3(-0.5f, -0.5f, 0); }
        else { rotationPoint = new Vector3(0, 0, 0); }

        tetramino = Instantiate(Tetrominoes[nextTetramino], spawn.transform.position, Quaternion.identity);
        NewTetromino();
    }

    public void Right()
    {

        tetramino.transform.position += new Vector3(1, 0, 0);
        if (!ValidMove())
            tetramino.transform.position -= new Vector3(1, 0, 0);

    }

    public void Left()
    {

        tetramino.transform.position += new Vector3(-1, 0, 0);
        if (!ValidMove())
            tetramino.transform.position -= new Vector3(-1, 0, 0);

    }

    public void Turn_Right()
    {

        tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        if (!ValidMove())
            tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);

    }

    public void Turn_Left()
    {
 
        tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
        if (!ValidMove())
            tetramino.transform.RotateAround(tetramino.transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);

    }

    bool AboveGrid()
    {

        for (int x = 0; x < width; ++x)
        {
            foreach (Transform child in tetramino.transform)
            {
                Vector2 pos = new Vector2(Mathf.Round(child.transform.position.x),Mathf.Round(child.transform.position.y));
                if (pos.y > height -2) {
                    return true;
                }
            }
        }
        return false;

    }

    void CheckForLines()
    {
    
        int count=0;

        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                count++;
                Lines++;
                DeleteLine(i);
                RowDown(i);
            }

        }
        
        line_text.text = "" + Lines;
       
        if (count == 1) { Score += 100; }
        if (count == 2) { Score += 300; }
        if (count == 3) { Score += 700; }
        if (count == 4) { Score += 1500;}

        score_text.text = Score + "";

    }

    bool HasLine(int i)
    {

        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }

        return true;

    }

    void DeleteLine(int i)
    {
       
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }

        if (Lines >= needLines)
        {
            LevelUp();
        }

    }

    void RowDown(int i)
    {

        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }

    }

    void AddToGrid()
    {

        foreach (Transform children in tetramino.transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }

    }

    bool ValidMove()
    {

        foreach (Transform children in tetramino.transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
                return false;
        }

        return true;

    }

    public void LevelUp()
    {

            needLines += 10;
            Level++;
            level_text.text = "" + Level;

        if (fallTime > 0.1f)
        {
            fallTime = fallTime - 0.02f;
        }
        else {
            fallTime = fallTime - 0.01f;
        }
    
    }

    void GameOver()
    {

        if (Score > topScore) {
            PlayerPrefs.SetInt("HighScore", Score);
        }
        Menu.CallAdd();


        SceneManager.LoadScene("Main", LoadSceneMode.Single);

    }

}

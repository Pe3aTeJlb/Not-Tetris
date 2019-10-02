using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject ClassicCup, PhysicCup, menu, Triggers, ButtonsForClassic, ButtonsForPhysics, Terminator, PauseButton, buttonsCanvas, menuCanvas, infoCanvas;

    public Transform camera;
    public Vector3 CameraLeftUpperCorner;
    public float CupLeftUpperCorner;

    public bool pause, canPause;

    public static int Level;

    public Text levelText, fps;

    void Start()
    {
        Time.timeScale = 1;

        buttonsCanvas.SetActive(false);
        infoCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        CameraLeftUpperCorner = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, 0));

        Level = 1;
        levelText.text = "" + Level;

        menu.SetActive(true);
        ClassicCup.SetActive(false);
        PhysicCup.SetActive(false);
        Triggers.SetActive(false);
        ButtonsForClassic.SetActive(false);
        ButtonsForPhysics.SetActive(false);
        Terminator.SetActive(false);

    }

    public void Update()
    {
        fps.text = "" + 1 / Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape) && pause == true && canPause == true)
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && pause != true && canPause == true)
        {

            Time.timeScale = 0;
            pause = true;
            PauseButton.SetActive(true);
        }


    }

    public void Classic()
    {

        buttonsCanvas.SetActive(true);
        infoCanvas.SetActive(true);

        canPause = true;
        menu.SetActive(false);
        ClassicCup.SetActive(true);
        ButtonsForClassic.SetActive(true);
        menuCanvas.GetComponent<Classic_Tetris>().enabled = true;


    }

    public void Not_Classic()
    {

        buttonsCanvas.SetActive(true);
        infoCanvas.SetActive(true);
        canPause = true;
        menu.SetActive(false);
        PhysicCup.SetActive(true);
        ButtonsForPhysics.SetActive(true);
        Triggers.SetActive(true);
        Terminator.SetActive(true);
        menuCanvas.GetComponent<Not_Tetris>().enabled = true;

    }

    public void ContinueGame()
    {

        PauseButton.SetActive(false);
        pause = false;
        Time.timeScale = 1;

    }


    public void IncreaseLevel()
    {
        Level++;
        levelText.text = "" + Level;
    }

    public void DecreaseLevel()
    {

        if (Level > 1)
        {
            Level--;
            levelText.text = "" + Level;
        }
        else { }

    }

}

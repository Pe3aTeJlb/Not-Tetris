using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject ClassicCup, PhysicCup, menu, Canvas, Triggers, ButtonsForClassic, ButtonsForPhysics, Terminator, PauseButton;

    public Transform camera;
    public Vector3 CameraLeftUpperCorner;
    public float CupLeftUpperCorner;

    public bool pause, canPause;

    void Start()
    {
        Time.timeScale = 1;

        CameraLeftUpperCorner = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, 0));
        
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
        canPause = true;
        menu.SetActive(false);
        ClassicCup.SetActive(true);
        ButtonsForClassic.SetActive(true);
        Canvas.GetComponent<Classic_Tetris>().enabled = true;


    }

    public void Not_Classic()
    {
        canPause = true;
        menu.SetActive(false);
        PhysicCup.SetActive(true);
        ButtonsForPhysics.SetActive(true);
        Triggers.SetActive(true);
        Terminator.SetActive(true);
        Canvas.GetComponent<Not_Tetris>().enabled = true;

    }

    public void ContinueGame() {

        PauseButton.SetActive(false);
        pause = false;
        Time.timeScale = 1;

    }







}

﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class Menu : MonoBehaviour
{
    public GameObject ClassicCup, PhysicCup, menu, Triggers, ButtonsForClassic, ButtonsForPhysics, Terminator, PauseButton, menuCanvas;

    public Transform camera;
    public Vector3 CameraLeftUpperCorner;
    public float CupLeftUpperCorner;

    public bool pause, canPause;

    public static int Level;

    public Text levelText, fps;

    public Canvas ButtonsUI, MenuUI, InfoUI;

    //Unity ADS
    public bool CanShowAdd;
    Coroutine timer;

    private string gameId = "3446907";
    private static string placementId = "video";
    private bool testMode = false;
    public bool PressRelease = false;


    void Start()
    {
        Application.targetFrameRate = 30;

        Time.timeScale = 1;

        ButtonsUI.enabled = false;
        InfoUI.enabled = false;
        MenuUI.enabled = true;

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

        CanShowAdd = false;

        Advertisement.Initialize(gameId, testMode);

    }

    public void Update()
    {
        //fps.text = "" + 1 / Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && pause == true && canPause == true)
        {
            if (PressRelease == false) {

                if (CanShowAdd == false)
                {
                    StopCoroutine(timer);
                }
                else
                {
                    if (Advertisement.IsReady(placementId)) {
                        Advertisement.Show();
                    }
                }

            }
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
        timer = StartCoroutine(IngameTimeForAdd());

        ButtonsUI.enabled = true;
        InfoUI.enabled = true;

        canPause = true;
        menu.SetActive(false);
        ClassicCup.SetActive(true);
        ButtonsForClassic.SetActive(true);
        menuCanvas.GetComponent<Classic_Tetris>().enabled = true;

    }

    public void Not_Classic()
    {
        timer = StartCoroutine(IngameTimeForAdd());

        ButtonsUI.enabled = true;
        InfoUI.enabled = true;

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

    public IEnumerator IngameTimeForAdd(){
        yield return new WaitForSeconds(15);
        CanShowAdd = true;
    }

    public static void CallAdd() {

        if (Advertisement.IsReady(placementId))
        {
            Advertisement.Show();
        }

    }

}

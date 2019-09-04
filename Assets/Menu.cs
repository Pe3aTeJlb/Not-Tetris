using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject ClassicCup, PhysicCup, menu, Canvas, Triggers, ButtonsForClassic, ButtonsForPhysics, Terminator;
    public static bool fuckPhysics;

    public Transform camera;
    public Vector3 CameraLeftUpperCorner;
    public float CupLeftUpperCorner;

    void Start()
    {
        Debug.Log(Camera.main.pixelRect);
        CameraLeftUpperCorner = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, 0));
        
        menu.SetActive(true);
        ClassicCup.SetActive(false);
        PhysicCup.SetActive(false);
        Triggers.SetActive(false);
        ButtonsForClassic.SetActive(false);
        ButtonsForPhysics.SetActive(false);
        Terminator.SetActive(false);
        
    }

    public void Classic()
    {

       // camera.position = new Vector3(camera.position.x - CameraLeftUpperCorner.x - CupLeftUpperCorner, camera.position.y, camera.position.z);

        menu.SetActive(false);
        ClassicCup.SetActive(true);
        ButtonsForClassic.SetActive(true);
        Canvas.GetComponent<Classic_Tetris>().enabled = true;

    }

    public void Not_Classic()
    {

      //  camera.position = new Vector3(camera.position.x - CameraLeftUpperCorner.x - 2.5f, camera.position.y, camera.position.z);

        menu.SetActive(false);
        PhysicCup.SetActive(true);
        ButtonsForPhysics.SetActive(true);
        Triggers.SetActive(true);
        Terminator.SetActive(true);
        fuckPhysics = false;
        Canvas.GetComponent<Not_Tetris>().enabled = true;

    }

    public void FuckPhysics()
    {

        menu.SetActive(false);
        PhysicCup.SetActive(true);
        ButtonsForPhysics.SetActive(true);
        Triggers.SetActive(true);
        Terminator.SetActive(true);
        fuckPhysics = true;
        Canvas.GetComponent<Not_Tetris>().enabled = true;

    }

}

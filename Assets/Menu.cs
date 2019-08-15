using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject Cup1, Cup2, menu,canvas, triggers,buttonsForClassic,buttonsForPhysics,terminator;
    public GameObject background1;
    public static bool fuckPhysics;
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(true);
        Cup1.SetActive(false);
        Cup2.SetActive(false);
        background1.SetActive(false);
        triggers.SetActive(false);
        buttonsForClassic.SetActive(false);
        buttonsForPhysics.SetActive(false);
        terminator.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Classic() {
        menu.SetActive(false);
        Cup1.SetActive(true);
        buttonsForClassic.SetActive(true);
        background1.SetActive(true);
        canvas.GetComponent<Classic_Tetris>().enabled = true;
    }
    public void Not_Classic()
    {
        menu.SetActive(false);
        Cup2.SetActive(true);
        buttonsForPhysics.SetActive(true);
        triggers.SetActive(true);
        terminator.SetActive(true);
        fuckPhysics = false;
        canvas.GetComponent<Not_Tetris>().enabled = true;

    }
    public void FuckPhysics() {
        menu.SetActive(false);
        Cup2.SetActive(true);
        buttonsForPhysics.SetActive(true);
        triggers.SetActive(true);
        terminator.SetActive(true);
        fuckPhysics = true;
        canvas.GetComponent<Not_Tetris>().enabled = true;

    }

}

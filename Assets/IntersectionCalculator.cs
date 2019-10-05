using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySpriteCutter;

public class IntersectionCalculator : MonoBehaviour, IComparable
{
    public float squareArea;
    public int requareOnce = 0;
    public int requareOnce2 = 0;

    public Vector2 upperBoundStart, upperBoundEnd, lowerBoundStart, lowerBoundEnd;
    public LayerMask CalculationMask, CuttingMask;

    public Vector2[] buffVector;
    public float buff;


    public List<Vector2> intersectionAreaPoints = new List<Vector2>();
    public List<GameObject> list = new List<GameObject>();
    public List<GameObject> toDelete = new List<GameObject>();
    public Dictionary<GameObject, float> intersectionSegments = new Dictionary<GameObject, float>();
    public List<GameObject> toEnable = new List<GameObject>();

    public bool ready;
    public bool inside1, inside2;

    public float win;
    public Text text;
    public SpriteRenderer sp;
    public SpriteRenderer fillingIcon, fillingIcon2;

    GameObject current;

    float segmentArea, mass;

    public bool Cut;

    public void Start()
    {

        sp = this.GetComponent<SpriteRenderer>();
        SetFillingLine();
       // fillingIcon.size = new Vector2((2 / win * squareArea), 1f);
        //fillingIcon2.size = new Vector2((2 / win * squareArea), 1f);

    }

    /*
    public void Update()
    {

        //text.text = "" + squareArea;

        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {

            Time.timeScale = 1;
            StartCoroutine(wait());
            //list.Clear();
            //intersectionSegments.Clear();
            //toDelete.Clear();
            //toEnable.Clear();

            //Not_Tetris.Score += Mathf.RoundToInt(10 * squareArea);

            //squareArea = 0;
            //ready = false;
            //requareOnce = 0;

            //sp.enabled = false;
            //current.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            //Not_Tetris.Lines += 1;

        }

    }*/

    public void OnTriggerEnter2D(Collider2D collision)
    {
       
        try
        {
            //StartCoroutine(CheckVelocity(collision));
            KekVelocity(collision);
            
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        KekVelocity(collision);


        //if (toDelete.Contains(collision.gameObject) != true && (collision.gameObject.tag == "shadow" || collision.gameObject.tag == "tetramino") && list.Contains(collision.gameObject) && collision.GetComponent<PolygonCollider2D>().bounds.center.y < upperBoundStart.y && collision.GetComponent<PolygonCollider2D>().bounds.center.y > lowerBoundStart.y)
        if (toDelete.Contains(collision.gameObject) != true && (collision.gameObject.tag == "shadow" || collision.gameObject.tag == "tetramino")  && collision.GetComponent<PolygonCollider2D>().bounds.center.y < upperBoundStart.y && collision.GetComponent<PolygonCollider2D>().bounds.center.y > lowerBoundStart.y)
        {
              toDelete.Add(collision.gameObject);
        }

        if (intersectionSegments.ContainsKey(collision.gameObject) == true && list.Contains(collision.gameObject) && collision.gameObject.tag == "shadow"  && collision.GetComponent<Rigidbody2D>().velocity.magnitude > 0.8f)
        {
            //Debug.Log("Recalculate " + collision.gameObject.name + " " + collision.GetComponent<Rigidbody2D>().velocity.magnitude);
            squareArea -= intersectionSegments[collision.gameObject];
            intersectionSegments.Remove(collision.gameObject);
            list.Remove(collision.gameObject);
            //StartCoroutine(CheckVelocity(collision));
            KekVelocity(collision);
        }



    }

    public void OnTriggerExit2D(Collider2D collision)
    {

        if (toDelete.Contains(collision.gameObject) == true)
        {
            toDelete.Remove(collision.gameObject);
        }
        if (list.Contains(collision.gameObject) == true)
        {
            list.Remove(collision.gameObject);
        }
        if (intersectionSegments.ContainsKey(collision.gameObject) == true)
        {
            squareArea -= intersectionSegments[collision.gameObject];
            if (squareArea < 0) { squareArea = 0; }
            intersectionSegments.Remove(collision.gameObject);
            SetFillingLine();
        }

    }

    public void KekVelocity(Collider2D collision) {

        //Debug.Log("KekVelocity");

        if (collision.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f) {

            if (collision.gameObject.GetComponent<MeshRenderer>() != null)
            {
                collision.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            if (list.Contains(collision.gameObject) != true && (collision.gameObject.tag == "floor" || collision.gameObject.tag == "shadow"))
            {
                list.Add(collision.gameObject);
                FindBounds(collision);
            }

        }

    }

    public IEnumerator CheckVelocity(Collider2D collision)
    {
       // Debug.Log("CheckVelocity");
        try
        {
            yield return new WaitUntil(() => collision.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f);

            if (collision.gameObject.GetComponent<MeshRenderer>() != null)
            {
                collision.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            if (list.Contains(collision.gameObject) != true && (collision.gameObject.tag == "floor" || collision.gameObject.tag == "shadow"))
            {
                list.Add(collision.gameObject);
                FindBounds(collision);
            }

        }
        finally { }

    }

    //находим границы и составляем массив
    public void FindBounds(Collider2D collision)
    {
        //Debug.Log("Founding Bounds");
        collision.gameObject.layer = 9;

        inside1 = false;
        inside2 = false;

        PolygonCollider2D pl = collision.gameObject.GetComponent<PolygonCollider2D>();

        buffVector = collision.gameObject.GetComponent<PolygonCollider2D>().points;
        buff = buffVector[0].y;

        for (int i = 0; i < pl.points.Length; i++)
        {
            buffVector[i] = pl.transform.TransformPoint(pl.points[i]);
        }

        ////
        for (int i = 0; i < buffVector.Length; i++)
        {
            if (buffVector[i].y > buff)
            {
                buff = buffVector[i].y;
            }
        }

        if (buff > upperBoundStart.y)
        {

            RaycastHit2D[] hits = Physics2D.LinecastAll(upperBoundStart, upperBoundEnd, CalculationMask);
            RaycastHit2D[] reversHits = Physics2D.LinecastAll(upperBoundEnd, upperBoundStart, CalculationMask);

            if (hits.Length != 0)
            {
                intersectionAreaPoints.Add(hits[0].point);
            }

            if (reversHits.Length != 0)
            {
                intersectionAreaPoints.Add(reversHits[0].point);
            }

        }
        else
        {
            inside1 = true;
        }

        ////
        for (int i = 0; i < buffVector.Length; i++)
        {
            if (buffVector[i].y < buff)
            {
                buff = buffVector[i].y;
            }
        }

        if (buff < lowerBoundStart.y)
        {
            RaycastHit2D[] hits2 = Physics2D.LinecastAll(lowerBoundStart, lowerBoundEnd, CalculationMask);
            RaycastHit2D[] reversHits2 = Physics2D.LinecastAll(lowerBoundEnd, lowerBoundStart, CalculationMask);

            if (hits2.Length != 0)
            {
                intersectionAreaPoints.Add(hits2[0].point);
            }
            if (reversHits2.Length != 0)
            {
                intersectionAreaPoints.Add(reversHits2[0].point);
            }
        }
        else
        {
            inside2 = true;
        }

        ////
        for (int i = 0; i < buffVector.Length; i++)
        {
           //Debug.Log(buffVector[i]);
            if (buffVector[i].y > lowerBoundStart.y && buffVector[i].y < upperBoundStart.y)
            {
                buffVector[i].x = ((int)(buffVector[i].x * 10) / 10f);
                buffVector[i].y = ((int)(buffVector[i].y * 10) / 10f);
                intersectionAreaPoints.Add(buffVector[i]);
            }
        }

        if (inside1 == true && inside2 == true)
        {
            collision.gameObject.layer = 8;
            buffVector = new Vector2[0];

            if (toDelete.Contains(collision.gameObject) != true)
            {
                toDelete.Add(collision.gameObject);
            }

            CalculateIntersection(collision);
        }
        else
        {
            collision.gameObject.layer = 8;
            buffVector = new Vector2[0];
           // intersectionAreaPoints.Sort((a, b) => a.x.CompareTo(b.y));

            CalculateIntersection(collision);
        }

    }

    public void CalculateIntersection(Collider2D collision)
    {

        //Debug.Log("Calculation Started");

        float buff_squareArea = 0;
        
        
        if (intersectionAreaPoints.Count > 3)
        {
            for (int i = 0; i < intersectionAreaPoints.Count - 2; i++)
            {

                buff_squareArea += SquareByGeron(intersectionAreaPoints[i], intersectionAreaPoints[i + 1], intersectionAreaPoints[i + 2]);
            }
        }
            
        //Debug.Log("Square Area" + buff_squareArea);

        intersectionSegments.Add(collision.gameObject, buff_squareArea);
        squareArea += buff_squareArea;

        text.text = "" + squareArea;

        buff = 0;
        intersectionAreaPoints.Clear();
        inside1 = false;
        inside2 = false;

        /*
        byte bt = (byte)(255 - (int)((255 / win) * squareArea));
        fillingIcon.size = new Vector2((2 / win * squareArea), 1f);
        fillingIcon.color = new Color32(bt, bt, bt, 255);
        fillingIcon2.size = new Vector2((2 / win * squareArea), 1f);
        fillingIcon2.color = new Color32(bt, bt, bt, 255);
        */
        SetFillingLine();

        if (squareArea >= win)
        {
            if (requareOnce < 1)
            {
                //fillingIcon.size = new Vector2((2 / win * squareArea), 1f);
                //fillingIcon2.size = new Vector2((2 / win * squareArea), 1f);
                SetFillingLine();
                requareOnce++;

                sp.enabled = true;

                current = GameObject.FindGameObjectWithTag("tetramino");
                current.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

                StartCoroutine(wait());
            }

        }
    }

    void LinecastCut(Vector2 lineStart, Vector2 lineEnd, int layerMask = Physics2D.AllLayers)
    {

        ready = false;
        List<GameObject> gameObjectsToCut = new List<GameObject>();
        RaycastHit2D[] hits = Physics2D.LinecastAll(lineStart, lineEnd, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (HitCounts(hit))
            {
                gameObjectsToCut.Add(hit.transform.gameObject);
            }
        }

        foreach (GameObject go in gameObjectsToCut)
        {

                segmentArea = 4;
                if (intersectionSegments.ContainsKey(go))
                {
                    segmentArea = intersectionSegments[go];
                }
                mass = go.GetComponent<Rigidbody2D>().mass;

            

            SpriteCutterOutput output = SpriteCutter.Cut(new SpriteCutterInput()
            {
                lineStart = lineStart,
                lineEnd = lineEnd,
                gameObject = go,
                gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
            });


            if (output != null && output.secondSideGameObject != null)
            {
                output.firstSideGameObject.tag = "shadow";
                output.secondSideGameObject.tag = "shadow";

                Rigidbody2D newRigidbody = output.secondSideGameObject.AddComponent<Rigidbody2D>();
                newRigidbody.gravityScale = 1;

                output.firstSideGameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                output.secondSideGameObject.GetComponent<Rigidbody2D>().isKinematic = true;



                    float mass1 = mass * segmentArea / 4;
                    float mass2 = mass - (mass * segmentArea / 4);

                    if (mass1 < 0.2f)
                    {
                        mass1 = 0.2f;
                    }
                    if (mass2 < 0.2f)
                    {
                        mass2 = 0.2f;
                    }

                    if (output.firstSideGameObject.GetComponent<Collider2D>().bounds.size.magnitude > output.secondSideGameObject.GetComponent<Collider2D>().bounds.size.magnitude)
                    {
                        output.firstSideGameObject.GetComponent<Rigidbody2D>().mass = mass1;
                        output.secondSideGameObject.GetComponent<Rigidbody2D>().mass = mass2;
                    }
                    else
                    {
                        output.secondSideGameObject.GetComponent<Rigidbody2D>().mass = mass2;
                        output.firstSideGameObject.GetComponent<Rigidbody2D>().mass = mass1;
                    }

                

                toEnable.Add(output.firstSideGameObject);
                toEnable.Add(output.secondSideGameObject);

            }
        }

        ready = true;
    }

    bool HitCounts(RaycastHit2D hit)
    {
        return (hit.transform.GetComponent<SpriteRenderer>() != null ||
                 hit.transform.GetComponent<MeshRenderer>() != null);
    }

    public IEnumerator ClearLine()
    {

        yield return new WaitUntil(() => ready == true);
        yield return new WaitForSeconds(.5f);

       // Time.timeScale = 0;

        for (int i = 0; i < toDelete.Count; i++)
        {
            try
            {

                toDelete[i].layer = 10;
               

            }
            catch (Exception e) { Debug.Log(e); }
        }

        for (int i = 0; i < toEnable.Count; i++)
        {
            if (toEnable[i].transform != null && toEnable[i].GetComponent<Rigidbody2D>() != null)
            {
                toEnable[i].GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        list.Clear();
        intersectionSegments.Clear();
        toDelete.Clear();
        toEnable.Clear();

        squareArea = 0;
        ready = false;
        requareOnce = 0;

        sp.enabled = false;
        SetFillingLine();
        current.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        Not_Tetris.Score += Mathf.RoundToInt(10 * squareArea);
        Not_Tetris.Lines += 1;
        Not_Tetris.UpdateUI();

    }

    public int CompareTo(object obj)
    {
        throw new NotImplementedException();
    }

    public float SquareByGeron(Vector2 FirstPoint, Vector2 SecondPoint, Vector2 ThirdPoint)
    {

        float FirstSide = (FirstPoint - SecondPoint).magnitude;
        float SecondSide = (SecondPoint - ThirdPoint).magnitude;
        float ThirdSide = (ThirdPoint - FirstPoint).magnitude;

        float halfPerimetr = (FirstSide + SecondSide + ThirdSide) / 2;

        return (Mathf.Sqrt(halfPerimetr * (halfPerimetr - FirstSide) * (halfPerimetr - SecondSide) * (halfPerimetr - ThirdSide)));

    }

    public IEnumerator wait()
    {

        LinecastCut(upperBoundStart, upperBoundEnd, CuttingMask);
        yield return new WaitForSeconds(0.1f);
        LinecastCut(lowerBoundStart, lowerBoundEnd, CuttingMask);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ClearLine());

    }

    public void SetFillingLine() {
        byte bt = 0;

        if ((255 - (int)((255 / win) * squareArea)) >= 0)
        {
             bt = (byte)(255 - (int)((255 / win) * squareArea));
             Debug.Log(bt);
        }
        else{ bt = 0; }
        
        fillingIcon.size = new Vector2((2 / win * squareArea), 1f);
        fillingIcon.color = new Color32(bt, bt, bt, 255);
        fillingIcon2.size = new Vector2((2 / win * squareArea), 1f);
        fillingIcon2.color = new Color32(bt, bt, bt, 255);

    }

}
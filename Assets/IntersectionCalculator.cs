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

    public List<Vector2> buffVector = new List<Vector2>();
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
    int f = 0;

    Coroutine Velocity;

    byte bt = 0;
    int data = 0;

    public void Start()
    {

        sp = this.GetComponent<SpriteRenderer>();
        SetFillingLine();

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            intersectionAreaPoints.Clear();
            buffVector.Clear();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
       
        try
        {
            Velocity = StartCoroutine(CheckVelocity(collision));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        //if (toDelete.Contains(collision.gameObject) != true && (collision.gameObject.tag == "shadow" || collision.gameObject.tag == "tetramino") && list.Contains(collision.gameObject) && collision.GetComponent<PolygonCollider2D>().bounds.center.y < upperBoundStart.y && collision.GetComponent<PolygonCollider2D>().bounds.center.y > lowerBoundStart.y)
        if (toDelete.Contains(collision.gameObject) != true && (collision.gameObject.tag == "shadow" || collision.gameObject.tag == "tetramino")  && collision.GetComponent<PolygonCollider2D>().bounds.center.y < upperBoundStart.y && collision.GetComponent<PolygonCollider2D>().bounds.center.y > lowerBoundStart.y)
        {
              toDelete.Add(collision.gameObject);
        }

        if (intersectionSegments.ContainsKey(collision.gameObject) == true && list.Contains(collision.gameObject) && (collision.gameObject.tag == "shadow") && collision.GetComponent<Rigidbody2D>().velocity.magnitude > 0.8f)
        {
            //Debug.Log("Recalculate " + collision.gameObject.name + " " + collision.GetComponent<Rigidbody2D>().velocity.magnitude);
            squareArea -= intersectionSegments[collision.gameObject];
            intersectionSegments.Remove(collision.gameObject);
            list.Remove(collision.gameObject);
            Velocity = StartCoroutine(CheckVelocity(collision));
        }

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Stop" + collision.gameObject.name);
        StopCoroutine(Velocity);
         
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
            list.Remove(collision.gameObject);
            squareArea -= intersectionSegments[collision.gameObject];
            intersectionSegments.Remove(collision.gameObject);
            //SetFillingLine();
        }

    }

    public IEnumerator CheckVelocity(Collider2D collision)
    {

        if (collision.gameObject == null) {
            StopCoroutine(Velocity);
        }

        try
        {
           // if (collision.gameObject != null)
          //  {
                yield return new WaitUntil(() => collision.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f);

                if (collision.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    collision.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }

                if (list.Contains(collision.gameObject) != true && (collision.gameObject.tag == "shadow"))
                {
                    //Debug.Log("CheckVelocity " + this.gameObject.name);
                    list.Add(collision.gameObject);
                    FindBounds(collision);
                }
            //}

        }
        finally { }

    }

    //находим границы и составляем массив
    public void FindBounds(Collider2D collision)
    {
        collision.gameObject.layer = 9;

        bool upstart=false, lowstart = false;
        bool clockwise = false;

        RaycastHit2D[] hits = Physics2D.LinecastAll(upperBoundStart, upperBoundEnd, CalculationMask);
        RaycastHit2D[] reversHits = Physics2D.LinecastAll(upperBoundEnd, upperBoundStart, CalculationMask);
        RaycastHit2D[] hits2 = Physics2D.LinecastAll(lowerBoundStart, lowerBoundEnd, CalculationMask);
        RaycastHit2D[] reversHits2 = Physics2D.LinecastAll(lowerBoundEnd, lowerBoundStart, CalculationMask);

       // Debug.Log(hits[0] + " " + reversHits[0] + " " + hits2[0] + " " + reversHits2[0]);
        
       // inside1 = false;
       // inside2 = false;

        PolygonCollider2D pl = collision.gameObject.GetComponent<PolygonCollider2D>();

        for (int i = 0; i < pl.points.Length; i++)
        {
            buffVector.Add(pl.transform.TransformPoint(pl.points[i]));
        }

        buffVector.Add(buffVector[0]);

        Debug.Log("Founding Bound in " + this.name);

        Debug.Log("Vector length " + buffVector.Count);

        float dich = 0;

        for (int i = 0; i < buffVector.Count-1; i++) {

            dich += (buffVector[i].x * buffVector[i + 1].y) - (buffVector[i + 1].x * buffVector[i].y);
        }

        Debug.Log("dich " + dich);
        if (dich > 0) { Debug.Log("!clocwise");  clockwise = false; } else { Debug.Log("clocwise"); clockwise = true; }
     
        for (int i = 0; i < buffVector.Count; i++)
        {
            if (clockwise)
            {
                if (buffVector[i].y > upperBoundStart.y && upstart != true)
                {

                    Debug.Log("Iteration " + i + "\n" + "buffV.y > upper " + hits[0].point);
                    upstart = true;
                    intersectionAreaPoints.Add(hits[0].point);

                }
                else if (buffVector[i].y < upperBoundStart.y && upstart == true)
                {

                    Debug.Log("Iteration " + i + "\n" + "buffV.y < upper " + reversHits[0].point + " " + buffVector[i]);
                    upstart = false;

                    intersectionAreaPoints.Add(reversHits[0].point);
                    intersectionAreaPoints.Add(buffVector[i]);

                }
                else if (buffVector[i].y < lowerBoundStart.y && lowstart != true)
                {

                    Debug.Log("Iteration " + i + "\n" + "buffV.y < lower " + reversHits2[0].point);
                    lowstart = true;
                    intersectionAreaPoints.Add(reversHits2[0].point);

                }
                else if (buffVector[i].y > lowerBoundStart.y && lowstart == true)
                {
                    Debug.Log("Iteration " + i + "\n" + "buffV.y > lower " + hits2[0].point + " " + buffVector[i]);
                    lowstart = false;

                    intersectionAreaPoints.Add(hits2[0].point);
                    intersectionAreaPoints.Add(buffVector[i]);

                }
                else if (lowstart == false && upstart == false)
                {
                    Debug.Log("Iteration " + i + "\n" + "just add " + buffVector[i]);
                    intersectionAreaPoints.Add(buffVector[i]);
                }
            }
            else {

                if (buffVector[i].y > upperBoundStart.y && upstart != true)
                {

                    Debug.Log("Iteration " + i + "\n" + "buffV.y > upper " + reversHits[0].point);
                    upstart = true;
                    intersectionAreaPoints.Add(reversHits[0].point);

                }
                else if (buffVector[i].y < upperBoundStart.y && buffVector[i].y < lowerBoundStart.y && upstart == true)
                {

                    Debug.Log("Iteration " + i + "\n" + "buffV.y < upper " + hits[0].point + " " + buffVector[i]);
                    upstart = false;

                    intersectionAreaPoints.Add(hits[0].point);
                    intersectionAreaPoints.Add(buffVector[i]);

                }
                else if (buffVector[i].y < lowerBoundStart.y && lowstart != true)
                {

                    Debug.Log("Iteration " + i + "\n" + "buffV.y < lower " + hits2[0].point);
                    lowstart = true;
                    intersectionAreaPoints.Add(hits2[0].point);

                }
                else if (buffVector[i].y > lowerBoundStart.y && buffVector[i].y < upperBoundStart.y && lowstart == true)
                {
                    Debug.Log("Iteration " + i + "\n" + "buffV.y > lower " + reversHits2[0].point + " " + buffVector[i]);
                    lowstart = false;

                    intersectionAreaPoints.Add(reversHits2[0].point);
                    intersectionAreaPoints.Add(buffVector[i]);

                }
                else if (lowstart == false && upstart == false)
                {
                    Debug.Log("Iteration " + i + "\n" + "just add " + buffVector[i]);
                    intersectionAreaPoints.Add(buffVector[i]);
                }

            }

        }

        collision.gameObject.layer = 8;

        if (toDelete.Contains(collision.gameObject) != true)
        {
            toDelete.Add(collision.gameObject);
        }

        CalculateIntersection(collision);

        #region
        /*
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
                //buffVector[i].x = ((int)(buffVector[i].x * 10) / 10f);
                //buffVector[i].y = ((int)(buffVector[i].y * 10) / 10f);
                intersectionAreaPoints.Add(buffVector[i]);
            }
        }

        buffList = intersectionAreaPoints;
        Triangulated = Triangulation.GetResult(buffList,true);

        if (inside1 == true && inside2 == true)
        {
            collision.gameObject.layer = 8;
            //buffVector = new Vector2[0];

            if (toDelete.Contains(collision.gameObject) != true)
            {
                toDelete.Add(collision.gameObject);
            }

           // CalculateIntersection(collision);
        }
        else
        {
            collision.gameObject.layer = 8;
            //buffVector = new Vector2[0];
            //intersectionAreaPoints.Sort((a, b) => a.x.CompareTo(b.y));

            //CalculateIntersection(collision);
        }
        */
#endregion

    }

    public void CalculateIntersection(Collider2D collision)
    {

        //Debug.Log("Calculation Started");

        float buff_squareArea = 0;

        for (int i = 0; i < intersectionAreaPoints.Count - 1; i++)
        {
            buff_squareArea += (intersectionAreaPoints[i].x * intersectionAreaPoints[i + 1].y) - (intersectionAreaPoints[i + 1].x * intersectionAreaPoints[i].y);
        }

        buff_squareArea = Mathf.Abs(buff_squareArea) / 2;

        Debug.Log("Square Area" + buff_squareArea);

        intersectionSegments.Add(collision.gameObject, buff_squareArea);
        squareArea += buff_squareArea;

        text.text = "" + squareArea;

        
        //buffVector.Clear();
      //  intersectionAreaPoints.Clear();


        //buff = 0;
        // inside1 = false;
        // inside2 = false;

        SetFillingLine();

        if (squareArea >= win)
        {
            if (requareOnce < 1)
            {
               
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

                f++;
                Debug.Log("Iteration "+f+"\n" +"Place: " + this.name + "\n" + output.firstSideGameObject.name + "\n" + output.secondSideGameObject.name);

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

        Not_Tetris.Score += Mathf.RoundToInt(10 * squareArea);
        Not_Tetris.Lines += 1;
        Not_Tetris.UpdateUI();

        squareArea = 0;
        ready = false;
        requareOnce = 0;

        sp.enabled = false;
        SetFillingLine();
        current.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

    }

    public int CompareTo(object obj)
    {
        throw new NotImplementedException();
    }

    /*
    public float SquareByGeron(Vector2 FirstPoint, Vector2 SecondPoint, Vector2 ThirdPoint)
    {

        float FirstSide = (FirstPoint - SecondPoint).magnitude;
        float SecondSide = (SecondPoint - ThirdPoint).magnitude;
        float ThirdSide = (ThirdPoint - FirstPoint).magnitude;

        float halfPerimetr = (FirstSide + SecondSide + ThirdSide) / 2;

        return (Mathf.Sqrt(halfPerimetr * (halfPerimetr - FirstSide) * (halfPerimetr - SecondSide) * (halfPerimetr - ThirdSide)));

    }
    */

    public IEnumerator wait()
    {

        LinecastCut(upperBoundStart, upperBoundEnd, CuttingMask);
        yield return new WaitForSeconds(0.3f);
        LinecastCut(lowerBoundStart, lowerBoundEnd, CuttingMask);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ClearLine());

    }

    public void SetFillingLine() {

        if (squareArea < 0.05f || list.Count==0) { squareArea = 0; print("<0"); }

        data = 255 - Mathf.FloorToInt((255 / win) * squareArea);

        if (data >= 0)
        {
             bt = (byte)data;
            // Debug.Log(bt);
        }
        else{ bt = 0; }
        
        fillingIcon.size = new Vector2((2 / win * squareArea), 1f);
        fillingIcon.color = new Color32(bt, bt, bt, 255);
        fillingIcon2.size = new Vector2((2 / win * squareArea), 1f);
        fillingIcon2.color = new Color32(bt, bt, bt, 255);

    }

}
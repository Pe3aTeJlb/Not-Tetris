using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnitySpriteCutter;

public class IntersectionCalculator : MonoBehaviour, IComparable
{
    public float squareArea;
    public int requareOnce = 0;

    public Vector2 upperBoundStart, upperBoundEnd, lowerBoundStart, lowerBoundEnd;
    public LayerMask CalculationMask, CuttingMask;

    public List<Vector2> buffVector = new List<Vector2>();
    //public float buff;


    public List<Vector2> intersectionAreaPoints = new List<Vector2>();
    public List<GameObject> list = new List<GameObject>();
    public List<GameObject> toDelete = new List<GameObject>();
    public Dictionary<GameObject, float> intersectionSegments = new Dictionary<GameObject, float>();
    public List<GameObject> toEnable = new List<GameObject>();
    public IntersectionCalculator[] UpperLines;

    public bool ready;

    public float win;
    public Text text;
    public SpriteRenderer sp;
    public SpriteRenderer fillingIcon, fillingIcon2;

    GameObject current;

    float segmentArea, mass;

    public bool Cut;
   // int f = 0;

    Coroutine Velocity;
   // Coroutine Velocity2;

    byte bt = 0;
    int data = 0;
    Vector2 indicatorLength = new Vector2(1f,1f);

    public bool deleting = false;

    public int lineNumber;

    public void Start()
    {
        UpperLines = this.transform.parent.GetComponentsInChildren<IntersectionCalculator>();
        sp = this.GetComponent<SpriteRenderer>();
        SetFillingLine();

    }

    public void Update()
    {

        text.text = "" + squareArea;

        if (Input.GetKeyDown(KeyCode.Space)) {
            //intersectionAreaPoints.Clear();
            //buffVector.Clear();
            // StartCoroutine(wait());\

            ClearLog();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {

            Time.timeScale = 1;
            ClearLog();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {

            Time.timeScale = 0;
        }

    }

    public void FixedUpdate()
    {
        SetFillingLine();
    }

    public static void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        try
        {
            if (GlobalObserver.Deleting == false) { 
            Velocity = StartCoroutine(CheckVelocity(collision));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (
            list.Contains(collision.gameObject) == false &&
            (collision.gameObject.tag == "fragment" || collision.gameObject.tag == "floor") &&
            toDelete.Contains(collision.gameObject) == false 
            && GlobalObserver.Deleting == false
            )
        {
            Velocity = StartCoroutine(CheckVelocity(collision));
        }


        if (
             GlobalObserver.Deleting == true &&
        (collision.gameObject.tag == "fragment") &&
        toDelete.Contains(collision.gameObject) == false && 
        collision.gameObject.GetComponent<PolygonCollider2D>().bounds.center.y > lowerBoundStart.y &&
        collision.gameObject.GetComponent<PolygonCollider2D>().bounds.center.y < upperBoundStart.y
        )
        {
            //Debug.Log("\n" + " ssss " + "\n");
            toDelete.Add(collision.gameObject);
        }


        if (intersectionSegments.ContainsKey(collision.gameObject) == true && list.Contains(collision.gameObject) && (collision.gameObject.tag == "fragment" || collision.gameObject.tag == "floor") && collision.GetComponent<Rigidbody2D>().velocity.magnitude > 0.4f)
        {
            //Debug.Log("Recalculate " + collision.gameObject.name + " " + collision.GetComponent<Rigidbody2D>().velocity.magnitude);
            squareArea -= intersectionSegments[collision.gameObject];
            intersectionSegments.Remove(collision.gameObject);
            list.Remove(collision.gameObject);
            toDelete.Remove(collision.gameObject);
            Velocity = StartCoroutine(CheckVelocity(collision));
        }

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        
        StopCoroutine(Velocity);
        
        if (toDelete.Contains(collision.gameObject) == true)
        {
            toDelete.Remove(collision.gameObject);
        }

        if (list.Contains(collision.gameObject) == true)
        {
            list.Remove(collision.gameObject);
            SetFillingLine();
        }

        if (intersectionSegments.ContainsKey(collision.gameObject) == true)
        {
            squareArea -= intersectionSegments[collision.gameObject];
            intersectionSegments.Remove(collision.gameObject);
            SetFillingLine();
        } 

    }

    public IEnumerator CheckVelocity(Collider2D collision)
    {

        if (collision.gameObject == null) {
            StopCoroutine(Velocity);
        }

        yield return new WaitUntil(() => collision.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f);

            if (list.Contains(collision.gameObject) != true && (collision.gameObject.tag == "fragment" || collision.gameObject.tag == "floor"))
            {
                list.Add(collision.gameObject);
                FindBounds(collision);
            }

    }

    //находим границы и составляем массив
    public void FindBounds(Collider2D collision)
    {
        collision.gameObject.layer = 9;

        RaycastHit2D[] hits = Physics2D.LinecastAll(upperBoundStart, upperBoundEnd, CalculationMask);
        RaycastHit2D[] reversHits = Physics2D.LinecastAll(upperBoundEnd, upperBoundStart, CalculationMask);
        RaycastHit2D[] hits2 = Physics2D.LinecastAll(lowerBoundStart, lowerBoundEnd, CalculationMask);
        RaycastHit2D[] reversHits2 = Physics2D.LinecastAll(lowerBoundEnd, lowerBoundStart, CalculationMask);

        collision.gameObject.layer = 8;

        bool upstart=false, lowstart = false;
        bool forGodsakeItAddedSomething = false;
        bool inside1 = true, inside2 = true;
        
        PolygonCollider2D pl = collision.gameObject.GetComponent<PolygonCollider2D>();

        for (int i = 0; i < pl.points.Length; i++)
        {
            buffVector.Add(pl.transform.TransformPoint(pl.points[i]));
        }
        buffVector.Add(buffVector[0]);

        float dich = 0;

        for (int i = 0; i < buffVector.Count-1; i++) {

            dich += (buffVector[i].x * buffVector[i + 1].y) - (buffVector[i + 1].x * buffVector[i].y);
        }

        if (dich > 0) {} else {buffVector.Reverse(); }
     
        for (int i = 0; i < buffVector.Count; i++)
        {

                if (buffVector[i].y > upperBoundStart.y && upstart != true && reversHits.Length != 0)
                {

                    upstart = true;
                    inside1 = false;
                    intersectionAreaPoints.Add(reversHits[0].point);

                }
                else if (buffVector[i].y < upperBoundStart.y && upstart == true && hits.Length != 0)
                { 
                    upstart = false;
                    intersectionAreaPoints.Add(hits[0].point); 

                    if (buffVector[i].y > lowerBoundStart.y)
                    {
                            forGodsakeItAddedSomething = true;
                            intersectionAreaPoints.Add(buffVector[i]);
                    }
                    else if(hits2.Length!=0)
                    {
                            lowstart = true;
                            intersectionAreaPoints.Add(hits2[0].point);   
                    }

                }
                else if (buffVector[i].y < lowerBoundStart.y && lowstart != true && hits2.Length != 0)
                {
                
                    lowstart = true;
                    inside2 = false;
                    intersectionAreaPoints.Add(hits2[0].point);
                    
                }
                else if (buffVector[i].y > lowerBoundStart.y && lowstart == true && reversHits2.Length != 0)
                {
                  
                    lowstart = false;
                    intersectionAreaPoints.Add(reversHits2[0].point);
                   
                    if (buffVector[i].y < upperBoundStart.y)
                    {
                            forGodsakeItAddedSomething = true;
                            intersectionAreaPoints.Add(buffVector[i]);
                    }
                    else if(reversHits.Length != 0)
                    {
                            upstart = true;
                            intersectionAreaPoints.Add(reversHits[0].point);
                    }

                }
                else if (buffVector[i].y < upperBoundStart.y && buffVector[i].y > lowerBoundStart.y)
                {
                    forGodsakeItAddedSomething = true;
                    intersectionAreaPoints.Add(buffVector[i]);
                }

        }

        if (forGodsakeItAddedSomething == false && reversHits.Length != 0 && hits.Length != 0 && hits2.Length != 0 && reversHits2.Length != 0)
        {

                intersectionAreaPoints.Clear();
                intersectionAreaPoints.Add(reversHits[0].point);
                intersectionAreaPoints.Add(hits[0].point);
                intersectionAreaPoints.Add(hits2[0].point);
                intersectionAreaPoints.Add(reversHits2[0].point);
                intersectionAreaPoints.Add(reversHits[0].point);
            
        }

        if (toDelete.Contains(collision.gameObject) != true && (pl.bounds.center.y < upperBoundStart.y && pl.bounds.center.y > lowerBoundStart.y) && ( (inside1 == true && inside2 == true) && (reversHits.Length == 0 && hits.Length == 0 && hits2.Length == 0 && reversHits2.Length == 0)) )
        {
            toDelete.Add(collision.gameObject);
        }

        buffVector.Clear();
        CalculateIntersection(collision);

    }

    public void CalculateIntersection(Collider2D collision)
    {

        float buff_squareArea = 0;

        for (int i = 0; i < intersectionAreaPoints.Count - 1; i++)
        {
            buff_squareArea += (intersectionAreaPoints[i].x * intersectionAreaPoints[i + 1].y) - (intersectionAreaPoints[i + 1].x * intersectionAreaPoints[i].y);
        }
        
        buff_squareArea = Mathf.Abs(buff_squareArea) / 2;

        intersectionSegments.Add(collision.gameObject, buff_squareArea);
        squareArea += buff_squareArea;

        intersectionAreaPoints.Clear();

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
                //list.Remove(go);

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
                //gameObjectCreationMode = SpriteCutterInput.GameObjectCreationMode.CUT_OFF_ONE,
            });


            if (output != null && output.secondSideGameObject != null)
            {
                output.firstSideGameObject.tag = "fragment";
                output.secondSideGameObject.tag = "fragment";


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

                PolygonCollider2D rb1 = output.firstSideGameObject.GetComponent<PolygonCollider2D>();
                PolygonCollider2D rb2 = output.secondSideGameObject.GetComponent<PolygonCollider2D>();

                StartCoroutine(wait2(output.firstSideGameObject, output.secondSideGameObject));

            }
        }

        ready = true;
    }

    public IEnumerator ClearLine()
    {
        yield return new WaitUntil(() => ready == true);
        yield return new WaitForSeconds(.5f);

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
            try
            {
                if (toEnable[i].transform != null && toEnable[i].GetComponent<Rigidbody2D>() != null)
                {
                    toEnable[i].GetComponent<Rigidbody2D>().isKinematic = false;
                }
            }
            catch (Exception e) { Debug.LogWarning(e); }
        }

        list.Clear();
        toDelete.Clear();
        toEnable.Clear();
        buffVector.Clear();
        intersectionAreaPoints.Clear();
        intersectionSegments.Clear();

        Not_Tetris.Score += Mathf.RoundToInt(10 * squareArea);
        Not_Tetris.Lines += 1;
        Not_Tetris.UpdateUI();

        squareArea = 0;
        ready = false;
        requareOnce = 0;

        //for (int i = lineNumber - 1; i < 20; i++){UpperLines[i].ClearUpperLines();}
        
        sp.enabled = false;
        SetFillingLine();
        current.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        GlobalObserver.Deleting = false;

    }

    public IEnumerator wait()
    {
        yield return new WaitUntil(() => GlobalObserver.Deleting == false);
        GlobalObserver.Deleting = true;
        LinecastCut(lowerBoundStart, lowerBoundEnd, CuttingMask);
        yield return new WaitForSeconds(0.3f);
        LinecastCut(upperBoundStart, upperBoundEnd, CuttingMask);
        yield return new WaitForSeconds(0.3f);
        //Time.timeScale = 0;
        StartCoroutine(ClearLine());

    }

    public IEnumerator wait2(GameObject a, GameObject b)
    {
        yield return new WaitForSeconds(0.1f);

        PolygonCollider2D rb1 = a.GetComponent<PolygonCollider2D>();
        PolygonCollider2D rb2 = b.GetComponent<PolygonCollider2D>();

        if (a.GetComponent<MeshRenderer>() != null)
        {
            a.GetComponent<MeshRenderer>().enabled = true;
        }

        if (b.GetComponent<MeshRenderer>() != null)
        {
            b.GetComponent<MeshRenderer>().enabled = true;
        }

        if (toDelete.Contains(a) != true && rb1.bounds.center.y < upperBoundStart.y && rb1.bounds.center.y > lowerBoundStart.y && rb1.bounds.size.y > 0.1f)
        {
            
            //Debug.Log("Add in to Delete " + a.name + " in line " + this.name + " from 562 ");
            toEnable.Add(a);
            toDelete.Add(a);

        }
        else if (rb1.bounds.size.y < 0.1f) { Destroy(a); }
        else
        {
            toEnable.Add(a);
            //Debug.Log("Nothing happening with " + a.name + " in line " + this.name); 
        }

        if (toDelete.Contains(b) != true && rb2.bounds.center.y < upperBoundStart.y && rb2.bounds.center.y > lowerBoundStart.y && rb2.bounds.size.y > 0.1f)
        {
           // Debug.Log("Add in to Delete " + b.name + " in line " + this.name + " from 569");
            toEnable.Add(b);
            toDelete.Add(b);

        }
        else if (rb2.bounds.size.y < 0.1f) { Destroy(b); }
        else
        {
            toEnable.Add(b);
            //Debug.Log("Nothing happening with " + b.name + " in line " + this.name); 
        }


    }

    public void SetFillingLine() {

        if (squareArea < 0.1f)
        {
            squareArea = 0;
        }

        data = 255 - Mathf.FloorToInt((255 / win) * squareArea);

        if (data >= 0)
        {
             bt = (byte)data;
        }
        else{ bt = 0; }

        if ((2 / win * squareArea) < 2)
        {
            indicatorLength.x = (2 / win * squareArea);
        }
        else { indicatorLength.x = 2; }

        fillingIcon.size = indicatorLength;
        fillingIcon.color = new Color32(bt, bt, bt, 255);
        fillingIcon2.size = indicatorLength;
        fillingIcon2.color = new Color32(bt, bt, bt, 255);  

    }

    bool HitCounts(RaycastHit2D hit)
    {
        return (hit.transform.GetComponent<SpriteRenderer>() != null ||
                 hit.transform.GetComponent<MeshRenderer>() != null);
    }

    public int CompareTo(object obj)
    {
        throw new NotImplementedException();
    }

    public void ClearUpperLines()
    {

        list.Clear();
        toDelete.Clear();
        toEnable.Clear();
        buffVector.Clear();
        intersectionAreaPoints.Clear();
        intersectionSegments.Clear();

        squareArea = 0;
        ready = false;
        requareOnce = 0;

        SetFillingLine();

    }

}
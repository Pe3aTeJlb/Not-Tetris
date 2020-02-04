using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnitySpriteCutter;
using UnityEngine.UI;

public class IntersectionCalculator : MonoBehaviour, IComparable
{
    public float squareArea;
    int requareOnce = 0;

    public Vector2 upperBoundStart, upperBoundEnd, lowerBoundStart, lowerBoundEnd;
    public LayerMask CalculationMask, CuttingMask;

    public List<Vector2> buffVector = new List<Vector2>();

    public List<Vector2> intersectionAreaPoints = new List<Vector2>();
    public List<GameObject> list = new List<GameObject>();
    public List<GameObject> toDelete = new List<GameObject>();
    public Dictionary<GameObject, float> intersectionSegments = new Dictionary<GameObject, float>();
    public List<GameObject> toEnable = new List<GameObject>();

    public bool ready;

    public float win;
    public SpriteRenderer sp;
    public SpriteRenderer fillingIcon, fillingIcon2;

    GameObject current;

    float segmentArea, mass;

    Coroutine Velocity;

    byte bt = 0;
    int data = 0;
    Vector2 indicatorLength = new Vector2(1f,1f);

    public bool deleting = false;

    public static bool IsGameOver = false;

    Stopwatch Watch = new Stopwatch();
    Vector2 technic = new Vector2(-100, -100);

    public bool ignore;
    public IntersectionCalculator lowerline;

    public Toggle a;
    public InputField b;
    public bool alt;
    public float per;

    public int frame;

    public bool waiting;

    public void Start()
    {
        sp = this.GetComponent<SpriteRenderer>();
        SetFillingLine();
        IsGameOver = false;

        Vector2 v = IntersectionPoint(new Vector2(0,0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1));
        frame = 0;
        waiting = false;
        per = 1.5f;
        alt = true;
    }

    public void Update()
    {
       // alt = a.isOn;
      //  per = float.Parse(b.text);

        if (frame < 31) {
            frame += 1;
        }
        if (frame > 30)
        {
            frame = 0;
        }

        if (frame == 0 || frame == 15 || frame == 30)
        {
            SetFillingLine();
        }   
        

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        try
        {
            if (GlobalObserver.Deleting == false && IsGameOver == false) { 
            Velocity = StartCoroutine(CheckVelocity(collision));
            }
        }
        catch (Exception e) { UnityEngine.Debug.Log(e);}

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (frame == 0 || frame == 15 || frame == 30)
        {

            if (IsGameOver == false)
            {

                if (
                        list.Contains(collision.gameObject) == false &&
                        (collision.gameObject.tag == "fragment" || collision.gameObject.tag == "floor") &&
                        toDelete.Contains(collision.gameObject) == false
                        //&& GlobalObserver.Deleting == false
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
                    toDelete.Add(collision.gameObject);
                }

                if (
                        intersectionSegments.ContainsKey(collision.gameObject) == true &&
                        list.Contains(collision.gameObject) &&
                        (collision.gameObject.tag == "fragment" || collision.gameObject.tag == "floor") &&
                        (
                        collision.GetComponent<Rigidbody2D>().velocity.magnitude > per
                        //Mathf.Abs(collision.GetComponent<Rigidbody2D>().angularVelocity) > 120
                        )
                    )
                {
                    squareArea -= intersectionSegments[collision.gameObject];
                    intersectionSegments.Remove(collision.gameObject);
                    list.Remove(collision.gameObject);
                    toDelete.Remove(collision.gameObject);
                    Velocity = StartCoroutine(CheckVelocity(collision));
                }

            }
        }
    }       

    public void OnTriggerExit2D(Collider2D collision)
    {

            if (Velocity != null)
            {
                StopCoroutine(Velocity);
            }

            if (toDelete.Contains(collision.gameObject) == true)
            {
                toDelete.Remove(collision.gameObject);
            }

            if (list.Contains(collision.gameObject) == true)
            {
                list.Remove(collision.gameObject);
                SetFillingLine();
            }

            if (intersectionSegments.ContainsKey(collision.gameObject) == true && deleting == false)
            {
                squareArea -= intersectionSegments[collision.gameObject];
                intersectionSegments.Remove(collision.gameObject);
                SetFillingLine();
            }

    }

    public IEnumerator CheckVelocity(Collider2D collision)
    {

        if (collision.GetComponent<Rigidbody2D>() == null) {
            StopCoroutine(Velocity);
        }

        yield return new WaitUntil(() => collision.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f);

            if (list.Contains(collision.gameObject) != true && (collision.gameObject.tag == "fragment" || collision.gameObject.tag == "floor"))
            {
                list.Add(collision.gameObject);
                FindBounds(collision);
            }

    }

    public void FindBounds(Collider2D collision)
    {

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

        #region
        if (alt == true)
        {
            //Watch.Start();

            if (buffVector[0].y < lowerBoundStart.y)
            {

                lowstart = true;
                inside2 = false;

                Vector2 a = IntersectionPoint(lowerBoundStart, lowerBoundEnd, buffVector[0], buffVector[1]);
                if (a != technic) { intersectionAreaPoints.Add(a); }
            }

            if (buffVector[0].y < upperBoundStart.y && buffVector[0].y > lowerBoundStart.y)
            {
                intersectionAreaPoints.Add(buffVector[0]);
            }

            if (buffVector[0].y > upperBoundStart.y)
            {
                upstart = true;
            }

            for (int i = 1; i < buffVector.Count; i++)
            {

                if (buffVector[i].y > upperBoundStart.y && upstart != true)
                {
                    upstart = true;
                    inside1 = false;

                    if (buffVector[i - 1].y < lowerBoundStart.y)
                    {

                        Vector2 a = IntersectionPoint(lowerBoundStart, lowerBoundEnd, buffVector[i], buffVector[i - 1]);
                        if (a != technic) { intersectionAreaPoints.Add(a); }

                        Vector2 b = IntersectionPoint(upperBoundStart, upperBoundEnd, buffVector[i], buffVector[i - 1]);
                        if (b != technic) { intersectionAreaPoints.Add(b); }

                    }

                    else if (buffVector[i - 1].y < upperBoundStart.y && buffVector[i - 1].y > lowerBoundStart.y)
                    {

                        Vector2 a = IntersectionPoint(upperBoundStart, upperBoundEnd, buffVector[i], buffVector[i - 1]);
                        if (a != technic) { intersectionAreaPoints.Add(a); }

                    }
                    else if (buffVector[i - 1].y > upperBoundStart.y)
                    {

                    }

                }
                else if (buffVector[i].y < upperBoundStart.y && upstart == true)
                {
                    upstart = false;

                    Vector2 a = IntersectionPoint(upperBoundStart, upperBoundEnd, buffVector[i], buffVector[i - 1]);
                    if (a != technic) { intersectionAreaPoints.Add(a); }

                    if (buffVector[i].y > lowerBoundStart.y)
                    {
                        intersectionAreaPoints.Add(buffVector[i]);
                    }
                    else
                    {
                        lowstart = true;

                        Vector2 b = IntersectionPoint(lowerBoundStart, lowerBoundEnd, buffVector[i], buffVector[i - 1]);
                        if (b != technic) { intersectionAreaPoints.Add(b); }

                    }

                }
                else if (buffVector[i].y < lowerBoundStart.y && lowstart != true)
                {

                    lowstart = true;
                    inside2 = false;

                    Vector2 a = IntersectionPoint(lowerBoundStart, lowerBoundEnd, buffVector[i], buffVector[i - 1]);
                    if (a != technic) { intersectionAreaPoints.Add(a); }


                }
                else if (buffVector[i].y > lowerBoundStart.y && lowstart == true)
                {

                    lowstart = false;

                    Vector2 a = IntersectionPoint(lowerBoundStart, lowerBoundEnd, buffVector[i], buffVector[i - 1]);
                    if (a != technic) { intersectionAreaPoints.Add(a); }

                    if (buffVector[i].y < upperBoundStart.y)
                    {
                        intersectionAreaPoints.Add(buffVector[i]);

                    }
                    else if (buffVector[i].y > upperBoundStart.y)
                    {

                        Vector2 b = IntersectionPoint(lowerBoundStart, lowerBoundEnd, buffVector[i], buffVector[i - 1]);
                        if (b != technic) { intersectionAreaPoints.Add(b); }

                    }
                    else
                    {
                        upstart = true;

                        Vector2 c = IntersectionPoint(upperBoundStart, upperBoundEnd, buffVector[i], buffVector[i - 1]);
                        if (c != technic) { intersectionAreaPoints.Add(c); }

                    }

                }
                else if (buffVector[i].y < upperBoundStart.y && buffVector[i].y > lowerBoundStart.y)
                {
                    intersectionAreaPoints.Add(buffVector[i]);
                }

            }

            if ( intersectionAreaPoints.Count != 0 && intersectionAreaPoints[0] != intersectionAreaPoints[intersectionAreaPoints.Count-1])
            {
                intersectionAreaPoints.Add(intersectionAreaPoints[0]);
            }

            if (toDelete.Contains(collision.gameObject) != true && (pl.bounds.center.y < upperBoundStart.y && pl.bounds.center.y > lowerBoundStart.y) && ((inside1 == true && inside2 == true)))
            { 
                toDelete.Add(collision.gameObject);
            }

           // TimeSpan ts2 = Watch.Elapsed;
           // Watch.Stop();
           // UnityEngine.Debug.Log("Time for manual" + ts2);
           // Watch.Reset();
        }
        #endregion

        #region
        if (alt == false)
        {
            collision.gameObject.layer = 9;

            UnityEngine.Debug.Log("time for" + name);
            Watch.Start();

            RaycastHit2D[] hits = Physics2D.LinecastAll(upperBoundStart, upperBoundEnd, CalculationMask);
            RaycastHit2D[] reversHits = Physics2D.LinecastAll(upperBoundEnd, upperBoundStart, CalculationMask);
            RaycastHit2D[] hits2 = Physics2D.LinecastAll(lowerBoundStart, lowerBoundEnd, CalculationMask);
            RaycastHit2D[] reversHits2 = Physics2D.LinecastAll(lowerBoundEnd, lowerBoundStart, CalculationMask);
            TimeSpan ts = Watch.Elapsed;

            Watch.Stop();
            UnityEngine.Debug.Log("Time for builtin points" + ts);
            Watch.Reset();

            collision.gameObject.layer = 8;


            Watch.Start();

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
                    else if (hits2.Length != 0)
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
                    else if (reversHits.Length != 0)
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

            TimeSpan ts3 = Watch.Elapsed;
            Watch.Stop();
            UnityEngine.Debug.Log("Time for build in calc" + ts3);
            Watch.Reset();


            if (forGodsakeItAddedSomething == false && reversHits.Length != 0 && hits.Length != 0 && hits2.Length != 0 && reversHits2.Length != 0)
            {
                Watch.Start();
                intersectionAreaPoints.Clear();
                intersectionAreaPoints.Add(reversHits[0].point);
                intersectionAreaPoints.Add(hits[0].point);
                intersectionAreaPoints.Add(hits2[0].point);
                intersectionAreaPoints.Add(reversHits2[0].point);
                intersectionAreaPoints.Add(reversHits[0].point);

                TimeSpan ts4 = Watch.Elapsed;
                Watch.Stop();
                UnityEngine.Debug.Log("Time for buildin worst" + ts4);
                Watch.Reset();

            }



            if (toDelete.Contains(collision.gameObject) != true && (pl.bounds.center.y < upperBoundStart.y && pl.bounds.center.y > lowerBoundStart.y) && ((inside1 == true && inside2 == true) && (reversHits.Length == 0 && hits.Length == 0 && hits2.Length == 0 && reversHits2.Length == 0)))
            {
                toDelete.Add(collision.gameObject);
            }
        }  
        #endregion

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

        if (intersectionSegments.ContainsKey(collision.gameObject) == false)
        {
            intersectionSegments.Add(collision.gameObject, buff_squareArea);
            squareArea += buff_squareArea;
        }

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

                StartCoroutine(CutQueue());
            }

        }

    }

    public IEnumerator CutQueue()
    {

        yield return new WaitForEndOfFrame();
        deleting = true;

        if (ignore == false)
        {

            if (lowerline.deleting == true || lowerline.waiting == true)
            {
                waiting = true;
            }

            for (int i = 0; i < toDelete.Count; i++)
            {
                try
                {
                    toDelete[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    toEnable.Add(toDelete[i]);
                }
                catch (Exception e) { UnityEngine.Debug.Log(e); }
            }

            yield return new WaitUntil(() => lowerline.deleting == false);
            yield return new WaitUntil(() => lowerline.waiting == false);
        }

        yield return new WaitUntil(() => GlobalObserver.Deleting == false);

        GlobalObserver.Deleting = true;

        LinecastCut(lowerBoundStart, lowerBoundEnd, CuttingMask);

        //yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => ready == true);
        LinecastCut(upperBoundStart, upperBoundEnd, CuttingMask);

        //yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => ready == true);
        yield return new WaitForSeconds(.5f);

        ClearLine();

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
            });


            if (output != null && output.secondSideGameObject != null)
            {
                output.firstSideGameObject.tag = "fragment";
                output.secondSideGameObject.tag = "fragment";

                Rigidbody2D newRigidbody = output.secondSideGameObject.AddComponent<Rigidbody2D>();
                newRigidbody.gravityScale = 1;

                output.firstSideGameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                output.secondSideGameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

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

                StartCoroutine(SortCutObjects(output.firstSideGameObject, output.secondSideGameObject));
                
            }
        }

        ready = true;
    }

    public IEnumerator SortCutObjects(GameObject a, GameObject b)
    {
        yield return new WaitForSeconds(0.1f);


        if (a != null)
        {

            if (a.GetComponent<MeshRenderer>() != null)
            {
                a.GetComponent<MeshRenderer>().enabled = true;
            }

            PolygonCollider2D rb1 = a.GetComponent<PolygonCollider2D>();


            if (toDelete.Contains(a) != true && rb1.bounds.center.y < upperBoundStart.y && rb1.bounds.center.y > lowerBoundStart.y && rb1.bounds.size.y > 0.1f)
            {

                toEnable.Add(a);
                toDelete.Add(a);

            }
            else if (rb1.bounds.size.y < 0.1f) { Destroy(a); }

            else if (toDelete.Contains(a) != true)
            {
                toEnable.Add(a);
            }
        }

        if (b != null)
        {

            if (b.GetComponent<MeshRenderer>() != null)
            {
                b.GetComponent<MeshRenderer>().enabled = true;
            }

            PolygonCollider2D rb2 = b.GetComponent<PolygonCollider2D>();


            if (toDelete.Contains(b) != true && rb2.bounds.center.y < upperBoundStart.y && rb2.bounds.center.y > lowerBoundStart.y && rb2.bounds.size.y > 0.1f)
            {

                toEnable.Add(b);
                toDelete.Add(b);

            }
            else if (rb2.bounds.size.y < 0.1f) { Destroy(b); }
            else if (toDelete.Contains(b) != true)
            {

                toEnable.Add(b);
            }
        }

    }

    public void ClearLine()
    {

        for (int i = 0; i < toDelete.Count; i++)
        {
            try
            {
                toDelete[i].layer = 10;
            }
            catch (Exception e) { UnityEngine.Debug.Log(e);}
        }

        for (int i = 0; i < toEnable.Count; i++)
        {
            try
            {
                if (toEnable[i].transform != null && toEnable[i].GetComponent<Rigidbody2D>() != null)
                {
                    toEnable[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                }
            }
            catch (Exception e) { UnityEngine.Debug.LogWarning(e);}
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
        
        sp.enabled = false;
        SetFillingLine();
        current.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        waiting = false;
        deleting = false;
        GlobalObserver.Deleting = false;

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

    public Vector2 IntersectionPoint(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        float xo = A.x, 
              yo = A.y;

        float p = B.x - A.x, 
              q = B.y - A.y;

        float x1 = C.x,
              y1 = C.y;

        float p1 = D.x - C.x,
              q1 = D.y - C.y;

        if (q * p1 - q1 * p != 0 && 
            p * q1 - p1 * q != 0)
        {
            float x = (xo * q * p1 - x1 * q1 * p - yo * p * p1 + y1 * p * p1) /
                (q * p1 - q1 * p);
            float y = (yo * p * q1 - y1 * p1 * q - xo * q * q1 + x1 * q * q1) /
                (p * q1 - p1 * q);

            if (Mathf.Abs(x) < 11)
            {
                return new Vector2(x, y);
            }
            else { return new Vector2(-100,-100); }

        }
        else {

            return new Vector2(-100, -100); ; }
        
    }

}
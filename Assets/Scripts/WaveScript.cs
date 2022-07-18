using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveScript : MonoBehaviour
{
    /*
    public Vector3 initalPosition;
    public int pointCount = 100;
    public LineRenderer line;

    private Vector3 secondPosition;
    private Vector3[] points;
    private float segmentWidth;

    private float amplitude = 1;
    private float frequency = 1;
    private float movementSpeed = 1;
    private float Tau = 2* Mathf.PI;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.positionCount = pointCount;

        // tell the linerenderer to use the local 
        // transform space for the point coorindates
        line.useWorldSpace = false;

        points = new Vector3[pointCount];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Camera.main.ScreenToWorldPoint needs a value in Z
            // for the distance to camera
            secondPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - Vector3.forward * Camera.main.transform.position.z);
            Debug.Log("Pos: " + secondPosition);
            secondPosition.z = 0;

            var dir = secondPosition - initalPosition;
            // get the segmentWidth from distance to end position
            segmentWidth = Vector3.Distance(initalPosition, secondPosition) / pointCount;

            // get the difference angle in the Z axis between the current transform.right
            // and the direction
            var angleDifference = Vector3.SignedAngle(transform.right, dir, Vector3.forward);
            // and rotate the linerenderer transform by that angle in the Z axis
            // so the result will be that it's transform.right
            // points now towards the clicked position
            transform.Rotate(Vector3.forward * angleDifference, Space.World);
        }

        for (var i = 0; i < points.Length; ++i)
        {
            float progress = (float)i/(pointCount-1);
            float x = Mathf.Lerp(0,secondPosition.x,progress);
            float y = amplitude*Mathf.Sin((Tau*frequency*x)+(Time.timeSinceLevelLoad*movementSpeed)); //Mathf.Sin(x * Time.time);
            points[i] = new Vector3(x, y, 0);
        }
        line.SetPositions(points);
    }
    */
    public LineRenderer myLineRenderer;
    private int points = 100;
    private float amplitude = 10;
    private float frequency = 1;
    private float movementSpeed = 1;
    void Start()
    {
        myLineRenderer = GetComponent<LineRenderer>();
        //myLineRenderer.useWorldSpace = false;
    }
    
    void Draw()
    {
        float xStart = transform.parent.GetComponent<RectTransform>().offsetMin.x + 10;  //xLimits.x;
        float Tau = 2* Mathf.PI;
        float xFinish = transform.parent.GetComponent<RectTransform>().offsetMax.x - 10; //xLimits.y;

        myLineRenderer.positionCount = 2;
        Vector3[] v = new Vector3[4];
        transform.parent.GetComponent<RectTransform>().GetWorldCorners(v);

        myLineRenderer.SetPosition(0, v[0]);
        //myLineRenderer.SetPosition(1, v[1]);
        //myLineRenderer.SetPosition(2, v[2]);
        //myLineRenderer.SetPosition(3, v[3]);
        //Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(50,0, Camera.main.transform.position.x + 3000));
        //position.z = 0;
        //position.y = Screen.height - position.y;
        Vector3 tPos = new Vector3(0,0,0);
        float tX = (tPos.x * Screen.width);
        float tY = (tPos.y * Screen.height);

        Vector3 screenPos = new Vector3(tX ,tY, 0);
        Vector3 StartPos = Camera.main.ScreenToWorldPoint(screenPos);

        myLineRenderer.SetPosition(1, StartPos);
        /*
        for(int currentPoint = 0; currentPoint<points;currentPoint++)
        {
            float progress = (float)currentPoint/(points-1);
            float x = Mathf.Lerp(xStart,xFinish,progress);
            float y = amplitude*Mathf.Sin((Tau*frequency*x)+(Time.timeSinceLevelLoad*movementSpeed));
            myLineRenderer.SetPosition(currentPoint, new Vector3(x,y,0));
        }
        */
    }
 
    void Update()
    {
        Draw();
    }
}

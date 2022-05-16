using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buildings = new List<GameObject>();
    [SerializeField]
    private List<GameObject> receivers = new List<GameObject>();
    [SerializeField]
    private Material rayMaterial;

    void Start()
    {
        int pathNumber = 0;

        //Reflected Rays
        foreach (GameObject building in buildings) {
            Vector3 offset = new Vector3(0, 10, 0);
            if (building.tag == "B2") {
                offset = new Vector3(0, 22, 0);
            }

            //Direct Ray from Satellite to Object 
            GameObject directRay = new GameObject();
            directRay.name = "Path " + pathNumber;
            LineRenderer dlr = directRay.AddComponent<LineRenderer>();
            dlr.positionCount = 2;
            dlr.SetPositions(new Vector3[] {transform.position, building.transform.position + offset});
            dlr.material = rayMaterial;
            dlr.material.color = Color.black;

            //Reflected Ray from same path to a receiver
            GameObject reflectedRay = new GameObject();
            reflectedRay.name = "Path " + pathNumber;
            LineRenderer rlr = reflectedRay.AddComponent<LineRenderer>();
            rlr.positionCount = 2;
            rlr.SetPositions(new Vector3[] {building.transform.position + offset, receivers[pathNumber].transform.position + new Vector3(0, 5, 0)});
            rlr.material = rayMaterial;
            rlr.material.color = new Color(255, 0, 0);

            pathNumber++;
        }

        //Direct Ray from Satellite to Object 
        GameObject directRay2 = new GameObject();
        directRay2.name = "Path " + pathNumber;
        LineRenderer dlr2 = directRay2.AddComponent<LineRenderer>();
        dlr2.positionCount = 2;
        dlr2.SetPositions(new Vector3[] {transform.position, receivers[0].transform.position + new Vector3(0, 5, 0)});
        dlr2.material = rayMaterial;
        dlr2.material.color = Color.black;
    }

    void Update()
    {
        
    }
}

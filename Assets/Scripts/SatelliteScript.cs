using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteScript : MonoBehaviour
{
    [SerializeField]
    private Material rayMaterial;
    private int spawnRays = 1000;
    private float radiuesIncrease = 0.1f;
    //private List<GameObject> buildings = new List<GameObject>();
    //private List<GameObject> receivers = new List<GameObject>();
    private List<GameObject> paths = new List<GameObject>();
    private List<GameObject> filteredPaths = new List<GameObject>();
    private bool rcvIsFocused = false;
    private GameObject focusedReceiver;
    private int count = 0;
    private int maxEveAmount = 10;
    private float coneAngle = 45f;
    private int scatterAmount = 20;
    private Vector3 offset = new Vector3(0, 5, 0);

    public void togglePaths(List<GameObject> filterPaths, GameObject focusedRcv) {
        if (focusedReceiver != focusedRcv | rcvIsFocused) {
            foreach (GameObject path in filteredPaths) {
                path.SetActive(true);
            }
            filteredPaths.Clear();
        }
        if (!rcvIsFocused | (rcvIsFocused && focusedReceiver != focusedRcv)) {
            foreach (GameObject path in paths) {
                if (!filterPaths.Contains(path)) {
                    filteredPaths.Add(path);
                    path.SetActive(false);
                }
            }
        }
        GameObject previousFocused = focusedReceiver;
        focusedReceiver = focusedRcv;
        if (previousFocused == focusedRcv | previousFocused == null | !rcvIsFocused) {
            rcvIsFocused = !rcvIsFocused;
        }
    }

    private Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
    {
        float angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        Vector2 PointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        Vector3 v = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * v;
    }

    private Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
    {
        return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
    }

    public void deletePaths() {
        foreach (GameObject path in paths) {
            Destroy(path);
        }
    }

    public void fireRays(List<GameObject> buildingList, List<GameObject> eavesdropperList, GameObject legitReceiver) {
        int steps = 1;
        float spawnRaysAmount = spawnRays * radiuesIncrease;
        float radius = 90 * radiuesIncrease;
        while (radius <= 90) {
            for (int i = 0; i < spawnRaysAmount; i++) {
                Vector2 rndPos = (Random.insideUnitCircle * radius) + new Vector2(legitReceiver.transform.position.x, legitReceiver.transform.position.z);
                Vector3 dir = -(transform.position - new Vector3(rndPos.x, 0, rndPos.y));

                Ray ray = new Ray(transform.position, dir);
                RaycastHit hit;

                List<Vector3> points = new List<Vector3>();
                if (Physics.Raycast(ray.origin, ray.direction, out hit)) {
                    if (hit.collider.tag != "Platform") {
                        points.Add(transform.position);
                        points.Add(hit.point);

                        //Reflected ray
                        if (hit.collider.tag == "B1") {
                            List<GameObject> candidates = new List<GameObject>();
                            foreach (GameObject eve in eavesdropperList) {
                                Vector3 eveDir = -(hit.point - eve.transform.position);
                                Ray eveRay = new Ray(hit.point, dir);
                                RaycastHit evehit;
                                //If eve in line of sight
                                if (Physics.Raycast(eveRay.origin, eveRay.direction, out evehit)) {
                                    if (evehit.collider.tag != "B1" && evehit.collider.tag != "B2" && evehit.collider.tag != "B2") {
                                        candidates.Add(eve);
                                        //Debug.Log("EVE visible" + eve.name);
                                    }
                                    else {
                                        //Debug.Log("EVE COLLISION");
                                    }
                                }
                            }
                            int n = Random.Range(0, maxEveAmount);
                            //Choose evacesdropper
                            if (n < candidates.Count) {
                                int j = Random.Range(0, n);
                                points.Add(candidates[j].transform.position + offset);
                                //Debug.Log("REFLECTED ON: " + candidates[j].name);
                                generateRay(points, "B1", new List<GameObject>() {candidates[j]});
                            }
                            //Choose legit receiver
                            else {
                                Vector3 rcvDir = -(hit.point - legitReceiver.transform.position);
                                Ray rcvRay = new Ray(hit.point, dir);
                                RaycastHit rcvhit;
                                //If legit receiver in line of sight
                                if (n < 5) {
                                    if (Physics.Raycast(rcvRay.origin, rcvRay.direction, out rcvhit)) {
                                        if (rcvhit.collider.tag != "B1" && rcvhit.collider.tag != "B2" && rcvhit.collider.tag != "B3") {
                                            points.Add(legitReceiver.transform.position + offset);
                                            //Debug.Log("REFLECTED ON: Legit");
                                            generateRay(points, "B1", new List<GameObject>() {legitReceiver});
                                        }
                                        else {
                                            //Debug.Log("LEGIT COLLISION");
                                        }
                                    }
                                }
                            }
                        }
                        //Scattered and Diffrected Rays
                        else if (hit.collider.tag == "B2" || hit.collider.tag == "B3") {
                            string rayCase = "B2";
                            if (hit.collider.tag == "B3") {
                                rayCase = "B3";
                            }
                            Vector3 rcvDir = -(hit.point - legitReceiver.transform.position);
                            HashSet<GameObject> scatterHitObjs = new HashSet<GameObject>();
                            for (int j = 0; j < scatterAmount; j++) {
                                Vector3 v = GetPointOnUnitSphereCap(rcvDir, coneAngle).normalized;

                                //Debug.DrawRay(hit.point, v, Color.magenta, 1000);

                                Ray scatterRay = new Ray(hit.point, v);
                                RaycastHit scatterhit;

                                List<Vector3> scatterPointList = new List<Vector3>();
                                scatterPointList.Add(hit.point);
                                
                                bool isHit = false;
                                if (Physics.Raycast(scatterRay.origin, scatterRay.direction, out scatterhit)) {
                                    if (scatterhit.collider.tag == "Receiver") {
                                        isHit = true;
                                        scatterHitObjs.Add(scatterhit.transform.gameObject);
                                        scatterPointList.Add(scatterhit.point);
                                        //Debug.Log("SCATTER HIT ON: " + scatterhit.transform.name);
                                    }
                                    else {
                                        scatterPointList.Add(hit.point + v * 10);
                                    }
                                }
                                else {
                                    scatterPointList.Add(hit.point + v * 10);
                                }
                                //scatterPointList.Add(hit.point + v * 10);
                                if (isHit) {
                                    generateRay(scatterPointList, rayCase, new List<GameObject>() {scatterhit.transform.gameObject}, j);
                                }
                                else {
                                    generateRay(scatterPointList, rayCase, new List<GameObject>(), j);
                                }
                            }
                            generateRay(points, rayCase, new List<GameObject>(scatterHitObjs));
                        }
                        else {
                            generateRay(points, "Direct", new List<GameObject>() {hit.transform.gameObject});
                        }
                    }
                }
            }
            steps++;
            radius = 90 * radiuesIncrease * steps; 
        }
        Debug.Log("Number of rays that hit an object: " + count);
    }

    private void generateRay(List<Vector3> points, string rayCase, List<GameObject> receivers, int subPath = -1) {
        GameObject rayObj = new GameObject();
        if (subPath != -1) {
            rayObj.name = "Path " + count + "," + subPath;
        }
        else {
            rayObj.name = "Path " + count;
            count++;
        }
        foreach (GameObject rcv in receivers){
            rcv.GetComponent<ReceiverScript>().addReceivedRay(rayObj);
        }
        paths.Add(rayObj);

        LineRenderer rayLine = rayObj.AddComponent<LineRenderer>();
        rayLine.startWidth = 0.5f;
        rayLine.endWidth = 0.5f;
        rayLine.material = rayMaterial;
        //rayLine.startColor = Color.black;
        //rayLine.endColor = Color.black;
        rayLine.material.SetColor("_Color", new Color(0f, 0f, 0f, 0.5f));
        if (rayCase == "B1") {
            rayObj.tag = "Reflected";
            //rayLine.startColor = Color.red;
            //rayLine.endColor = Color.red;
            rayLine.material.SetColor("_Color", new Color(1f, 0f, 0f, 0.5f));
        }
        else if (rayCase == "B2") {
            rayObj.tag = "Scattered";
            //rayLine.startColor = Color.green;
            //rayLine.endColor = Color.green;
            rayLine.material.SetColor("_Color", new Color(0f, 1f, 0f, 0.5f));
        }
        else if (rayCase == "B3") {
            rayObj.tag = "Diffracted";
            //rayLine.startColor = Color.magenta;
            //rayLine.endColor = Color.magenta;
            rayLine.material.SetColor("_Color", new Color(1f, 0f, 1f, 0.5f));
        }

        rayLine.positionCount = 0;
        foreach (Vector3 point in points) {
            rayLine.positionCount += 1;
            rayLine.SetPosition(rayLine.positionCount - 1, point);
        }
    }
}

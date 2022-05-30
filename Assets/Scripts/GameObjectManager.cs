using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameObjectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject platform, uic, satellitePrefab, legitReceiverPrefab, eavesdropperPrefab;
    [SerializeField]
    private List<GameObject> buildingPrefabs;
    private GameObject legitRecv;
    private GameObject satellite;
    private List<GameObject> eavesdropperList;
    private List<GameObject> buildingsList;
    public int spawnNumberBuildings = 10;
    public int spawnNumberEavesdropper = 3;
    private float platformRadius;
    public int radiusAroundLegitRecv = 30;
    public int eveLegitRecvDistance = 15;
    public int selfEveDistance = 15;
    public int buildingLegitRecvDistance = 20;
    public int selfBuildingDistance = 25;
    private int reCalculationTTries = 1000000;
    private bool fireRays = false;
    private bool startSimulation = false;

    public void setStartSimulation() {
        startSimulation = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        platformRadius = platform.transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (startSimulation) {
            startSimulation = false;
            if (legitRecv != null) {
                satellite.GetComponent<SatelliteScript>().deletePaths();
                Destroy(legitRecv);
                Destroy(satellite);
                foreach (GameObject eavesDropper in eavesdropperList) {
                    Destroy(eavesDropper);
                }
                foreach (GameObject building in buildingsList) {
                    Destroy(building);
                }
            }
            //Spawn an instance of a legitimate receiver
            legitRecv = Instantiate(legitReceiverPrefab);
            legitRecv.transform.SetParent(platform.transform, false);
            Vector2 legitRecvNewPos = Random.insideUnitCircle * (platformRadius - 10);
            legitRecv.transform.position = new Vector3(legitRecvNewPos.x, 0, legitRecvNewPos.y);
            legitRecv.transform.eulerAngles = new Vector3(0, Random.rotation.eulerAngles.y, 0);

            //Spawn instances of eavesdropper
            List<Vector2> positionList = new List<Vector2>();
            (eavesdropperList, positionList) = spawnEavesdropper(legitRecvNewPos);

            //Spawn instances of buildings
            buildingsList = spawnBuildings(legitRecvNewPos, positionList);

            fireRays = true;
        }
    }

    void LateUpdate() {
        //Fire rays
        if (fireRays) {
            Physics.autoSimulation = false;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.autoSimulation = true;

            fireRays = false;
            satellite = Instantiate(satellitePrefab);
            satellite.transform.position = new Vector3(-legitRecv.transform.position.x, satellite.transform.position.y, -legitRecv.transform.position.z);
            satellite.transform.LookAt(legitRecv.transform);
            satellite.GetComponent<SatelliteScript>().fireRays(buildingsList, eavesdropperList, legitRecv);

            legitRecv.GetComponent<ReceiverScript>().addSatellite(satellite);
            legitRecv.GetComponent<ReceiverScript>().addUiController(uic);
            foreach (GameObject eve in eavesdropperList) {
                eve.GetComponent<ReceiverScript>().addSatellite(satellite);
                eve.GetComponent<ReceiverScript>().addUiController(uic);
            }
        }
    }

    public List<GameObject> getCorrelationMatrix() {
        List<List<GameObject>> intersectionList = new List<List<GameObject>>();
        List<GameObject> legitRcvRayList = legitRecv.GetComponent<ReceiverScript>().getReceivedRays();
        foreach (GameObject eve in eavesdropperList) {
            intersectionList.Add(legitRcvRayList.Intersect(eve.GetComponent<ReceiverScript>().getReceivedRays()).ToList());
        }
        List<GameObject> correlationMatrix = new List<GameObject>();
        foreach (List<GameObject> intersectList in intersectionList) {
            correlationMatrix = correlationMatrix.Union(intersectList).ToList();
        }
        return correlationMatrix;
    }

    public List<int> getStartingSettingValues() {
        return new List<int>() {spawnNumberBuildings, spawnNumberEavesdropper, radiusAroundLegitRecv, eveLegitRecvDistance, 
                                selfEveDistance, buildingLegitRecvDistance, selfBuildingDistance};

    }

    private (List<GameObject>, List<Vector2>) spawnEavesdropper(Vector2 legitRecvCenter) {
        List<GameObject> eavesdropperList = new List<GameObject>();
        List<Vector2> positionList = new List<Vector2>();
        for (int i = 0; i < spawnNumberEavesdropper; i++) {
            GameObject newEavesdropper = Instantiate(eavesdropperPrefab);
            newEavesdropper.name = "Eavesdropper " + i;
            newEavesdropper.transform.SetParent(platform.transform, false);
            Vector2 eaveNewPos = (Random.insideUnitCircle * 10) + legitRecvCenter;
            //New eavesdropper postion has to be inside the platform and is not allowed to be close to the legitimate receiver
            /*
            while (Mathf.Pow(eaveNewPos.x, 2) + Mathf.Pow(eaveNewPos.y, 2) >= Mathf.Pow(platformRadius, 2) |
            Mathf.Pow(eaveNewPos.x - legitRecvCenter.x, 2) + Mathf.Pow(eaveNewPos.y - legitRecvCenter.y, 2) < Mathf.Pow(eveLegitRecvDistance, 2)) {
                eaveNewPos = (Random.insideUnitCircle * radiusAroundLegitRecv) + legitRecvCenter;
            }
            */

            bool calcNewPos = true;
            int tries = 0;
            while (calcNewPos && tries < reCalculationTTries) {
                bool conflict = false;
                tries++;
                //Distance to legit receiver
                if (Mathf.Pow(eaveNewPos.x, 2) + Mathf.Pow(eaveNewPos.y, 2) >= Mathf.Pow((platformRadius - 10), 2) |
                    Mathf.Pow(eaveNewPos.x - legitRecvCenter.x, 2) + Mathf.Pow(eaveNewPos.y - legitRecvCenter.y, 2) < Mathf.Pow(eveLegitRecvDistance, 2)) {
                    conflict = true;
                }
                //Distance to other buildings
                if (!conflict) {
                    foreach (Vector2 eveCenter in positionList) {
                        if (Mathf.Pow(eaveNewPos.x - eveCenter.x, 2) + Mathf.Pow(eaveNewPos.y - eveCenter.y, 2) < Mathf.Pow(selfEveDistance, 2)) {
                            conflict = true;
                        }
                        if (conflict) {
                            break;
                        }
                    }
                }
                if (!conflict) {
                    calcNewPos = false;
                }
                else {
                    eaveNewPos = (Random.insideUnitCircle * radiusAroundLegitRecv) + legitRecvCenter;
                }
            }
            if (tries >= reCalculationTTries) {
                Debug.Log("Maximum of recalculations for eavesdropper reached!");
            }
            newEavesdropper.transform.position = new Vector3(eaveNewPos.x, 0, eaveNewPos.y);
            newEavesdropper.transform.eulerAngles = new Vector3(0, Random.rotation.eulerAngles.y, 0);
            eavesdropperList.Add(newEavesdropper);
            positionList.Add(eaveNewPos);
        }
        return (eavesdropperList, positionList);
    }

    private List<GameObject> spawnBuildings(Vector2 legitRecvCenter, List<Vector2> positionList) {
        List<GameObject> buildingsList = new List<GameObject>();
        //List<Vector2> positionList = new List<Vector2>();
        //System.Random rnd = new System.Random();
        for (int i = 0; i < spawnNumberBuildings; i++) {
            GameObject newBuilding = Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Count)]);
            newBuilding.transform.SetParent(platform.transform, false);
            Vector2 newPos = Random.insideUnitCircle * (platformRadius - 10);

            bool calcNewPos = true;
            int tries = 0;
            while (calcNewPos && tries < reCalculationTTries) {
                bool conflict = false;
                tries++;
                //Distance to legit receiver
                if (Mathf.Pow(newPos.x - legitRecvCenter.x, 2) + Mathf.Pow(newPos.y - legitRecvCenter.y, 2) < Mathf.Pow(buildingLegitRecvDistance, 2)) {
                    conflict = true;
                }
                //Distance to other buildings
                if (!conflict) {
                    foreach (Vector2 buildingCenter in positionList) {
                        if (Mathf.Pow(newPos.x - buildingCenter.x, 2) + Mathf.Pow(newPos.y - buildingCenter.y, 2) < Mathf.Pow(selfBuildingDistance, 2)) {
                            conflict = true;
                        }
                        if (conflict) {
                            break;
                        }
                    }
                }
                if (!conflict) {
                    calcNewPos = false;
                }
                else {
                    newPos = Random.insideUnitCircle * (platformRadius - 10);
                }
            }
            if (tries >= reCalculationTTries) {
                Debug.Log("Maximum of recalculations for building reached!");
            }
            newBuilding.transform.position = new Vector3(newPos.x, 0, newPos.y);
            newBuilding.transform.eulerAngles = new Vector3(0, Random.rotation.eulerAngles.y, 0);
            buildingsList.Add(newBuilding);
            positionList.Add(newPos);
        }
        return buildingsList;
    }
}

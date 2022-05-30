using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiverScript : MonoBehaviour
{
    private List<GameObject> receivedRays = new List<GameObject>();
    private GameObject satellite;
    private GameObject uiController;

    public void addReceivedRay(GameObject ray) {
        receivedRays.Add(ray);
    }

    public List<GameObject> getReceivedRays() {
        return receivedRays;
    }

    public void addSatellite(GameObject sat) {
        satellite = sat;
    }

    public void addUiController(GameObject uic) {
        uiController = uic;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.name == name) {
                    //Debug.Log("This hit at " + hit.transform.name);
                    //foreach (GameObject rcvdRay in receivedRays) {
                        //Debug.Log("- " + rcvdRay.name);
                    //}
                    satellite.GetComponent<SatelliteScript>().togglePaths(receivedRays, gameObject);
                    uiController.GetComponent<UIControllerScript>().spawnPathsScrollView(receivedRays, gameObject);
                }
            }
        }
    }
}

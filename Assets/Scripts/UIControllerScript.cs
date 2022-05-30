using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIControllerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject rotateToggle, returnToggle, corrMatrixToggle, gameObjectManager, uicanvas;
    [SerializeField]
    private List<GameObject> valueTextList;
    [SerializeField]
    private GameObject pathScrollViewPrefab, pathTextPrefab;
    private GameObject currentPathScrollView;
    private GameObject currentTarget;
    private GameObject currentCorrMatrixScrollView;

    void Start() {
        List<int> startSettingValues = gameObjectManager.GetComponent<GameObjectManager>().getStartingSettingValues();
        for (int i = 0; i < valueTextList.Count; i++) {
            valueTextList[i].GetComponent<Text>().text = startSettingValues[i].ToString(); 
        }
    }

    public void spawnPathsScrollView(List<GameObject> paths, GameObject target) {
        if (currentPathScrollView != null) {
            Destroy(currentPathScrollView);
        }
        if (currentTarget != target) {
            Vector2 offset = new Vector3(150, 0);
            GameObject pathScrollView = Instantiate(pathScrollViewPrefab);
            pathScrollView.transform.SetParent(uicanvas.transform, false);
            currentPathScrollView = pathScrollView;
            currentTarget = target;

            //Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
            //pathScrollView.transform.position = position; //+ offset;

            //Correct y position of StatePanel if over or under screenboundry
            /*
            Vector3[] v = new Vector3[4];
            pathScrollView.GetComponent<RectTransform>().GetWorldCorners(v);
            float maxY = Mathf.Max(v[0].y, v[1].y, v[2].y, v[3].y);
            float minY = Mathf.Min(v[0].y, v[1].y, v[2].y, v[3].y);
            if (maxY > Screen.height) {
                pathScrollView.transform.position = new Vector3(pathScrollView.transform.position.x, pathScrollView.transform.position.y - (maxY - (float)Screen.height), pathScrollView.transform.position.z);
            } 
            else if (minY < 0) {
                pathScrollView.transform.position = new Vector3(pathScrollView.transform.position.x, pathScrollView.transform.position.y - minY, pathScrollView.transform.position.z);
            }
            */

            //Convert screen coords
            Vector2 adjustedPosition = Camera.main.WorldToScreenPoint(target.transform.position);

            adjustedPosition.x *= uicanvas.GetComponent<RectTransform>().rect.width / (float)Camera.main.pixelWidth;
            adjustedPosition.y *= uicanvas.GetComponent<RectTransform>().rect.height / (float)Camera.main.pixelHeight;

            pathScrollView.GetComponent<RectTransform>().anchoredPosition = (adjustedPosition - uicanvas.GetComponent<RectTransform>().sizeDelta / 2f) + offset;

            GameObject content = pathScrollView.transform.GetChild(0).GetChild(0).gameObject;
            foreach (GameObject path in paths) {
                if (!path.name.Contains(",")) {
                    GameObject pathText = Instantiate(pathTextPrefab);
                    pathText.GetComponent<Text>().text = path.name;
                    if (path.tag == "Reflected") {
                        pathText.GetComponent<Text>().color = Color.red;
                    }
                    else if (path.tag == "Scattered") {
                        pathText.GetComponent<Text>().color = Color.green;
                    }
                    else if (path.tag == "Diffracted") {
                        pathText.GetComponent<Text>().color = Color.magenta;
                    }
                    pathText.transform.SetParent(content.transform, false);
                }
            }
        }
        else {
            currentTarget = null;
        }
    }

    public void HandleCorrMatrixToggle(bool toggleValue) {
        if (!toggleValue) {
            Destroy(currentCorrMatrixScrollView);
        }
        else {
            //Vector3 offset = new Vector3(uicanvas.GetComponent<RectTransform>().rect.width, 0, 0);
            List<GameObject> correlationMatrix = gameObjectManager.GetComponent<GameObjectManager>().getCorrelationMatrix();
            currentCorrMatrixScrollView = Instantiate(pathScrollViewPrefab);
            currentCorrMatrixScrollView.transform.SetParent(uicanvas.transform, false);

            //currentCorrMatrixScrollView.transform.position += offset;

            GameObject content = currentCorrMatrixScrollView.transform.GetChild(0).GetChild(0).gameObject;
            foreach (GameObject path in correlationMatrix) {
                if (!path.name.Contains(",")) {
                    GameObject pathText = Instantiate(pathTextPrefab);
                    pathText.GetComponent<Text>().text = path.name;
                    if (path.tag == "Reflected") {
                        pathText.GetComponent<Text>().color = Color.red;
                    }
                    else if (path.tag == "Scattered") {
                        pathText.GetComponent<Text>().color = Color.green;
                    }
                    else if (path.tag == "Diffracted") {
                        pathText.GetComponent<Text>().color = Color.magenta;
                    }
                    pathText.transform.SetParent(content.transform, false);
                }
            }
        }
    }

    public void HandleStartButton() {
        if (currentCorrMatrixScrollView != null) {
            Destroy(currentCorrMatrixScrollView);
            corrMatrixToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
        }
        gameObjectManager.GetComponent<GameObjectManager>().setStartSimulation();
    }

    public void HandleRotateToggle(bool toggleValue) {
        Camera.main.GetComponent<CameraScript>().setRotateAround(toggleValue);
    }

    public void HandleReturnToggle(bool toggleValue) {
        Camera.main.GetComponent<CameraScript>().setTargetReached(!toggleValue);
    }

    public void resetReturnToggle() {
        returnToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
    }

    public void buildingRateSlider(float value) {
        valueTextList[0].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().spawnNumberBuildings = (int) value;
    }

    public void eveRateSlider(float value) {
        valueTextList[1].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().spawnNumberEavesdropper = (int) value;
    }

    public void radiusLegitRecvSlider(float value) {
        valueTextList[2].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().radiusAroundLegitRecv = (int) value;
    }

    public void distEveRecvSlider(float value) {
        valueTextList[3].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().eveLegitRecvDistance = (int) value;
    }

    public void distEveSlider(float value) {
        valueTextList[4].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().selfEveDistance = (int) value;
    }

    public void distBuildingRecvSlider(float value) {
        valueTextList[5].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().buildingLegitRecvDistance = (int) value;
    }

    public void distBuildingSlider(float value) {
        valueTextList[6].GetComponent<Text>().text = value.ToString();
        gameObjectManager.GetComponent<GameObjectManager>().selfBuildingDistance = (int) value;
    }

}

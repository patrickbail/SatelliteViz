using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    private float speed = 100.0f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private float interpolationPrecision = 0.1f;
    [SerializeField]
    private float transitionSpeed = 2.0f;
    [SerializeField]
    private GameObject platform, uic;
    private Vector3 starterPosition;
    private Vector3 starterRotation;
    private bool targetReached = true;
    private bool rotateAround = false;

    public void setTargetReached(bool value) {
        targetReached = value;
    }

    public void setRotateAround(bool value) {
        rotateAround = value;
    }

    void Start() {
        starterPosition = transform.position;
        starterRotation = transform.eulerAngles;
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.L)) {
            rotateAround = !rotateAround;
        }
        if (Input.GetKey(KeyCode.R)) {
            targetReached = false;
        }
        */

        if (targetReached && !rotateAround) {
            Vector3 pos = transform.position;

            if (Input.GetAxis("Vertical") != 0) {
                transform.Translate(Vector3.forward * speed * Input.GetAxis("Vertical") * Time.deltaTime);
            }
            if (Input.GetAxis("Horizontal") != 0) {
                transform.Translate(Vector3.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Q)) {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E)) {
                transform.Translate(Vector3.up * speed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Mouse1)) {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                
                Vector3 rotateValue = new Vector3(mouseY, mouseX * -1 * rotationSpeed, 0);
                transform.eulerAngles -= rotateValue;
            }
        }

        else if (targetReached && rotateAround) {
            if (Input.GetAxis("Horizontal") != 0) {
                transform.RotateAround(platform.transform.position, new Vector3(0, 1, 0), speed * Input.GetAxis("Horizontal") * Time.deltaTime);
                starterPosition = transform.position;
                starterRotation = transform.eulerAngles;
            }
        }

        else {
            transform.position = Vector3.Lerp(transform.position, starterPosition, transitionSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, starterRotation, transitionSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, starterPosition) <= interpolationPrecision 
                && Vector3.Distance(transform.eulerAngles, starterRotation) <= interpolationPrecision) {
                targetReached = true;
                uic.GetComponent<UIControllerScript>().resetReturnToggle();
                Debug.Log("Target Reached");
            }
        }
    }
}

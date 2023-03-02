using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLight : MonoBehaviour
{
    [SerializeField] private float maxSize;
    [SerializeField] private float dissapearAngle;

    [SerializeField] private Transform orientedTo;
    [SerializeField] private Transform lightTransform;
    private Camera cam;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        changeBrightness();
    }

    private void changeBrightness()
    {
        Vector3 orientedDir = orientedTo.transform.position - transform.position;
        Vector3 toCamera = cam.transform.position - transform.position;

        float angle = Vector3.Angle(orientedDir, toCamera);

        float coef = angle/dissapearAngle;
        lightTransform.localScale = Vector3.one * Mathf.Lerp(maxSize,0, coef);
        lightTransform.LookAt(cam.transform);
    }

    private bool glow = false;
    public void turnON()
    {
        glow = true;
        gameObject.SetActive(true);
    }

    public void turnOFF()
    {
        glow = false;
        gameObject.SetActive(false);
    }
}

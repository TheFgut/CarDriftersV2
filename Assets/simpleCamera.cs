using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothStep = 0.1f;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - target.position ;
    }
    private void LateUpdate()
    {
        SmoothFollow();
    }

    public void SmoothFollow()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 smoothFollow = Vector3.Lerp(transform.position,
        targetPos, smoothStep);

        transform.position = smoothFollow;
        transform.LookAt(target);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarControls : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Player controlledCar;
    public void OnPointerUp(PointerEventData eventData)
    {
        controlledCar.goToNextCircle();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {

    }
}

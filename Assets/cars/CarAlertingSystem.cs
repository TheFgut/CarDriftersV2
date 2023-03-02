using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAlertingSystem : MonoBehaviour
{
    [SerializeField] private float timeBetweenChecks = 0.2f;
    [SerializeField] private float alertingDistance = 0.5f;
    [SerializeField] private float carMidHeigth = 0.5f;
    private float timer;

    private CarMover mover;
    void Start()
    {
        mover = GetComponent<CarMover>();
    }
    [SerializeField] private bool triggerSearching; 
    void FixedUpdate()
    {
        if (triggerSearching == false)
        {
            loockingForCarsTick();
        }
        else
        {
            triggerAlertTick();
        }
    }

    private List<CarAlertSystem> alertsSystemsInTrigger = new List<CarAlertSystem>();
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "car")
        {
            CarAlertSystem sys = other.GetComponent<CarAlertSystem>();
            alertsSystemsInTrigger.Add(sys);
            sys.Alert();
        }
    }
    private void triggerAlertTick()
    {
        foreach (CarAlertSystem sys in alertsSystemsInTrigger)
        {
            setAlertingPowerTo(sys, (transform.position - sys.transform.position).magnitude);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "car")
        {
            if (other.tag == "car")
            {
                CarAlertSystem sys = other.GetComponent<CarAlertSystem>();
                alertsSystemsInTrigger.Remove(sys);
                sys.Stop();
            }
        }
    }

    public void loockingForCarsTick()
    {
        timer -= Time.fixedDeltaTime;
        if (timer < 0)
        {
            List<CarAlertSystem> carAlertSystems = getCarsAlertSysThatInDanger(mover.speed);
            updateCarAlertSystemsState(carAlertSystems);
            timer = timeBetweenChecks;
        }
    }

    private List<CarAlertSystem> prevSystems = new List<CarAlertSystem>();
    public void updateCarAlertSystemsState(List<CarAlertSystem> carAlertSystems)
    {
        foreach(CarAlertSystem sys in carAlertSystems)
        {
            if (!prevSystems.Contains(sys))
            {
                sys.Alert();
            }
            else
            {
                prevSystems.Remove(sys);
            }
        }

        foreach (CarAlertSystem sysToStop in prevSystems)
        {
            sysToStop.Stop();
        }
        prevSystems = carAlertSystems;
    }

    private const float castAngleStep = 15;
    public List<CarAlertSystem> getCarsAlertSysThatInDanger(float carMoveSpeed)
    {
        List<CarAlertSystem> affectedCars = new List<CarAlertSystem>();


        float angleDrag = 0;
        for (int num = 0; num < 2;num++)
        {
            
            float distance = num == 0 ? carMoveSpeed + alertingDistance : alertingDistance;
            float angleFactor = -castAngleStep;
            for (int i = 0; i < 3; i++)
            {

                CarAlertSystem found = castRayAndReturnAlertSys(transform.position + new Vector3(0, carMidHeigth,0),
                    math.rotVector(new Vector3(0, 0, 1), transform.rotation * 
                    Quaternion.Euler(0, angleFactor + angleDrag, 0)), distance);
                if (found != null && !affectedCars.Contains(found))
                {
                    affectedCars.Add(found);
                }
                angleFactor += castAngleStep;
            }
            angleDrag = 180;
        }

        return affectedCars;
    }

    [Header("shadow params")]
    [SerializeField]private float DistToFullDangerShadowAppear;
    public CarAlertSystem castRayAndReturnAlertSys(Vector3 origin,Vector3 direction, float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, distance, LayerMask.GetMask("Car")))
        {
            CarAlertSystem alertingSys = hit.collider.GetComponent<CarAlertSystem>();
            setAlertingPowerTo(alertingSys, hit.distance); 
            return alertingSys;
        }

        return null;
    }

    private void setAlertingPowerTo(CarAlertSystem alertSys, float hitDist)
    {
        float power = 1;
        hitDist -= DistToFullDangerShadowAppear;
        float coef = alertingDistance - DistToFullDangerShadowAppear;
        if(hitDist > 0)
        {
            power = 1 - (hitDist / coef);
        }
        alertSys.setAlertPower(easeOutCubic(power));
    }

    private float easeOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }
}

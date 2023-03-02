using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayCircle : MonoBehaviour
{
    [SerializeField]private bool clockwiseMovement = true;

    public bool clockwise { get { return clockwiseMovement; } }

    [Header("basic params")]
    [SerializeField] private float radius;

    public float getRadius()
    {
        return radius;
    }

    [SerializeField] private float speedInAngles;

    public float getAngleSpeed()
    {
        return speedInAngles;
    }

    [Header("params that have impact on player")]
    [SerializeField] private ConnectedCircles nextCirclesSetUp;
    public ConnectedCircles nextCircles { get { return nextCirclesSetUp; } }
    [SerializeField] private MoveLineModule line;

    public MoveLineModule getMoveLineModule()
    {
        return line;
    }

    private List<DestinationParams> controllingCars = new List<DestinationParams>();
    private const float anglesTraversWhenConnected = 25;
    public DestinationParams connectToCircle(CarMover car)
    {
        DestinationParams carsMovingParams = new DestinationParams(car, getCircleSpeed(),
            radius, transform.position);
        updateDestinationOfCar(carsMovingParams, anglesTraversWhenConnected);
        controllingCars.Add(carsMovingParams);
        return carsMovingParams;
    }

    private float getCircleSpeed()
    {
        float C = 2 * Mathf.PI * radius;
        float spd = (C / 360f) * speedInAngles;
        return spd;
    }

    public void enableDirectionLine()
    {
        line.enable();
    }

    public void disableDirectionLine()
    {
        line.disable();
    }

    public void disconnectFromCircle(CarMover car)
    {
        for (int i = 0; i < controllingCars.Count; i++)
        {
            if (controllingCars[i].sameCar(car))
            {
                controllingCars.RemoveAt(i);
                return;
            }
        }
    }

    void Start()
    {
        line.init(this);
        nextCirclesSetUp.Init(transform, line);
    }

    void FixedUpdate()
    {
        float speedModifiedByTime = speedInAngles * Time.fixedDeltaTime;
        foreach (DestinationParams carMove in controllingCars)
        {
            if (carMove.waiting)
            {
                carMove.waitingToCarApproach();
                continue;
            }
            updateDestinationOfCar(carMove, speedModifiedByTime);
        }
        line.UpdateLine();
    }

    private void updateDestinationOfCar(DestinationParams carMove, float angleTimeModified)
    {
        Vector3 previousPos = (carMove.destination - transform.position).normalized;
        Vector3 newPos = math.rotVector(previousPos * radius, Quaternion.Euler(0,
           clockwiseMovement == true ? angleTimeModified :
           angleTimeModified * -1, 0));

        carMove.updateDestination(transform.position + newPos);
    }


  
}


public class DestinationParams
{
    private CarMover car;
    public bool waiting { get; private set; }
    private const float distToActivate = 0.8f;

    public void waitingToCarApproach()
    {
        if((car.transform.position - destination).magnitude <= distToActivate)
        {
            waiting = false;
        }
    }

    public bool sameCar(CarMover anotherCar)
    {
        return ReferenceEquals(car, anotherCar);
    }

    public float maxMoveSpeed { get; private set; }
    public DestinationParams(CarMover car, float moveSpeed,float radius,Vector3 circlePos)
    {
        maxMoveSpeed = moveSpeed;
        destination = (car.transform.position - circlePos).normalized * radius +
            circlePos;
        this.car = car;
        waiting = true;
    }

    public Vector3 destination { get; private set; }

    public void updateDestination(Vector3 newDest)
    {
        direction = (newDest - destination).normalized;
        destination = newDest;
    }

    public Vector3 direction { get; private set; }
}

public static class math
{
    public static Vector3 rotVector(Vector3 vector, Quaternion rotation)
    {
        float magnitude = vector.magnitude;
        vector.Normalize();
        Quaternion q = new Quaternion(vector.x, vector.y, vector.z, 0);
        q = rotation * q * Quaternion.Inverse(rotation);
        return new Vector3(q.x, q.y, q.z) * magnitude;
    }
}
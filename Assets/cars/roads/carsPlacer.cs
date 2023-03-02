using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class carsPlacer : MonoBehaviour
{
    private WayCircle circle;

    [SerializeField] private List<Car> cars;
    [SerializeField] private Placer placer;

    protected void addNewCar(CarMover mover)
    {
        mover.transform.parent = transform;
        Car n = new Car();
        n.setInstance(mover);
        cars.Add(n);
        
    }

    void Awake()
    {
        circle = GetComponent<WayCircle>();
        placer.Init(this);
    }
    void LateUpdate()
    {
        if (!Application.isEditor || Application.isPlaying)
        {
            this.enabled = false;
        }

        placer.tryToPlace();

        Vector3 circPos = circle.transform.position;
        float circRadius = circle.getRadius();

        if (cars != null && cars.Count != 0)
        {
            foreach (Car car in cars)
            {
                car.updateCircPos(circPos, circRadius, InEditorCarMoveSimulator.instance.time* circle.getAngleSpeed(), circle.clockwise);
            }
        }

    }
    [System.Serializable]
    private class Car
    {
        [SerializeField] private CarMover carInstance;
        [SerializeField] private float anglePos;
        private float prevAngle;

        internal void setInstance(CarMover carInstance)
        {
            this.carInstance = carInstance;
        }

        public void updateCircPos(Vector3 circPos, float radius, float additionAngle,bool clockwise)
        {
            float newAngle = anglePos + additionAngle;
            if (newAngle == prevAngle) return;
            prevAngle = newAngle;

            float clockwiseAngles = clockwise ? 90 : -90;
            carInstance.transform.rotation = Quaternion.Euler(0, newAngle + clockwiseAngles, 0);

            Vector3 newDif = math.rotVector(new Vector3(0,0,radius), Quaternion.Euler(0, newAngle, 0));
            carInstance.transform.position = circPos + newDif;
        }
    }

    [System.Serializable]
    private class Placer
    {
        [SerializeField] private CarMover[] carsPrefabs;
        [SerializeField] private int placeId;
        [SerializeField] private bool place;

        private carsPlacer main;

        public void Init(carsPlacer main)
        {
            this.main = main;
        }

        public void tryToPlace()
        {
            if(place == true)
            {
                CarMover placed = Object.Instantiate(carsPrefabs[placeId]);
                main.addNewCar(placed);
                place = false;
            }
        }
    }
}
#endif
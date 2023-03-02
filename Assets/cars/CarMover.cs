using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMover : MonoBehaviour
{
    [SerializeField] protected WayCircle wayCircle;

    [Header("movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float minRotSpeed;
    [SerializeField] private float maxRotSpeed;

    [SerializeField] protected PhysicalModule physicalModule;

    public PhysicalModule getPhysicalModule()
    {
        return physicalModule;
    }

    protected DestinationParams movementCircle;
    private Rigidbody rig;
    void Start()
    {

        movementCircle = wayCircle.connectToCircle(this);
        rig = GetComponent<Rigidbody>();
        physicalModule.Init(rig, this);
    }


    void FixedUpdate()
    {
        if (functionalEnabled)
        {
            MoveTo(movementCircle.destination);
        }


    }



    public float speed { get; private set; }
    protected void MoveTo(Vector3 destination)
    {
        //rotation
        Vector3 loockDir = math.rotVector(new Vector3(0, 0, 1),transform.rotation);
        Vector3 toTargetVect = destination - transform.position;
        float needToRotAngle = Vector3.SignedAngle(loockDir, toTargetVect, new Vector3(0, 1, 0));


        if (Mathf.Abs(needToRotAngle) > 1f)
        {
            float rotSpeed = Mathf.Sign(needToRotAngle) *
        Mathf.Lerp(minRotSpeed, maxRotSpeed, Mathf.Abs(needToRotAngle) / 90f)
        * Time.fixedDeltaTime;
            transform.Rotate(0, rotSpeed, 0);
        }

        


        //movement
        float maxSpd = movementCircle.waiting ? maxSpeed : movementCircle.maxMoveSpeed;

        float magnitude = toTargetVect.magnitude;
        if (magnitude > 0.05f)
        {
            Vector3 positionChangeVect = loockDir * Time.fixedDeltaTime *
                 Mathf.Lerp(maxSpd, minSpeed, Mathf.Abs(needToRotAngle) / 90f);
            speed = positionChangeVect.magnitude;
            transform.position += positionChangeVect;
        }

    }

    private bool functionalEnabled = true;
    public void disable()
    {
        wayCircle.disconnectFromCircle(this);
        functionalEnabled = false;
    }

    public void enable()
    {
        movementCircle = wayCircle.connectToCircle(this);
        functionalEnabled = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "car")
        {
            physicalModule.collisionWithCarHappened(collision);
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "car")
        {
            physicalModule.stayInCollision(collision);
        }
    }

    [System.Serializable]
    public class PhysicalModule
    {
        [SerializeField] private float anotheObjImpactPower = 1;
        [SerializeField] private float selfImpactPower = 1;
        private Rigidbody rig;
        private CarMover mover;
        public void Init(Rigidbody rigidbody, CarMover mover)
        {
            rig = rigidbody;
            this.mover = mover;
        }

        public void Impact(Vector3 point, Vector3 normal)
        {
            normal.y = 0;
            normal.Normalize();

            rig.constraints = RigidbodyConstraints.None;
            rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rig.AddForce(-normal * anotheObjImpactPower);

            mover.disable();
            mover.StartCoroutine(returnControlsRoutine());
        }

        //private IEnumerator destroyRoutine()
        //{
        //    mover.transform.parent = null;
        //    float destroyTimer = destroyTime;
        //    do
        //    {
        //        destroyTimer -= Time.fixedDeltaTime;
        //        Vector3 scale = Vector3.Lerp(Vector3.one, Vector3.zero, easeInExpo(1 - (destroyTimer / destroyTime)));
        //        mover.transform.localScale = scale;
        //        yield return new WaitForFixedUpdate();
        //    } while (destroyTimer > 0);
        //    Destroy(mover.gameObject);
        //}

        private IEnumerator returnControlsRoutine()
        {
            float minEffectTime = 1f;
            do
            {
                minEffectTime -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            } while (rig.velocity.magnitude > 0.01f || minEffectTime > 0);
            rig.constraints = RigidbodyConstraints.FreezeAll;
            mover.enable();
        }

        private float easeInExpo(float x)
        {
            return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
        }

        public void collisionWithCarHappened(Collision collision)
        {
            CollisionWithCar.collide(collision);
        }

        public void stayInCollision(Collision collision)
        {
            CollisionWithCar.stayInCollide(collision);
        }

        
        public void tryToLeaveCollision(Vector3 point, Vector3 normal)
        {
            if(rig.velocity.magnitude < 0.05f)
            {
                Impact(point, normal);
            }
        }
    }
}

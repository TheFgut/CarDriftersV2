using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class CollisionWithCarSettings
{
    [SerializeField] private ParticleSystem collideParticles;
}
static class CollisionWithCar
{

    public static void collide(Collision collision)
    {
        CarMover mover = collision.gameObject.GetComponent<CarMover>();
        mover.getPhysicalModule().Impact(getContactPoint(collision.contacts),
            getNormal(collision.contacts));
    }

    public static void stayInCollide(Collision collision)
    {
        CarMover mover = collision.gameObject.GetComponent<CarMover>();
        mover.getPhysicalModule().tryToLeaveCollision(getContactPoint(collision.contacts),
            getNormal(collision.contacts));
    }

    private static Vector3 getNormal(ContactPoint[] contacts)
    {
        Vector3 normal = new Vector3();
        foreach (ContactPoint contact in contacts)
        {
            normal += contact.normal;
        }
        normal.Normalize();
        return normal;
    }

    private static Vector3 getContactPoint(ContactPoint[] contacts)
    {
        Vector3 point = new Vector3();
        foreach (ContactPoint contact in contacts)
        {
            point += contact.point;
        }
        point /= contacts.Length;
        return point;
    }
}

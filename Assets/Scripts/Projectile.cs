using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ShotType
{
    UNUSED = 0,
    PISTOL,
    ARTILLERY,
    FIREBALL,
    LASER
}

public class CycloneParticle
{
    public Rigidbody body;
    public Vector3 acceleration;
    public float damping;
    public Vector3 forceAccum;

    public float inverseMass;

    public void integrate(float duration)
    {
        // We don't integrate things with zero mass.
        if (inverseMass <= 0.0f) return;

        //assert(duration > 0.0);

        // Update linear position.

        addScaledVector(body.position, body.velocity, 1.0f/* duration */);

        // Work out the acceleration from the force
        Vector3 resultingAcc = acceleration;
        addScaledVector(resultingAcc, forceAccum, inverseMass);

        // Update linear velocity from the acceleration.
        addScaledVector(body.velocity, resultingAcc, duration);

        // Impose drag.
        body.velocity *= Mathf.Pow(damping, duration);

        // Clear the forces.
        forceAccum.Set(0f, 0f, 0f);
    }

    private void addScaledVector(Vector3 destVector, Vector3 addingVec, float scale)
    {
        destVector.x += addingVec.x * scale;
        destVector.y += addingVec.y * scale;
        destVector.z += addingVec.z * scale;
    }
}
public class AmmoRound
{
    public CycloneParticle particle;
    public ShotType type;
    public uint startTime;

    public void setMass(float mass)
    {
        particle.body.mass = mass;
    }
    public void setVelocity(Vector3 velocity)
    {
        particle.body.velocity = velocity;
    }

    public void setAcceleration(Vector3 acceleration)
    {
        particle.acceleration = acceleration;
    }
    public void setDamping(float damping)
    {
        particle.damping = damping;
    }

    public void setPosition(Vector3 position)
    {
        particle.body.position = position;
    }

    public Vector3 getAcceleration()
    {
        return particle.acceleration;
    }

    public void clearAccumulator()
    {
        particle.forceAccum.Set(0f, 0f, 0f);
    }

    void addForce(Vector3 force)
    {
        particle.forceAccum += force;
    }

}
class Projectile : MonoBehaviour, IPointerUpHandler
{

    [SerializeField] private Transform TargetObject;
    [Range(1.0f, 6.0f)] public float TargetRadius;
    [Range(20.0f, 75.0f)] public float LaunchAngle;

    public GameObject pistol, artillery, fireball, laser;

    private bool bTargetReady;

    private Rigidbody rigid;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float duration;

    public readonly static uint ammoRounds = 2;

    AmmoRound[] ammo = new AmmoRound[ammoRounds];

    ShotType currentShotType;

    private float lastFrameTimestamp;

    public void Start()
    {
        duration = 0;
        lastFrameTimestamp = 0;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        fire();
    }
    public void fire()
    {
        // Find the first available round.
        AmmoRound tempShot = null;
        foreach (var shot in ammo)
        {
            tempShot = shot;
            if (shot.type == ShotType.UNUSED) break;
        }

        // If we didn't find a round, then exit - we can't fire.
        if (tempShot == ammo[ammo.Length - 1]) return;

        // Set the properties of the particle
        switch (currentShotType)
        {
            case ShotType.PISTOL:
                tempShot.setMass(2.0f); // 2.0kg
                tempShot.setVelocity(new Vector3(0.0f, 0.0f, 35.0f)); // 35m/s
                tempShot.setAcceleration(new Vector3(0.0f, -1.0f, 0.0f));
                tempShot.setDamping(0.99f);
                break;

            case ShotType.ARTILLERY:
                tempShot.setMass(200.0f); // 200.0kg
                tempShot.setVelocity(new Vector3(0.0f, 30.0f, 40.0f)); // 50m/s
                tempShot.setAcceleration(new Vector3(0.0f, -20.0f, 0.0f));
                tempShot.setDamping(0.99f);
                break;

            case ShotType.FIREBALL:
                tempShot.setMass(1.0f); // 1.0kg - mostly blast damage
                tempShot.setVelocity(new Vector3(0.0f, 0.0f, 10.0f)); // 5m/s
                tempShot.setAcceleration(new Vector3(0.0f, 0.6f, 0.0f)); // Floats up
                tempShot.setDamping(0.9f);
                break;

            case ShotType.LASER:
                // Note that this is the kind of laser bolt seen in films,
                // not a realistic laser beam!
                tempShot.setMass(0.1f); // 0.1kg - almost no weight
                tempShot.setVelocity(new Vector3(0.0f, 0.0f, 100.0f)); // 100m/s
                tempShot.setAcceleration(new Vector3(0.0f, 0.0f, 0.0f)); // No gravity
                tempShot.setDamping(0.99f);
                break;
        }

        // Set the data common to all particle types
        tempShot.setPosition(new Vector3(0.0f, 1.5f, 0.0f));
        //tempShot.startTime = TimingData get().lastFrameTimestamp;
        tempShot.type = currentShotType;

        // Clear the force accumulators
        tempShot.clearAccumulator();
    }

    public void Update()
    {
        duration += Time.deltaTime;
        lastFrameTimestamp += Time.deltaTime;
        //if (duration <= 0.0f) return;

        // Update the physics of each particle in turn
        foreach (var shot in ammo)
        {
            if (shot.type != ShotType.UNUSED)
            {
                // Run the physics
                shot.particle.integrate(duration);

                // Check if the particle is now invalid
                if (shot.particle.body.position.y < 0.0f ||
                    shot.startTime + 5000 < lastFrameTimestamp ||
                      shot.particle.body.position.z > 200.0f)
                {
                    // We simply set the shot type to be unused, so the
                    // memory it occupies can be reused by another shot.
                    shot.type = ShotType.UNUSED;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentShotType = ShotType.PISTOL;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentShotType = ShotType.ARTILLERY;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentShotType = ShotType.FIREBALL;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentShotType = ShotType.LASER;
        }

        SetShot();
    }

    public void SetShot()
    {
        foreach (var shot in ammo)
        {
            shot.type = currentShotType;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        fire();
    }
}



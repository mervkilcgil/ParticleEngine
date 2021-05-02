using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cyclone
{
    public class CycloneParticle : MonoBehaviour
    {
        [SerializeField] protected Rigidbody body;
        [SerializeField] protected Vector3 acceleration;
        [SerializeField] protected double damping;
        [SerializeField] protected Vector3 forceAccum;

        [SerializeField] protected float inverseMass;

        [SerializeField] protected Vector3 velocity;

        public Rigidbody Body
        {
            get => body;
            set => body = value;
        }

        public Vector3 Acceleration
        {
            get => acceleration;
            set => acceleration = value;
        }

        public Vector3 ForceAccum
        {
            get => forceAccum;
            set => forceAccum = value;
        }

        public double Damping
        {
            get => damping;
            set => damping = value;
        }

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
            body.velocity *= Mathf.Pow((float)damping, duration);

            // Clear the forces.
            forceAccum.Set(0f, 0f, 0f);
        }

        public void addScaledVector(Vector3 destVector, Vector3 addingVec, float scale)
        {
            destVector.x += addingVec.x * scale;
            destVector.y += addingVec.y * scale;
            destVector.z += addingVec.z * scale;
        }

        public void setPosition(Vector3 pos)
        {
            transform.position = pos;
        }
        
        public Vector3 getPosition()
        {
            return transform.position;
        }
        
        
        public void setVelocity(Vector3 vel)
        {
            velocity = vel;
        }
        
        public Vector3 getVelocity()
        {
            return velocity;
        }
        
        public void clearAccumulator()
        {
            forceAccum = Vector3.zero;
        }
        
        public void addForce(Vector3 force)
        {
            forceAccum += force;
        }
        
        public void setMass(float mass)
        {
            if(mass != 0) inverseMass = ((float)1.0)/mass;
        }
    }
}

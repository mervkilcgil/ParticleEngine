using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cyclone
{
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
}

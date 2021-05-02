
using System;
using System.Runtime.InteropServices;
using Cyclone;
using UnityEngine;
using Random = UnityEngine.Random;

public class Firework : CycloneParticle
{
    public uint type;
    public float age;
    public bool update(float duration)
    {
        integrate(duration);
        age -= duration;
        return (age < 0) || (this.transform.position.y < 0);
    }
};

public class FireworkRule 
{
    uint type;

    float minAge;

    float maxAge;

    Vector3 minVelocity;

    Vector3 maxVelocity;

    double damping;

    public class Payload
    {
        public uint type;

        public uint count;
        public void set(uint type, uint count)
        {
            this.type = type;
            this.count = count;
        }
    };

    public uint payloadCount;

    public Payload[] payloads;

    public FireworkRule()
    {
        payloadCount = 0;
        payloads = null;
    }

    public void init(uint payloadCount)
    {
        this.payloadCount = payloadCount;
        payloads = new Payload[payloadCount];
        for (int j = 0; j < payloads.Length; j++)
        {
            if (payloads[j] == null)
                payloads[j] = new Payload();
        }
    }

    public void setParameters(uint type, float minAge, float maxAge,
        Vector3 minVelocity, Vector3 maxVelocity,
        double damping)
    {
        this.type = type;
        this.minAge = minAge;
        this.maxAge = maxAge;
        this.minVelocity = minVelocity;
        this.maxVelocity = maxVelocity;
        this.damping = damping;
    }

    public void create(Firework firework, Firework parent = null)
    {
        firework.type = type;
        firework.age = Random.Range(minAge, maxAge + 1);

        Vector3 vel = Vector3.zero;
        if (parent != null) 
        {
            firework.setPosition(parent.getPosition());
            vel += parent.getVelocity();
        }
        else
        {
            Vector3 start = Vector3.zero;
            int x = (int)randomInt(3) - 1;
            start.x = 5.0f * x;
            firework.setPosition(start);
        }

        vel += randomVector(minVelocity, maxVelocity);
        firework.setVelocity(vel);

        // We use a mass of one in all cases (no point having fireworks
        // with different masses, since they are only under the influence
        // of gravity).
        firework.setMass(1);

        firework.Damping = this.damping;

        firework.Acceleration = new Vector3(0, -9.81f, 0);

        firework.clearAccumulator();
    }
    
    private float randomInt(uint max)
    {
        return randomBits()  % max;
    }
    
    private int rotl(int n, int r)
    {
        return	(n << r) |
                (n >> (32 - r));
    }

    private int rotr(int n, int r)
    {
        return	(n >> r) |
                (n << (32 - r));
    }
    
    int randomBits()
    {
        int p1, p2;
        float s = Time.time;
        p1 = 0;  p2 = 10;
        int[] buffer = new int[17];

        // Fill the buffer with some basic random numbers
        for (uint i = 0; i < 17; i++)
        {
            // Simple linear congruential generator
            s = s * 2891336453 + 1;
            buffer[i] = (int)s;
        }
        
        // Rotate the buffer and store it back to itself
        buffer[p1] = rotl(buffer[p2], 13) + rotl((int)buffer[p1], 9);

        // Return result
        return buffer[p1];
    }
    
    private Vector3 randomVector(Vector3 min, Vector3 max)
    {
        return new Vector3(
            randomReal(min.x, max.x),
            randomReal(min.y, max.y),
            randomReal(min.z, max.z)
        );
    }
    
    private float randomBinomial(float scale)
    {
        return (randomReal()-randomReal())*scale;
    }
    
    private float randomReal()
    {
        // Get the random number
        int bits = randomBits();

        // Set up a reinterpret structure for manipulation
        
        float value = BitConverter.ToSingle(BitConverter.GetBytes((bits >> 9) | 0x3f800000), 0);

        // And return the value
        return value - 1.0f;
    }
    
    private float randomReal(float min, float max)
    {
        return randomReal() * (max-min) + min;
    }
    

};



class Firework : CycloneParticle
{
    public unsigned type;
    public int age;
    public bool update(int duration)
    {
        integrate(duration);
        age -= duration;
        return (age < 0) || (position.y < 0);
    }
};

struct FireworkRule
{
    unsigned type;

    int minAge;

    int maxAge;

    Vector3 minVelocity;

    Vector3 maxVelocity;

    int damping;

    struct Payload
    {
        unsigned type;

        unsigned count;
        public void set(unsigned type, unsigned count)
        {
            this.type = type;
            this.count = count;
        }
    };

    unsigned payloadCount;

    Payload[] payloads;

    public FireworkRule()
    {
        payloadCount = 0;
        payloads = null;
    }

    public void init(unsigned payloadCount)
    {
        this.payloadCount = payloadCount;
        payloads = new Payload[payloadCount];
    }

    public void setParameters(unsigned type, int minAge, int maxAge,
        Vector3 minVelocity, Vector3 maxVelocity,
        int damping)
    {
        this.type = type;
        this.minAge = minAge;
        this.maxAge = maxAge;
        this.minVelocity = minVelocity;
        this.maxVelocity = maxVelocity;
        this.damping = damping;
    }

    public void create(Firework firework, Firework parent = NULL)
    {
        firework.type = type;
        firework.age = crandom.randomReal(minAge, maxAge);

        Vector3 vel;
        if (parent != null) 
        {
            firework.setPosition(parent.getPosition());
            vel += parent.getVelocity();
        }
        else
        {
            Vector3 start;
            int x = (int)crandom.randomInt(3) - 1;
            start.x = 5.0f * x;
            firework.setPosition(start);
        }

        vel += crandom.randomVector(minVelocity, maxVelocity);
        firework.setVelocity(vel);

        // We use a mass of one in all cases (no point having fireworks
        // with different masses, since they are only under the influence
        // of gravity).
        firework.setMass(1);

        firework.setDamping(damping);

        firework.setAcceleration(Vector3::GRAVITY);

        firework.clearAccumulator();
    }
};


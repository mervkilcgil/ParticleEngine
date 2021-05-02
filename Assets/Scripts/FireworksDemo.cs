using UnityEngine;
using UnityEngine.EventSystems;

class FireworksDemo : MonoBehaviour, IPointerUpHandler
{
    static uint maxFireworks = 1024;
    Firework[] fireworks = new Firework[maxFireworks];
    uint nextFirework;
    static uint ruleCount = 9;
    FireworkRule[] rules = new FireworkRule[ruleCount];

    public GameObject fireworkPrefab;

    private bool isStarted;

    /** Dispatches a firework from the origin. */
    public void create(uint type, Firework parent = null)
    {
        // Get the rule needed to create this firework
        FireworkRule rule = rules[type - 1];

        // Create the firework
        rule.create(fireworks[nextFirework], parent);

        // Increment the index for the next firework
        nextFirework = (nextFirework + 1) % maxFireworks;
    }
    public void create(uint type, uint number, Firework parent)
    {
        init(number);
        for (uint i = 0; i < number; i++)
        {
            create(type, parent);
        }
    }

    public void initFireworkRules()
    {
        // Go through the firework types and create their rules.
        rules[0].init(2);
        rules[0].setParameters(
            1, // type
            0.5f, 1.4f, // age range
            new Vector3(-5, 25, -5), // min velocity
            new Vector3(5, 28, 5), // max velocity
            0.1 // damping
            );
        rules[0].payloads[0].set(3, 5);
        rules[0].payloads[1].set(5, 5);

        rules[1].init(1);
        rules[1].setParameters(
            2, // type
            0.5f, 1.0f, // age range
            new Vector3(-5, 10, -5), // min velocity
            new Vector3(5, 20, 5), // max velocity
            0.8 // damping
            );
        rules[1].payloads[0].set(4, 2);

        rules[2].init(0);
        rules[2].setParameters(
            3, // type
            0.5f, 1.5f, // age range
            new Vector3(-5, -5, -5), // min velocity
            new Vector3(5, 5, 5), // max velocity
            0.1 // damping
            );

        rules[3].init(0);
        rules[3].setParameters(
            4, // type
            0.25f, 0.5f, // age range
            new Vector3(-20, 5, -5), // min velocity
            new Vector3(20, 5, 5), // max velocity
            0.2 // damping
            );

        rules[4].init(1);
        rules[4].setParameters(
            5, // type
            0.5f, 1.0f, // age range
            new Vector3(-20, 2, -5), // min velocity
            new Vector3(20, 18, 5), // max velocity
            0.01 // damping
            );
        rules[4].payloads[0].set(3, 5);

        rules[5].init(0);
        rules[5].setParameters(
            6, // type
            3, 5, // age range
            new Vector3(-5, 5, -5), // min velocity
            new Vector3(5, 10, 5), // max velocity
            0.95 // damping
            );

        rules[6].init(1);
        rules[6].setParameters(
            7, // type
            4, 5, // age range
            new Vector3(-5, 50, -5), // min velocity
            new Vector3(5, 60, 5), // max velocity
            0.01 // damping
            );
        rules[6].payloads[0].set(8, 10);

        rules[7].init(0);
        rules[7].setParameters(
            8, // type
            0.25f, 0.5f, // age range
            new Vector3(-1, -1, -1), // min velocity
            new Vector3(1, 1, 1), // max velocity
            0.01 // damping
            );

        rules[8].init(0);
        rules[8].setParameters(
            9, // type
            3, 5, // age range
            new Vector3(-15, 10, -5), // min velocity
            new Vector3(15, 15, 5), // max velocity
            0.95 // damping
            );
        // ... and so on for other firework types ...
    }


    public void init(uint number)
    {
        isStarted = true;
        nextFirework = 0;
        for(int i = 0; i < number; i++)
        {
            if (fireworks[i] == null)
            {
                GameObject firework = Instantiate(fireworkPrefab);
                fireworks[i] = firework.GetComponent<Firework>();
            }
            fireworks[i].type = 0;
        }
        
        for(int i = 0; i < rules.Length; i++)
        {
            if (rules[i] == null) 
                rules[i] = new FireworkRule();
        }

        // Create the firework types
        initFireworkRules();
    }

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                create(1, 100, null); 
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                create(2, 100, null);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                create(3, 100, null);
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                create(4, 100, null);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                create(5, 100, null);
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                create(6, 100, null); 
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                create(7, 100, null);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                create(8, 100, null);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                create(9, 100, null);
            }
        }

        if (isStarted)
        {

            foreach (var firework in fireworks)
            {
                // Check if we need to process this firework.
                if (firework != null && firework.type > 0)
                {
                    // Does it need removing?
                    if (firework.update(Time.deltaTime))
                    {
                        // Find the appropriate rule
                        FireworkRule rule = rules[firework.type - 1];

                        // Delete the current firework (this doesn't affect its
                        // position and velocity for passing to the create function,
                        // just whether or not it is processed for rendering or
                        // physics.
                        firework.type = 0;

                        // Add the payload
                        for (uint i = 0; i < rule.payloadCount; i++)
                        {
                            FireworkRule.Payload payload = rule.payloads[i];
                            create(payload.type, payload.count, firework);
                        }
                        DestroyImmediate(firework.gameObject);
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
};

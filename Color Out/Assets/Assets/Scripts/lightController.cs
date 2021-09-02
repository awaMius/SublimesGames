using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightController : MonoBehaviour
{

    public float speed = 0.10f;
    public Light light;

    public bool up;


    public float maxIntensity = 0.9f;
    public float MinIntensity = 1.3f;



    // Update is called once per frame
    void FixedUpdate()
    {


        if (up) light.intensity = light.intensity + speed;
        if (!up) light.intensity = light.intensity - speed;

        if (light.intensity >= maxIntensity && up) up = false;
        if (light.intensity <= MinIntensity && !up) up = true;


    }
}

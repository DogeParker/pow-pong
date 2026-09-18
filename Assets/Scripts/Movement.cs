using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Assemblies;
using NUnit.Framework.Constraints;

public class Movement : MonoBehaviour
{   
    //Notes: will have to add flipping of hitbox when moving left, as well as changing the sprite to backhand neutral
    public float speed = 0f;
    public float maxSpeed = 50;
    public float accelerationRate = 5;
    public double friction = 0.9; //multiplied by current speed every frame
    public float forwardSpeed = 0f;
    public float maxForwardSpeed = 50;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Movement is running");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Keyboard.current.rightArrowKey.isPressed && speed > -maxSpeed) //13 is not pulled from anything specific, if a different value feels better then change it
        {
            speed-=accelerationRate;
        }

        else if (Keyboard.current.leftArrowKey.isPressed && speed < maxSpeed)
        {
            speed+=accelerationRate;
        }

        speed*=(float)friction; //slows down character hor.
        if (speed<0.01&&speed>-0.01) speed = 0; //prevents speed from being a nasty tiny number

        // forwards and backwards movement
        if (Keyboard.current.upArrowKey.isPressed && forwardSpeed < maxForwardSpeed)
        {
            forwardSpeed += accelerationRate;
        }
        else if (Keyboard.current.downArrowKey.isPressed && forwardSpeed > -maxForwardSpeed)
        {
            forwardSpeed -= accelerationRate;
        }

        forwardSpeed *= (float)friction;//slows down character fwd/bwd
        if (forwardSpeed < 0.01f && forwardSpeed > -0.01f) forwardSpeed = 0;

        Vector3 newPos = transform.position + new Vector3(forwardSpeed, 0f, speed) * Time.deltaTime;

        if (newPos.z < -11f) { newPos.z = -11f; speed = 0f; }
        if (newPos.z > 13f) { newPos.z = 13f; speed = 0f; }
        if (newPos.x < -24f) { newPos.x = -24f; forwardSpeed = 0f; }
        if (newPos.x > -19f) { newPos.x = -19f; forwardSpeed = 0f; }

        transform.position = newPos;
    }
}

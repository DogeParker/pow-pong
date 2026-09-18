using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Assemblies;
using NUnit.Framework.Constraints;
using UnityEngine.WSA;
using UnityEditor.ShaderGraph.Internal;

public class BallMovement : MonoBehaviour
{
    //Notes: yes I know that there are far too many variables, as a matter of fact, several basically do nothing,
    // however I was trying to tune in the bouncing and got carried away trying to modify every value in every vector
    // gl shirav <3
    private bool isLaunched = false; //false if ball is ready to be served
    public float launchSpeed = 30f; //velocity on launch
    public float constantSpeed = 10f; //constant velocity with bounce on table
    public float downwardAngle = 30f; 
    public float launchHeight = 30f;
    public float airResistance = 15f;
    public string groundTag = "Ground";
    public static float bounceMult = 0.015f;
    public Vector3 bounceVector = new Vector3(0f, bounceMult, 0f);
    public float bounceCooldown = 0.1f;
    private float lastBounceTime = -999f;
    public string paddleTag = "Paddle";
    public float paddleBounceForce = 5f;
    public float paddleHalfWidth = 0.7175f; 
    public Transform leftTarget;
    public Transform midTarget;
    public Transform rightTarget;
    public float netClearHeight = 3f; 
    public float launchArc = 5f; 

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
    }

    // Update is called once per frame
    public float gravityMultiplier = 3f;
    Vector3 CalculateArcVelocity(Transform target)
    {
        Vector3 flatDelta = target.position - transform.position;
        flatDelta.y = 0f;
        float distance = flatDelta.magnitude;

        float g = Physics.gravity.magnitude * gravityMultiplier;
        float vy = Mathf.Sqrt(2f * g * netClearHeight);
        float timeUp = vy / g;
        float totalTime = timeUp * 2f;
        float horizontalSpeed = distance / totalTime;

        return flatDelta.normalized * horizontalSpeed + Vector3.up * vy;
    }
    void Update()
    {
        if (!isLaunched && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            LaunchBall();
            isLaunched = true;
        }
    }
    void FixedUpdate()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
            Vector3 vel = rb.linearVelocity;

            Vector3 horizontal = new Vector3(vel.x, 0f, vel.z).normalized * constantSpeed;
            float dampedY = vel.y * (1f - airResistance * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector3(horizontal.x, dampedY, horizontal.z);
        }

    void LaunchBall()
    {
        transform.SetParent(null);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Transform[] targets = { leftTarget, midTarget, rightTarget };
        Transform chosen = targets[UnityEngine.Random.Range(0, targets.Length)];

        rb.linearVelocity = CalculateArcVelocity(chosen);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag) && Time.time - lastBounceTime > bounceCooldown)
        {
            lastBounceTime = Time.time;
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 vel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(vel.x, 0f, vel.z) + bounceVector;
        }
        else if (collision.gameObject.CompareTag(paddleTag))
        {
            Transform paddle = collision.transform;
            float hitX = paddle.InverseTransformPoint(collision.contacts[0].point).x;
            float zoneT = hitX / paddleHalfWidth;
            Debug.Log($"hitX = {hitX}, zoneT = {zoneT}");

            Transform target = zoneT < -0.33f ? leftTarget
                            : zoneT > 0.33f ? rightTarget
                            : midTarget;

            Vector3 aimPoint = target.position + Vector3.up * netClearHeight;
            Vector3 toTarget = (aimPoint - transform.position).normalized;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.linearVelocity = CalculateArcVelocity(target);
            Debug.Log($"Paddle hit. Target = {target}");
        }
    }

}   

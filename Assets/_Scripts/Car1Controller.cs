using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static GameStat;

public class Car1Controller : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    public float desiredVelocityKmph;
    private float desiredVelocityMps;

    public bool isToCrash;

    private Rigidbody rb;

    // 查找相应的可视车轮
    // 正确应用变换
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void InitWheelColliders()
    {
        foreach (var axleInfo in axleInfos)
        {
            initWheelCollider(axleInfo.leftWheel);
            initWheelCollider(axleInfo.rightWheel);
        };
    }

    private void initWheelCollider(WheelCollider wheelCollider)
    {
        wheelCollider.radius = 0.3f;
        wheelCollider.center = new Vector3(0, 0.1f, 0);
        var ss = wheelCollider.suspensionSpring;
        ss.damper = 1500;
        wheelCollider.suspensionSpring = ss;
    }

    private void InitCollidingCar()
    {
        transform.position = CRASH_SITE + desiredVelocityMps * (TTC_AT_START + TTC_BIAS) * Vector3.right;
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    private void InitNotCollidingCar()
    {
        transform.position = CRASH_SITE + desiredVelocityMps * (TTC_AT_START + TTC_BIAS) * 2 * Vector3.left;
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void Start()
    {
        desiredVelocityMps = desiredVelocityKmph * 1000 / 3600;

        if (isToCrash)
        {
            InitCollidingCar();
        }
        else
        {
            InitNotCollidingCar();
        }

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * desiredVelocityMps;

        InitWheelColliders();
    }
    public void FixedUpdate()
    {
        //float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float motor;
        //float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        var currentVelocity = rb.velocity.magnitude;
        var gap = currentVelocity - desiredVelocityMps;

        if (gap < -1)
        {
            motor = 600;
        }
        else if (gap < -0.1)
        {
            motor = 200;
        }
        else if (gap < -0.01)
        {
            motor = 20;
        }
        else
        {
            motor = 0;
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {

            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

}



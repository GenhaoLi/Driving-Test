using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static GameStat;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class PlayerController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    public float desiredVelocityKmph;
    private float desiredVelocityMps;

    private Rigidbody rb;

    private void initWheelColliders()
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

    private void Start()
    {
        desiredVelocityMps = desiredVelocityKmph * 1000 / 3600;
        
        transform.position = CRASH_SITE + desiredVelocityMps * (TTC_AT_START + TTC_BIAS) * Vector3.back;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * desiredVelocityMps;

        initWheelColliders();
    }

    private void FixedUpdate()
    {
        //float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float motor;
        //float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        var currentVelocity = rb.velocity.magnitude;
        var gap = currentVelocity - desiredVelocityMps;

        if (gap < -9)
        {
            motor = 900;
        }
        else if (gap < -1)
        {
            motor = 400;
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
        //print($"gap: {gap} m/s, motor: {motor}, rpm: {axleInfos[0].leftWheel.rpm}");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            //if (axleInfo.steering)
            //{
            //    axleInfo.leftWheel.steerAngle = steering;
            //    axleInfo.rightWheel.steerAngle = steering;
            //}
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        //print(axleInfos[0].leftWheel.motorTorque);

        //setDampingRate(currentDR - gap * 10);
        //print(currentVelocity.ToString());
        //print(currentDR.ToString());
    }

    public void OnCollisionEnter(Collision collision)
    {
        print($"time elapsed at collison: {Time.time}");
        foreach (var contact in collision.contacts)
        {
            print($"collision position: {contact.point}");
        }

        ShutdownAllMotors();
    }

    private void ShutdownAllMotors()
    {
        // TODO MAYBE
    }
}



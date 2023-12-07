using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Resource used: https://resocoder.com/2018/07/20/shake-detecion-for-mobile-devices-in-unity-android-ios/

public class PhysicsController : MonoBehaviour
{

    public float ShakeForceMultiplier;
    public Rigidbody2D[] ShakingRigidbodies;

    public void ShakeRigidbodies(Vector3 deviceAcceleration)
    {

        //foreach (var rigidbody in ShakingRigidbodies)
        //{
            //rigidbody.AddForce(deviceAcceleration * ShakeForceMultiplier, ForceMode2D.Impulse);
        //}
    }
}
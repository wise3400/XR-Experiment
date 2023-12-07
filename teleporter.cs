using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


//Resources Used: ChatGPT, my older brother, and Peter!!!

[RequireComponent(typeof(PhysicsController))]

public class Teleporter : MonoBehaviour
{
   
    
    public GameObject blueCube;    // Far location
    public GameObject pinkCube;    // Closer location
    public GameObject whiteCube;   // Default position
    public GameObject[] randomCubes; // Array of random locations
    public TMP_Text textFeedback; //Displaying feedback

    public Camera mainCamera; 
    public Color backgroundColorChange;
    public LayerMask tpLayer;



    private float lastPressTime; 
    private int pressCount;
    private float pressDuration = 0;

    private float doubleClickTimer = 0;
    private float doubleClickThreshold = 0.3f;
    public int clickNum = 0;

    //new - for shake https://resocoder.com/2018/07/20/shake-detecion-for-mobile-devices-in-unity-android-ios/

    public float ShakeDetectionThreshold;
    public float MinShakeInterval;

    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;

    private PhysicsController physicsController;

    //new - for shake https://resocoder.com/2018/07/20/shake-detecion-for-mobile-devices-in-unity-android-ios/
    private void Start()
    {
        sqrShakeDetectionThreshold = Mathf.Pow(ShakeDetectionThreshold, 2);
        physicsController = GetComponent<PhysicsController>();

        
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check for Google Cardboard button press
        if (Input.GetMouseButtonDown(0))
        {
            pressDuration = 0;
            doubleClickTimer = 0;
            //CardboardButtonPress();
        }
        if(Input.GetMouseButton(0))
        {
            pressDuration += Time.deltaTime;
        }

        // needs to increase every frame
        doubleClickTimer += Time.deltaTime;

        if (Input.GetMouseButtonUp(0))
        {
            clickNum += 1;
        }

        if (doubleClickTimer > doubleClickThreshold && clickNum > 0)
        {
            CardboardButtonPress();
            clickNum = 0;
        }

        //new - for shake https://resocoder.com/2018/07/20/shake-detecion-for-mobile-devices-in-unity-android-ios/
        if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold
            && Time.unscaledTime >= timeSinceLastShake + MinShakeInterval)
        {
            Debug.Log(Input.acceleration.sqrMagnitude);
            //physicsController.ShakeRigidbodies(Input.acceleration);

            //Teleport(whiteCube);
            TeleportToRandomLocation();
            timeSinceLastShake = Time.unscaledTime;
        }
    }

    void DoRaycastTeleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, tpLayer))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }
    }

   
    void CardboardButtonPress()
    {
        // shake for 2 secs - Teleport to a random cube. Either blue, white, or pink.
        
        if (Input.acceleration.magnitude > 2.0f)
        {
            TeleportToRandomLocation();
            DoRaycastTeleport();
        }

        // double presses - Teleport to the default position. Will go to the white cube.
        else if (clickNum == 2)
        {
            Teleport(whiteCube);
            ChangeBackgroundColor();
            DisplayFeedback("(2P) White");
            //DoRaycastTeleport();
        }
        // Long press - Teleport to a far location. Will go to the blue cube.
        else if(pressDuration > 1.0f)
        {
            Teleport(blueCube);

            DisplayFeedback("(LP) Blue");
            //DoRaycastTeleport();
        }
        // Short press - Teleport to a closer location. Will go to the pink cube.
        else if (pressDuration <= 1.0f)
        {
            Teleport(pinkCube);

            DisplayFeedback("(SP) Pink");
            //DoRaycastTeleport();
        }

        // Update the last press time for future calculations
        lastPressTime = Time.time;
    }


    bool DoublePress()
    {
        // Check if the time between consecutive presses is less than or equal to 0.5 seconds
        if (Time.time - lastPressTime <= 0.5f)
        {
            pressCount++;
            // Check if the press count is 2 (double press)
            if (pressCount == 2)
            {
                pressCount = 0; // Reset press count
                return true;    // Double press detected
            }
        }
        else
        {
            pressCount = 1; // Reset press count to 1 if there is a gap between presses
        }

        return false; // Double press not detected
    }


    bool TriplePress()
    {
        // Check if the time between consecutive presses is less than 0.5 seconds
        if (Time.time - lastPressTime < 0.5f)
        {
           
            pressCount++;
            // Check if the press count is 3 (triple press)
            if (pressCount == 3)
            {
                pressCount = 0; // Reset press count
                return true;    // Triple press detected
            }
        }
        else
        {
            pressCount = 1; // Reset press count to 1 if there is a gap between presses
        }

        return false; // Triple press not detected
    }

    // teleport to a specific location
    void Teleport(GameObject targetLocation)
    {
        transform.position = targetLocation.transform.position;
    }

    // teleport to a random location
    void TeleportToRandomLocation()
    {
        if (randomCubes.Length > 0)
        {
            int randomCu = Random.Range(0, randomCubes.Length);
            
            transform.position = randomCubes[randomCu].transform.position;
            DisplayFeedback("R:" + randomCubes[randomCu].transform.name);
        }
    }

    void ChangeBackgroundColor()
    {
        if (mainCamera != null)
        {
            //random color change 
            backgroundColorChange = new Color(Random.value, Random.value, Random.value);
            mainCamera.backgroundColor = backgroundColorChange;
        }
    }

    void DisplayFeedback(string something)
    {
        textFeedback.text = something;
    }
   
}
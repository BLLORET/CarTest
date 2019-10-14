using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public enum Controllor { Keyboard, WirelessController, SteeringWheel };

    [SerializeField]
    private Controllor controllor = Controllor.Keyboard;

    [SerializeField]
    private Text txtSpeed = null;

    [SerializeField]
    private GameObject backLights = null;

    [Header("Wheels")]
    [SerializeField] private WheelCollider frontLeft = null;
    [SerializeField] private WheelCollider frontRight = null;
    [SerializeField] private WheelCollider backLeft = null;
    [SerializeField] private WheelCollider backRight = null;

    // WheelsMesh
    [SerializeField] private Transform transformFL = null;
    [SerializeField] private Transform transformFR = null;
    [SerializeField] private Transform transformBL = null;
    [SerializeField] private Transform transformBR = null;

    [Header("Controls")]
    [SerializeField] private float torque = 0f;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float maxSpeed = 200f;
    [SerializeField] private int brake = 10000;
    [SerializeField] private float coeffAcceleration = 10f;
    [SerializeField] private float wheelAngleMax = 10f;



    public float forward = 1f;

    private bool isBraking = false;

    private bool leftLight = false;
    private bool rightLight = false;
    private bool warning = false;

    private AudioSource audioSource;

    //Properties
    public Controllor Controller { get { return controllor; } }

    public bool LeftLightActivated { get { return leftLight;  } }

    public bool RightLightActivated { get { return rightLight; } }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Fix the center of mass
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, -0.9f, 0.2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Around
        EngineSound();
        DisplaySpeed();
        TurnLight();
        WheelRotation();

        // Input
        GetIndicatorsInput();
        InverseForward();
        Acceleration();
        Deceleration();
        Direction();
        Braking();
    }

    private void Acceleration()
    {
        /*if (Input.GetKey(KeyCode.UpArrow) && speed < maxSpeed && !isBraking)
        {
            
        }*/
        if (speed < maxSpeed && !isBraking)
        {

            if (controllor == Controllor.Keyboard && Input.GetKey(KeyCode.UpArrow) )
            {
                frontRight.brakeTorque = 0;
                frontLeft.brakeTorque = 0;
                backLeft.brakeTorque = 0;
                backRight.brakeTorque = 0;
                float vertical = Input.GetAxis("Vertical");
                Debug.Log("Up: " + vertical);
                backLeft.motorTorque = vertical * torque * coeffAcceleration * Time.deltaTime * forward;
                backRight.motorTorque = vertical * torque * coeffAcceleration * Time.deltaTime * forward;
            }
            else if (controllor == Controllor.WirelessController)
            {
                float WirelessControllerRT = Input.GetAxisRaw("RT");
                Debug.Log("RT is presed: " + WirelessControllerRT);
                if (WirelessControllerRT != 0 && speed < maxSpeed && !isBraking)
                {

                    Debug.Log(forward);
                    frontRight.brakeTorque = 0;
                    frontLeft.brakeTorque = 0;
                    backLeft.brakeTorque = 0;
                    backRight.brakeTorque = 0;
                    backLeft.motorTorque = WirelessControllerRT * torque * coeffAcceleration * Time.deltaTime * forward;
                    backRight.motorTorque = WirelessControllerRT * torque * coeffAcceleration * Time.deltaTime * forward;
                }
            }
            else if (controllor == Controllor.SteeringWheel)
            {
                float pedal = Input.GetAxis("SteeringWheelPedal");
                if (pedal < 0 && speed < maxSpeed && !isBraking)
                {
                    pedal *= -1;
                    frontRight.brakeTorque = 0;
                    frontLeft.brakeTorque = 0;
                    backLeft.brakeTorque = 0;
                    backRight.brakeTorque = 0;
                    backLeft.motorTorque = pedal * torque * coeffAcceleration * Time.deltaTime * forward;
                    backRight.motorTorque = pedal * torque * coeffAcceleration * Time.deltaTime * forward;
                }
            }
        }
        
    }

    private void Deceleration()
    {
        //float WirelessControllerLT = Input.GetAxis("LT");
        // Debug.Log("LT: " + WirelessControllerLT);
        //if ( && !isBraking || speed > maxSpeed)
        if ((controllor == Controllor.WirelessController && Input.GetAxis("RT") == 0)
            || (controllor == Controllor.Keyboard && !Input.GetKey(KeyCode.UpArrow)
            || (controllor == Controllor.SteeringWheel && Input.GetAxis("SteeringWheelPedal") == 0))
            && !isBraking || speed > maxSpeed)
        {
            /*backLeft.motorTorque = 0;
            backRight.motorTorque = 0;
            backLeft.brakeTorque = brake * coeffAcceleration * Time.deltaTime;
            backRight.brakeTorque = brake * coeffAcceleration * Time.deltaTime;*/
            if (GetComponent<Rigidbody>().velocity.y > 0)
            {
                backLeft.motorTorque = -1000;
                backRight.motorTorque = -1000;
            }
            else
            {
                backLeft.brakeTorque = 5000;
                backRight.brakeTorque = 5000;
            }
        }
    }

    private void Braking()
    {

        if (isBraking = (controllor == Controllor.Keyboard && Input.GetKey(KeyCode.DownArrow))
            || (controllor == Controllor.WirelessController && Input.GetAxis("RT") < 0)
            || (controllor == Controllor.SteeringWheel && Input.GetAxis("SteeringWheelPedal") > 0))
        {
            backLeft.brakeTorque = brake * coeffAcceleration * Time.deltaTime * 10;
            backRight.brakeTorque = brake * coeffAcceleration * Time.deltaTime * 10;
            frontRight.brakeTorque = brake * coeffAcceleration * Time.deltaTime * 10;
            frontLeft.brakeTorque = brake * coeffAcceleration * Time.deltaTime * 10;
            backRight.motorTorque = 0;
            backLeft.motorTorque = 0;
        }
    }

    private void Direction()
    {
        float axis = 0;
        switch (controllor)
        {
            case Controllor.Keyboard:
                axis = Input.GetAxis("Horizontal");
                break;

            case Controllor.WirelessController:
                axis = Input.GetAxis("Horizontal");
                break;
            case Controllor.SteeringWheel:
                axis = Input.GetAxis("Horizontal");
                Debug.Log("axis: " + axis);
                break;
            default:
                Debug.LogError("Enum not hold.");
                break;
        }

        // Wheel Collider
        frontLeft.steerAngle = axis * wheelAngleMax;
        frontRight.steerAngle = axis * wheelAngleMax;

        // Mesh

        transformFL.localEulerAngles = new Vector3(transformFL.localEulerAngles.x, frontLeft.steerAngle - transformFL.localEulerAngles.z, transformFL.localEulerAngles.z);
        transformFR.localEulerAngles = new Vector3(transformFR.localEulerAngles.x, frontRight.steerAngle - transformFR.localEulerAngles.z, transformFR.localEulerAngles.z);
    }

    private void WheelRotation()
    {
        transformFL.Rotate(frontLeft.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        transformFR.Rotate(frontRight.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        transformBL.Rotate(backLeft.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        transformBR.Rotate(backRight.rpm / 60 * 360 * Time.deltaTime, 0, 0);
    }

    private void DisplaySpeed()
    {
        // 3.6 to convert m/s to km/h.
        speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        txtSpeed.text = (int) speed + " KM/H";
    }

    /// <summary>
    /// Set the backLight On or off.
    /// </summary>
    private void TurnLight()
    {
        backLights.SetActive(isBraking || (forward < 0 && speed > 0.1));
    }

    /// <summary>
    /// Fix the sound of the engine.
    /// </summary>
    private void EngineSound()
    {
        audioSource.pitch = speed / maxSpeed + 1f;
    }

    /// <summary>
    /// Get if the player choose to inverse the sens of direction of the car.
    /// </summary>
    private void InverseForward()
    {
        if (((controllor == Controllor.Keyboard && Input.GetKeyDown(KeyCode.Space))
            || (controllor == Controllor.WirelessController && Input.GetButtonDown("XBoxY"))
            || controllor == Controllor.SteeringWheel && Input.GetButtonDown("SteeringWheelTriangle"))
            && speed < 0.1)
        {
            forward *= -1f;
        }
    }

    /// <summary>
    /// Get input about indicators.
    /// This options are hold:
    ///     - left indicator
    ///     - right indicator
    ///     - warnings.
    ///     
    /// This function set the boolean that indicate if the light is enabled.
    /// </summary>
    private void GetIndicatorsInput()
    {
        // Enabled or disabled indicators.
        if ((controllor == Controllor.Keyboard && Input.GetKeyDown(KeyCode.E))
            || (controllor == Controllor.WirelessController && Input.GetButtonDown("XBoxLB"))
            || (controllor == Controllor.SteeringWheel && Input.GetButtonDown("SteeringWheelL2")))
        {
            // Disable right light if it is enable.
            if (rightLight)
            {
                rightLight = false;
            }
            leftLight = !leftLight;
        }
        // Enabled or disabled indicators.
        else if (controllor == Controllor.Keyboard && Input.GetKeyDown(KeyCode.R)
            || (controllor == Controllor.WirelessController && Input.GetButtonDown("XBoxRB"))
            || (controllor == Controllor.SteeringWheel && Input.GetButtonDown("SteeringWheelR2")))
        {
            // Disable left light if it is enable.
            if (leftLight)
            {
                leftLight = false;
            }
            rightLight = !rightLight;
        }
        // Enabled or disabled warnings
        else if (controllor == Controllor.Keyboard && Input.GetKeyDown(KeyCode.T)
            || (controllor == Controllor.WirelessController && Input.GetButtonDown("XBoxB"))
            || (controllor == Controllor.SteeringWheel && Input.GetButtonDown("SteeringWheelPS")))
        {
            warning = !warning;
            // Try to disabled indicators before warnings.
            rightLight = false;
            leftLight = false;
            leftLight = warning;
            rightLight = warning;
        }
    }
}

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

    private AudioSource audioSource;

    //Properties
    public Controllor Controller { get { return controllor; } }

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
        MoteurSound();
        DisplaySpeed();
        TurnLight();
        WheelRotation();

        // Input
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
        frontLeft.steerAngle = Input.GetAxis("Horizontal") * wheelAngleMax;
        frontRight.steerAngle = Input.GetAxis("Horizontal") * wheelAngleMax;

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

    private void TurnLight()
    {
        if (isBraking || (forward < 0 && speed > 0.1))
        {
            backLights.SetActive(true);
        }
        else
        {
            backLights.SetActive(false);
        }
    }

    private void MoteurSound()
    {
        audioSource.pitch = speed / maxSpeed + 1f;
    }

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] private Camera fisrtPerson;
    [SerializeField] private Camera thirdPerson;

    [SerializeField] private GameObject car;

    private CarController carController;


    private void Start()
    {
        carController = car.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Switch();
        //LookRight();
    }
    
    private void Switch()
    {
        if ((carController.Controller == CarController.Controllor.Keyboard && Input.GetKeyDown(KeyCode.Tab))
            || (carController.Controller == CarController.Controllor.WirelessController && Input.GetButtonDown("XBoxX"))
            || (carController.Controller == CarController.Controllor.SteeringWheel && Input.GetButtonDown("SteeringWheelSquare")))
        {
            if (fisrtPerson.gameObject.activeInHierarchy)
            {
                thirdPerson.gameObject.SetActive(true);
                fisrtPerson.gameObject.SetActive(false);
            }
            else
            {
                thirdPerson.gameObject.SetActive(false);
                fisrtPerson.gameObject.SetActive(true);
            }
        }
    }

    private Camera GetActiveCamera()
    {
        if (fisrtPerson.gameObject.activeInHierarchy)
        {
            return fisrtPerson;
        }
        return thirdPerson;
    }

    /*private void LookRight()
    {
        if (Input.GetButtonDown("SteeringWheelRightRetro"))
        {
            Transform camTransform = GetActiveCamera().transform;
            camTransform.rotation = Quaternion.LookRotation(camTransform.position, Vector3.left);
        }
    }*/
}

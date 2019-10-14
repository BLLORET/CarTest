using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicators : MonoBehaviour
{
    [SerializeField] private GameObject car = null;
    [SerializeField] private GameObject leftLightActivator = null;
    [SerializeField] private GameObject rightLightActivator = null;


    private CarController carController;
    


    // Start is called before the first frame update
    void Start()
    {
        carController = car.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        EnableLeftIndicator();
        EnableRightIndicator();
    }

    private void EnableLeftIndicator()
    {
        if (carController.LeftLightActivated)
        {
            leftLightActivator.SetActive(true);
        }
        else
        {
            leftLightActivator.SetActive(false);
        }
    }

    private void EnableRightIndicator()
    {
        if (carController.RightLightActivated)
        {
            rightLightActivator.SetActive(true);
        }
        else
        {
            rightLightActivator.SetActive(false);
        }
    }
}

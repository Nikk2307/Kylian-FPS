using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float fuel = 100;

    [SerializeField]
    Text fuelText;

    [SerializeField]
    float fuelRechargeSpeed = 1f;

    void Update()
    {
        if(fuel >= 100)
        {
            fuel = 100;
        }
        else if(fuel < 100)
        {
            fuel += fuelRechargeSpeed * Time.deltaTime;
        }

        fuelText.text = "Fuel = " + (int)fuel;
    }
}

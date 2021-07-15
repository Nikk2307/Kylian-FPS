using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    #region Variables

    [Header("Look Properties")]

    [SerializeField]
    float mouseXSensitivity;

    [SerializeField]
    float mouseYSensitivity;

    float mouseX;
    float mouseY;
    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    [SerializeField]
    float normalFOV;

    [SerializeField]
    float wallRunFOV;

    [SerializeField]
    float fovTransitionTime;

    [SerializeField]
    float cameraTilt;

    float tilt;

    [SerializeField]
    float cameraTiltTransitionTime;

    [Header("References")]

    [SerializeField]
    Transform fpsCameraRoot;

    [SerializeField]
    Camera fpsCamera;

    [SerializeField]
    Transform orientation;

    #endregion

    private void OnEnable()
    {
        PlayerController.onWallRun += WallRunOn;

        PlayerController.onNotWallRun += WallRunOff;
    }

    private void OnDisable ()
    {
        PlayerController.onWallRun -= WallRunOn;

        PlayerController.onNotWallRun -= WallRunOff;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MouseInput();

        fpsCameraRoot.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void MouseInput()
    {
        yRotation += InputManager.mouseX * mouseXSensitivity * multiplier;
        xRotation -= InputManager.mouseY * mouseYSensitivity * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    void WallRunOn()
    {
        if (PlayerController.wallOnLeft)
        {
            tilt = Mathf.Lerp(tilt, -cameraTilt, cameraTiltTransitionTime * Time.deltaTime);
        }
        else if (PlayerController.wallOnRight)
        {
            tilt = Mathf.Lerp(tilt, cameraTilt, cameraTiltTransitionTime * Time.deltaTime);
        }

        fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, wallRunFOV, fovTransitionTime * Time.deltaTime);
    }

    void WallRunOff()
    {
        fpsCamera.fieldOfView = Mathf.Lerp(fpsCamera.fieldOfView, normalFOV, fovTransitionTime * Time.deltaTime);

        tilt = Mathf.Lerp(tilt, 0, cameraTiltTransitionTime * Time.deltaTime);
    }
}

using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Variables

    public static float horizontalMove;

    public static float verticalMove;

    public static float mouseX;

    public static float mouseY;

    [SerializeField]
    KeyCode jumpKey = KeyCode.Space;

    [SerializeField]
    KeyCode wallJumpKey = KeyCode.Space;

    #endregion

    public bool OnNormalJumpPressed()
    {
        return Input.GetKeyDown(jumpKey);
    }

    public bool OnWallJumpPressed()
    {
        return Input.GetKeyDown(wallJumpKey);
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");

        verticalMove = Input.GetAxisRaw("Vertical");

        mouseX = Input.GetAxisRaw("Mouse X");

        mouseY = Input.GetAxisRaw("Mouse Y");
    }
}

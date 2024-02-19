using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        // Lock cursor in the middle and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(GameManager.isGameOver || GameManager.isPaused) {  return; }

        // Get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        yRotation += mouseX;

        xRotation -= mouseY;

        // Fix rotation between -90 and 90 so the camera won't get upside down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate cam
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        // Rotate orientation so the player turns towards the look direction
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}

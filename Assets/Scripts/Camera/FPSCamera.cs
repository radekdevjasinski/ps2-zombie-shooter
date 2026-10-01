using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    private const float MaxPitch = 15f;

    public float sens;
    public Transform orientation;
    public bool lockMovement = false;

    private float pitch;
    private float yaw;

    void Awake()
    {
        sens = GameSettings.LoadMouseSensitivity(sens);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (lockMovement || PauseMenu.IsGamePaused)
        {
            return;
        }

        yaw += Input.GetAxisRaw("Mouse X") * sens;
        pitch -= Input.GetAxisRaw("Mouse Y") * sens;
        pitch = Mathf.Clamp(pitch, -MaxPitch, MaxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        orientation.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    public void SetSensitivity(float sensitivity)
    {
        sens = sensitivity;
        GameSettings.SaveMouseSensitivity(sensitivity);
    }
}

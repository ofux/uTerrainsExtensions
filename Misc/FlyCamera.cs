using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float LookSpeed = 5f;
    public float MoveSpeed = 15f;

    float rotationX;
    float rotationY;

    // Update is called once per frame
    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * LookSpeed * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * LookSpeed * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);
        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up) * Quaternion.AngleAxis(rotationY, Vector3.left);
        transform.position += (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * MoveSpeed * Time.deltaTime;
    }
}
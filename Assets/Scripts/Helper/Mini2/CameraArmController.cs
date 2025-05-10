using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArmController : MonoBehaviour
{
    [SerializeField] private float verticalClamp = 30f;
    [SerializeField] private Vector2 sensitivity = Vector2.one;

    [SerializeField] private TouchInputMini2 touchInputManager;
    private void FixedUpdate()
    {
        Vector2 movementInput = touchInputManager.movementAmount;
        movementInput *= sensitivity;
        transform.localRotation = Quaternion.Euler(new Vector3(movementInput.y, movementInput.x * -1f, 0));

        float clamped_x = 0;

        if(transform.localRotation.eulerAngles.x < 180)
        {
            clamped_x = Mathf.Clamp(transform.localRotation.eulerAngles.x, -verticalClamp, verticalClamp);
        }
        else
        {
            clamped_x = Mathf.Clamp(transform.localRotation.eulerAngles.x, 360f-verticalClamp, 360f+verticalClamp);
        }

        transform.localRotation = Quaternion.Euler(
            new Vector3(
                clamped_x,
                transform.localRotation.eulerAngles.y,
                0
            )
        );
    }
}

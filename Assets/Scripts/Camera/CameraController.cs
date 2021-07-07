using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Vector2 previousInput;

    private InputControls inputControls;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);
        playerCameraTransform.position = new Vector3(0f, 10f, 0f);

        inputControls = new InputControls();

        inputControls.Player.CameraMovement.performed += SetPreviousInput;
        inputControls.Player.CameraMovement.canceled += SetPreviousInput;

        inputControls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        //Get the mouse position.
        if (previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            //Check if the mouse is near the edges of the screen. If so twick the value of cursorMovement accordingly.
            if (cursorPosition.y >= Screen.height - screenBorderThickness)
                cursorMovement.z += 1;

            else if (cursorPosition.y <= screenBorderThickness)
                cursorMovement.z -= 1;

            if (cursorPosition.x >= Screen.width - screenBorderThickness)
                cursorMovement.x += 1;

            else if (cursorPosition.x <= screenBorderThickness)
                cursorMovement.x -= 1;

            // Move camera to the new position.
            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        // Check if we also getting the arrow keys and WASD keys.
        else
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;

        //Limit the moving so we dont go out of the screen limits.
        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}

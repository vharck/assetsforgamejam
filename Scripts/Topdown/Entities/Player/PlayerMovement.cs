/*

TODO:
-> comentar o código
-> add coisa extra, tipo dash etc

 */
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerMovement : Movement
{
    #region Movement

    private Vector2 input;

    public void OnMoveCtrl(InputAction.CallbackContext context) => input = context.ReadValue<Vector2>();

    #endregion

    #region Dash

    [SerializeField] private float dashDuration = 10;
    [SerializeField] private float dashVelocity = 10;
    [SerializeField] private float dashCooldown = 1;

    private bool dashEnable = true;
    private float dashTimer = 0;
    private float dashCooldownTimer = 0;
    private Vector2 dashInput = Vector2.zero;

    public void EnableDash(bool value) => dashEnable = value;

    private bool CanDash => dashEnable && moveEnable && Time.time > dashCooldownTimer;
    public bool IsDashing => Time.time < dashTimer;

    public void OnDashButtom(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (!CanDash) return;

        dashInput = input;
        dashTimer = Time.time + dashDuration;
        dashCooldownTimer = Time.time + dashCooldown;
    }

    #endregion

    private void FixedUpdate()
    {
        if (IsDashing) rb.velocity = Time.deltaTime * dashVelocity * dashInput;

        else if (moveEnable) rb.velocity = Time.deltaTime * velocity * input;
    }
}
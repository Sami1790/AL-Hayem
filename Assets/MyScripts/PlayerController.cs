// ───────────────────────────────────────────────────────────────────────────────
//  PlayerController.cs 
// ───────────────────────────────────────────────────────────────────────────────
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /*════════════ Tunables ════════════*/
    [Header("Speeds")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 7.5f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 14f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 1.35f;
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float jumpBuffer = 0.15f;

    [Header("Stamina")]
    [SerializeField] float maxStamina = 6f;
    [SerializeField] float recoverRate = 1.8f;
    [SerializeField] float drainRate   = 1.2f;

    [Header("Camera & Head‑Bob")]
    [SerializeField] Transform cameraRoot;
    [SerializeField] float mouseSensX = 2.5f;
    [SerializeField] float mouseSensY = 2.5f;
    [SerializeField] Vector2 pitchClamp = new(-89, 89);
    [SerializeField] bool   headBob = true;
    [SerializeField] float  bobAmp  = 0.05f;
    [SerializeField] float  bobFreq = 9f;

    /*════════════ Private ════════════*/
    CharacterController cc;
    Vector3 moveInput, velocity;
    float   verticalVel, speed;
    bool    sprintHeld;

    float stamina;
    float lastGrounded, lastJumpCmd;

    float yaw, pitch;
    float baseCamY, bobT;

    /*════════════ Init ════════════*/
    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cameraRoot) cameraRoot = Camera.main.transform;
        baseCamY = cameraRoot.localPosition.y;
        stamina  = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        cc.stepOffset = 0f; 
    }

    void Start() => StartCoroutine(SnapToGround());

    IEnumerator SnapToGround()
    {
       
        cc.enabled = false;
        
        yield return new WaitForFixedUpdate();

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
        {
            float bottom = cc.height * 0.5f - cc.center.y + cc.skinWidth + 0.001f;
            transform.position = hit.point + Vector3.up * bottom;
        }
        cc.enabled   = true;
        verticalVel  = -2f; 
    }

    /*════════════ UPDATE ════════════*/
    void Update()
    {
        ReadInput();
        HandleLook();
        HandleMove();
        HandleJumpGravity();
        HandleStamina();
        if (headBob) HandleHeadBob();

        cc.Move((velocity + Vector3.up * verticalVel) * Time.deltaTime);
    }

    /*════════════ INPUT (Classic) */
    void ReadInput()
    {
        moveInput   = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        sprintHeld  = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetButtonDown("Jump")) lastJumpCmd = Time.time;
    }

    /*════════════ LOOK */
    void HandleLook()
    {
        yaw   += Input.GetAxis("Mouse X") * mouseSensX;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensY;
        pitch  = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.rotation       = Quaternion.Euler(0, yaw, 0);
    }

    /*════════════ MOVE */
    void HandleMove()
    {
        bool wantsSprint = sprintHeld && stamina > .1f && moveInput.sqrMagnitude > .01f;
        float targetSpeed = wantsSprint ? sprintSpeed : walkSpeed;
        Vector3 desired = (transform.right * moveInput.x + transform.forward * moveInput.z) * targetSpeed;
        float accel = (desired.magnitude > speed) ? acceleration : deceleration;
        velocity = Vector3.MoveTowards(velocity, desired, accel * Time.deltaTime);
        speed    = velocity.magnitude;
    }

    /*════════════ Jump & Gravity */
    void HandleJumpGravity()
    {
        bool grounded = cc.isGrounded;
        if (grounded) lastGrounded = Time.time;

        bool canJump = (Time.time - lastGrounded) <= coyoteTime && (Time.time - lastJumpCmd) <= jumpBuffer;
        if (canJump)
        {
            verticalVel   = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            lastGrounded  = lastJumpCmd = -999f;
        }
        verticalVel += (grounded && verticalVel < 0 ? -2f : Physics.gravity.y) * Time.deltaTime;
    }

    /*════════════ Stamina */
    void HandleStamina()
    {
        bool draining = sprintHeld && moveInput.sqrMagnitude > .01f;
        stamina += (draining ? -drainRate : recoverRate) * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    /*════════════ Head‑Bob */
    void HandleHeadBob()
    {
        if (cc.isGrounded && moveInput.sqrMagnitude > .01f)
        {
            bobT += Time.deltaTime * bobFreq * (sprintHeld ? 1.5f : 1f);
            float offset = Mathf.Sin(bobT) * bobAmp;
            Vector3 p = cameraRoot.localPosition; p.y = baseCamY + offset; cameraRoot.localPosition = p;
        }
        else
        {
            bobT = 0f;
            Vector3 p = cameraRoot.localPosition; p.y = Mathf.Lerp(p.y, baseCamY, Time.deltaTime * 6f); cameraRoot.localPosition = p;
        }
    }
}

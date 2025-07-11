using System.Collections;
using UnityEngine;

/// <summary>
/// تحكم منظور أول شخص: حركة، سبرنت، قفز ذكي، ستامينا، هيد-بوب.
/// إضافة ميزة التجميد: إيقاف الحركة والكاميرا عند تفعيل التولتيب أو البانل.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // إعدادات الحركة
    [Header("Speeds")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 7.5f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 14f;

    // إعدادات القفز
    [Header("Jump")]
    [SerializeField] float jumpHeight = 1.35f;
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float jumpBuffer = 0.15f;

    // ستامينا
    [Header("Stamina")]
    [SerializeField] float maxStamina = 6f;
    [SerializeField] float recoverRate = 1.8f;
    [SerializeField] float drainRate = 1.2f;

    // الكاميرا والهيد-بوب
    [Header("Camera & Head‑Bob")]
    [SerializeField] Transform cameraRoot;
    [SerializeField] float mouseSensX = 2.5f;
    [SerializeField] float mouseSensY = 2.5f;
    [SerializeField] Vector2 pitchClamp = new(-89, 89);
    [SerializeField] bool headBob = true;
    [SerializeField] float bobAmp = 0.05f;
    [SerializeField] float bobFreq = 9f;

    // متغيرات محلية
    CharacterController cc;
    Vector3 moveInput, velocity;
    float verticalVel, speed;
    bool sprintHeld;

    float stamina;
    float lastGrounded, lastJumpCmd;

    float yaw, pitch;
    float baseCamY, bobT;

    bool ready = false;

    // ========= NEW: لتجميد الحركة/الكاميرا =========
    [HideInInspector]
    public bool isFrozen = false; // عدلها من أي سكربت خارجي (زي التولتيب بانل)

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cameraRoot) cameraRoot = Camera.main.transform;
        baseCamY = cameraRoot.localPosition.y;
        stamina = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cc.stepOffset = 0f;

        lastJumpCmd = -999f;
        lastGrounded = Time.time - 10f;

        Vector3 startAngles = transform.eulerAngles;
        yaw = startAngles.y;
        pitch = cameraRoot.localEulerAngles.x;
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
        cc.enabled = true;
        verticalVel = -2f;
        yield return null;
        ready = true;
    }

    void Update()
    {
        if (!ready) return;
        // ========= إيقاف كل شيء إذا كان مجمّد =========
        if (isFrozen)
            return;

        ReadInput();
        HandleLook();
        HandleMove();
        HandleGravityJump();
        HandleStamina();
        if (headBob) HandleHeadBob();

        cc.Move((velocity + Vector3.up * verticalVel) * Time.deltaTime);
    }

    void ReadInput()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        sprintHeld = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetButtonDown("Jump"))
            lastJumpCmd = Time.time;
    }

    void HandleLook()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensX;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensY;
        pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

    void HandleMove()
    {
        bool wantsSprint = sprintHeld && stamina > .1f && moveInput.sqrMagnitude > .01f;
        float targetSpeed = wantsSprint ? sprintSpeed : walkSpeed;
        Vector3 desired = (transform.right * moveInput.x + transform.forward * moveInput.z) * targetSpeed;
        float accel = (desired.magnitude > speed) ? acceleration : deceleration;
        velocity = Vector3.MoveTowards(velocity, desired, accel * Time.deltaTime);
        speed = velocity.magnitude;
    }

    void HandleGravityJump()
    {
        if (cc.isGrounded)
        {
            lastGrounded = Time.time;
            if (verticalVel < 0f) verticalVel = -0.5f;
        }
        else
        {
            verticalVel += Physics.gravity.y * Time.deltaTime;
        }

        bool pressedJump = (Time.time - lastJumpCmd) <= jumpBuffer;
        bool canCoyote = (Time.time - lastGrounded) <= coyoteTime;

        if (lastJumpCmd > 0f && pressedJump && canCoyote)
        {
            verticalVel = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            lastJumpCmd = -999f;
        }
    }

    void HandleStamina()
    {
        bool draining = sprintHeld && moveInput.sqrMagnitude > .01f;
        stamina += (draining ? -drainRate : recoverRate) * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

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

    public void RefillStamina() => stamina = maxStamina;
    public float StaminaPercent() => stamina / maxStamina;
}

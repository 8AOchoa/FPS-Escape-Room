using UnityEngine;

public class FPSPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6.0f;
    public float jumpForce = 8.0f;
    public float gravityMultiplier = 2.5f;

    [Header("Look Settings")]
    public float mouseSensitivity = 3.0f;
    public float minLookAngle = -85f;
    public float maxLookAngle = 85f;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform headTransform;

    [Header("Weapon Settings")]
    public float pickupRange = 2.0f;
    public LayerMask gunLayer;
    public float shootRange = 100f;
    public AudioClip shootSound;
    public float damage = 10f; // Damage per shot

    // Private references
    private CharacterController controller;
    private Camera playerCamera;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private bool isGrounded;
    private GameObject equippedGun;
    private AudioSource audioSource;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (headTransform == null)
        {
            Debug.LogError("Head Transform not assigned!");
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2.0f;
            controller.radius = 0.5f;
            Debug.Log("CharacterController added automatically");
        }

        SyncCameraToHead();
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovementInput();
        HandleDirectMouseLook();
        HandleWeaponInput();
        ApplyMovement();
        SyncCameraToHead();
    }

    private void CheckGrounded()
    {
        isGrounded = controller.isGrounded;
        if (!isGrounded)
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - (controller.height / 2), transform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, groundCheckDistance, groundMask);
        }
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = Vector3.ProjectOnPlane(headTransform.forward, Vector3.up).normalized * vertical;
        Vector3 right = Vector3.ProjectOnPlane(headTransform.right, Vector3.up).normalized * horizontal;
        moveDirection = (forward + right).normalized * moveSpeed;

        if (isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            }
        }

        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
    }

    private void HandleDirectMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minLookAngle, maxLookAngle);
        cameraYaw += mouseX;

        headTransform.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
    }

    private void HandleWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (equippedGun == null)
                TryPickupGun();
            else
                DropGun();
        }

        if (equippedGun != null && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void TryPickupGun()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupRange, gunLayer))
        {
            GameObject gun = hit.collider.gameObject;
            EquipGun(gun);
        }
    }

    private void EquipGun(GameObject gun)
    {
        equippedGun = gun;
        Rigidbody rb = equippedGun.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        equippedGun.transform.SetParent(headTransform);
        equippedGun.transform.localPosition = new Vector3(0.2f, -0.1f, 0.3f);
        equippedGun.transform.localRotation = Quaternion.identity;
    }

    private void DropGun()
    {
        if (equippedGun == null) return;

        Rigidbody rb = equippedGun.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = velocity;
        }

        equippedGun.transform.SetParent(null);
        equippedGun = null;
    }

    private void Shoot()
    {
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        RaycastHit hit;
        Vector3 shootOrigin = playerCamera.transform.position;
        Vector3 shootDirection = playerCamera.transform.forward;

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Deal damage to target
            Target target = hit.collider.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // Hit effect
            GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitEffect.transform.position = hit.point;
            hitEffect.transform.localScale = Vector3.one * 0.1f;
            Destroy(hitEffect, 1f);
        }
    }

    private void SyncCameraToHead()
    {
        playerCamera.transform.position = headTransform.position;
        playerCamera.transform.rotation = headTransform.rotation;
    }

    private void ApplyMovement()
    {
        Vector3 moveVector = moveDirection;
        moveVector.y = velocity.y;
        controller.Move(moveVector * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (controller != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - (controller.height / 2), transform.position.z);
            Gizmos.DrawWireSphere(spherePosition, groundCheckDistance);
        }
    }
}
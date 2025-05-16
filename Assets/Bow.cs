using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform ArrowPoint;
    public Transform CameraPosition;
    public float minForce = 10f;
    public float maxForce = 50f;
    public float chargeTime = 2f;

    private float currentCharge = 0f;
    private bool isCharging = false;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Normal.Enable();
        controls.Normal.ShootBow.started += StartCharging;
        controls.Normal.ShootBow.canceled += ReleaseArrow;
    }

    private void OnDisable()
    {
        controls.Normal.ShootBow.started -= StartCharging;
        controls.Normal.ShootBow.canceled -= ReleaseArrow;
        controls.Normal.Disable();
    }

    private void Update()
    {
        if (isCharging)
        {
            currentCharge += Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, chargeTime);
        }
    }

    private void StartCharging(InputAction.CallbackContext context)
    {
        isCharging = true;
        currentCharge = 0f;
    }

    private void ReleaseArrow(InputAction.CallbackContext context) // Broken
    {
        if (!isCharging) return;

        float power = Mathf.Lerp(minForce, maxForce, currentCharge / chargeTime);
        GameObject arrow = Instantiate(arrowPrefab, ArrowPoint.position, ArrowPoint.rotation);
        arrow.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.velocity = CameraPosition.transform.forward * power;

        isCharging = false;
    }
}
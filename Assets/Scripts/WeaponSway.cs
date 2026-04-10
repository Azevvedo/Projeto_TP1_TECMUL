using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("ADS Settings")]
    public Vector3 adsPosition;
    public Vector3 adsRotationOffset; 
    public float adsSpeed = 12f;
    public float adsZoomFOV = 45f;

    [Header("Run Settings (Sideways)")]
    public Vector3 runPosition;    
    public Vector3 runRotationOffset; 
    
    [Header("Breathing & Walking")]
    public float idleAmount = 0.012f;     
    public float walkAmount = 0.04f;      
    public float smooth = 10f;
    private float timer;

    [Header("References")]
    public CanvasGroup crosshairCanvasGroup;
    public float fadeSpeed = 15f;
    private Camera cam;
    private Vector3 hipPosition;
    private Quaternion hipRotation; 
    private float defaultFOV;
    private bool estaMirando;
    
    private PlayerMovement playerMovement;
    private PlayerInputActions controls;

    public bool EstaMirando => estaMirando;

    void Awake()
    {
        controls = new PlayerInputActions();
        playerMovement = GetComponentInParent<PlayerMovement>();

        controls.Player.Mira.performed += ctx => {
            if (playerMovement != null && playerMovement.IsGrounded && !playerMovement.IsRunning)
                estaMirando = true;
        };
        controls.Player.Mira.canceled += ctx => estaMirando = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        cam = Camera.main;
        if (cam != null) defaultFOV = cam.fieldOfView;
        
        hipPosition = transform.localPosition;
        hipRotation = transform.localRotation; 
    }

    void Update()
    {
        bool isMoving = playerMovement != null && playerMovement.CurrentSpeed > 0.1f;
        bool isRunning = playerMovement != null && playerMovement.IsRunning;
        bool noAr = playerMovement != null && !playerMovement.IsGrounded;

        if (isRunning || noAr) estaMirando = false;

        float speed = isMoving ? (isRunning && !noAr ? 14f : 10f) : 2f;
        float amount = (isMoving && !noAr) ? (isRunning ? walkAmount * 1.5f : walkAmount) : idleAmount;
        if (estaMirando) amount *= 0.15f; 

        timer += Time.deltaTime * speed;
        Vector3 bobbingOffset = new Vector3(Mathf.Cos(timer) * amount, Mathf.Sin(timer * 2f) * amount, 0);

        Vector3 targetPos = hipPosition;
        Quaternion targetRot = hipRotation;

        if (estaMirando)
        {
            targetPos = adsPosition;
            targetRot = hipRotation * Quaternion.Euler(adsRotationOffset);
        }
        else if (isRunning && isMoving && !noAr)
        {
            targetPos = runPosition;
            targetRot = hipRotation * Quaternion.Euler(runRotationOffset);
        }
        else if (noAr)
        {
            targetPos = hipPosition + new Vector3(0, -0.05f, 0);
        }

        float curSmooth = (isRunning || estaMirando || noAr) ? adsSpeed : smooth;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos + bobbingOffset, Time.deltaTime * curSmooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * curSmooth);

        if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, estaMirando ? adsZoomFOV : defaultFOV, Time.deltaTime * adsSpeed);
        
        if (crosshairCanvasGroup != null)
        {
            float targetAlpha = (estaMirando || (isRunning && noAr == false)) ? 0f : 1f;
            crosshairCanvasGroup.alpha = Mathf.MoveTowards(crosshairCanvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }
}
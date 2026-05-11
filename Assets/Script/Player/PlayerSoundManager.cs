using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static PlayerSoundManager Instance;

    [System.Serializable]
    public class SurfaceFootsteps
    {
        public GroundType.SurfaceType surfaceType;
        public AudioClip[] footstepSounds;
    }

    [Header("발소리 목록")]
    public SurfaceFootsteps[] surfaceFootsteps;

    [Header("이동 타이밍")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    [Header("액션 사운드")]
    public AudioClip rollSound;
    public AudioClip jumpSound;

    private AudioSource audioSource;
    private PlayerMovement playerMovement;
    private float stepTimer = 0f;
    private GroundType.SurfaceType currentSurface = GroundType.SurfaceType.Grass;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        DetectGround();
        HandleFootsteps();
    }

    void DetectGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
        if (hit.collider == null) return;

        GroundType ground = hit.collider.GetComponent<GroundType>();
        if (ground != null)
            currentSurface = ground.surfaceType;
    }

    void HandleFootsteps()
    {
        bool isMoving = InputManager.Instance != null && InputManager.Instance.MoveInput.magnitude > 0;

        if (isMoving && !playerMovement.IsRolling())
        {
            bool isRunning = InputManager.Instance != null && InputManager.Instance.IsRunning;
            float interval = isRunning ? runStepInterval : walkStepInterval;

            stepTimer += Time.deltaTime;

            if (stepTimer >= interval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void PlayFootstep()
    {
        foreach (SurfaceFootsteps surface in surfaceFootsteps)
        {
            if (surface.surfaceType == currentSurface && surface.footstepSounds.Length > 0)
            {
                AudioClip clip = surface.footstepSounds[Random.Range(0, surface.footstepSounds.Length)];
                audioSource.PlayOneShot(clip);
                return;
            }
        }
    }

    public void PlayRoll()
    {
        if (rollSound != null)
            audioSource.PlayOneShot(rollSound);
    }
}

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
    
    [Header("지형별 발소리")]
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
    
    if (hit.collider != null)
    {
        Debug.Log($"Hit: {hit.collider.name}");
        
        GroundType ground = hit.collider.GetComponent<GroundType>();
        if (ground != null)
        {
            currentSurface = ground.surfaceType;
            Debug.Log($"Ground Type: {currentSurface}");
        }
        else
        {
            Debug.Log("GroundType 컴포넌트 없음!");
        }
    }
}
    
    void HandleFootsteps()
    {
        bool isMoving = Input.GetKey(KeyCode.LeftArrow) || 
                        Input.GetKey(KeyCode.RightArrow) || 
                        Input.GetKey(KeyCode.UpArrow) || 
                        Input.GetKey(KeyCode.DownArrow);
        
        if (isMoving && !playerMovement.IsRolling())
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
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
        Debug.Log($"PlayFootstep 호출! 현재 지형: {currentSurface}");

        foreach (SurfaceFootsteps surface in surfaceFootsteps)
        {
            if (surface.surfaceType == currentSurface && surface.footstepSounds.Length > 0)
            {
                Debug.Log($"발소리 재생! {surface.footstepSounds[0].name}");
                AudioClip clip = surface.footstepSounds[Random.Range(0, surface.footstepSounds.Length)];
                audioSource.PlayOneShot(clip);
                return;
            }
        }

        Debug.Log("발소리 못 찾음!");
    }

    // 나머지 메서드들 동일...

    public void PlayRoll()
    {
        if (rollSound != null)
        {
            audioSource.PlayOneShot(rollSound);
        }
    }
}

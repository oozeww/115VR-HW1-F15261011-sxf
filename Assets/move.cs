using UnityEngine;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{
    [Header("移动参数 (Celeste手感)")]
    public float maxSpeed = 5f;
    public float timeToMaxSpeed = 0.1f; 
    public float timeToStop = 0.05f;    

    [Header("跳跃参数")]
    public float jumpVelocity = 12f;       
    public float jumpCutMultiplier = 0.5f; 

    [Header("视觉特效与音效")]
    public GameObject smokePrefab;        
    public Transform feetPos;             
    public float smokeDestroyTime = 0.4f; 
    public GameObject deathEffect;       
    public AudioSource bgmSource;         
    public AudioClip jumpSFX;            
    public AudioClip landSFX;            
    public AudioClip dieSFX;

    [Header("挤压与拉伸 (视觉效果)")]
    public Transform visualChild; 
    public float maxStretch = 1.15f; 
    public float maxStretchVelocity = 12f; 
    public float squashStretchSmoothing = 10f; 

    private Rigidbody2D rb;
    private AudioSource sfxSource;
    private Animator animator; 
    private bool isGrounded = true;
    private float currentSpeedX;
    private bool isMoving; 
    
    private float facingDirection = 1f;

    // 单独记录当前平滑过渡的“拉伸数值”（绝对值）
    private float currentStretchY = 1f;
    private float currentStretchX = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sfxSource = GetComponent<AudioSource>();
        
        SpriteRenderer parentSR = GetComponent<SpriteRenderer>();
        if (parentSR != null) parentSR.enabled = false;

        if (visualChild != null)
        {
            animator = visualChild.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("请在 Inspector 面板中将 'Visual' 子物体拖入 'Visual Child' 槽位中！");
        }
    }

    void Update()
    {
        if (visualChild == null) return;

        HandleInput();
        ApplyVisualScale(); 
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(currentSpeedX, rb.linearVelocity.y);
    }

    private void HandleInput()
    {
        float inputX = 0f;
        if (Input.GetKey(KeyCode.D)) inputX = 1f;
        else if (Input.GetKey(KeyCode.A)) inputX = -1f;

        if (inputX != 0)
        {
            isMoving = true;
            facingDirection = inputX > 0 ? 1f : -1f; 
        }
        else
        {
            isMoving = false;
        }

        float targetSpeed = inputX * maxSpeed;
        float accelerationRate = Mathf.Abs(inputX) > 0 ? (maxSpeed / timeToMaxSpeed) : (maxSpeed / timeToStop);
        currentSpeedX = Mathf.MoveTowards(currentSpeedX, targetSpeed, accelerationRate * Time.deltaTime);

        if (animator != null) animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            if (animator != null) animator.SetTrigger("jump");
            PlaySFX(jumpSFX);
            isGrounded = false;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    // 重构的缩放逻辑：拉伸平滑过渡，转向瞬间完成
    private void ApplyVisualScale()
    {
        float targetStretchY = 1f;
        float targetStretchX = 1f;

        if (!isGrounded)
        {
            float stretchFactor = Mathf.Abs(rb.linearVelocity.y) / maxStretchVelocity;
            stretchFactor = Mathf.Clamp01(stretchFactor);

            targetStretchY = 1.0f + stretchFactor * (maxStretch - 1.0f);
            targetStretchX = 1.0f / targetStretchY;
        }

        // 1. 只对“绝对尺寸”进行平滑插值（Lerp）
        currentStretchY = Mathf.Lerp(currentStretchY, targetStretchY, squashStretchSmoothing * Time.deltaTime);
        currentStretchX = Mathf.Lerp(currentStretchX, targetStretchX, squashStretchSmoothing * Time.deltaTime);

        // 2. 瞬间应用朝向（正负号直接相乘，不经过平滑插值）
        visualChild.localScale = new Vector3(currentStretchX * facingDirection, currentStretchY, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                PlayLandingDust();
                PlaySFX(landSFX);
            }
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (bgmSource != null) bgmSource.Stop();
        PlaySFX(dieSFX);
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        if (visualChild != null) visualChild.localScale = Vector3.one;

        if (visualChild != null) visualChild.gameObject.SetActive(false);
        enabled = false;
        
        Invoke("ReloadScene", 0.5f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PlayLandingDust()
    {
        if (smokePrefab != null && feetPos != null)
        {
            GameObject dust = Instantiate(smokePrefab, feetPos.position, Quaternion.identity);
            Destroy(dust, smokeDestroyTime);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
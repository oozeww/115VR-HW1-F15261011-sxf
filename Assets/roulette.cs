using UnityEngine;

public class SpinWheel : MonoBehaviour
{
    [Header("旋转设置")]
    public float initialSpeed = 720f;
    public float decayRate = 0.8f;

    [Header("音效")]
    public AudioClip repeat;
    public AudioClip end;

    [Header("结束庆祝效果")]
    public ParticleSystem celebrationEffect;

    [Header("音效设置")]
    public float soundStopSpeed = 80f;   // 低于这个速度停止转动声
    public float minVolume = 0.1f;       // 最低音量
    public float minPitch = 0.5f;        // 最低音调

    private float currentSpeed;
    private bool isSpinning = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 确保庆祝粒子一开始不播放
        if (celebrationEffect != null)
        {
            celebrationEffect.Stop();
        }
    }

    void Update()
    {
        if (isSpinning)
        {
            // =========================
            // 1. 转盘旋转
            // =========================
            transform.Rotate(0, 0, currentSpeed * Time.deltaTime);

            // =========================
            // 2. 指数减速
            // =========================
            currentSpeed *= Mathf.Exp(-decayRate * Time.deltaTime);

            // =========================
            // 3. 根据速度调整音效
            // =========================
            float speedRatio = Mathf.Clamp01(currentSpeed / initialSpeed);

            // 速度越慢，音调越低
            audioSource.pitch = Mathf.Lerp(minPitch, 1.0f, speedRatio);

            // 速度越慢，音量越小
            audioSource.volume = Mathf.Lerp(minVolume, 1.0f, speedRatio);

            // =========================
            // 4. 转动声音停止
            // =========================
            if (currentSpeed < soundStopSpeed)
            {
                audioSource.Stop();
            }

            // =========================
            // 5. 转盘完全停止
            // =========================
            if (currentSpeed < 1f)
            {
                StopSpinning();
            }
        }
    }

    private void OnMouseDown()
    {
        // 正在转的时候不能再次点击
        if (isSpinning)
        {
            return;
        }

        Debug.Log("开始抽签！");

        // 开始旋转
        currentSpeed = initialSpeed;
        isSpinning = true;

        // 播放转盘声音
        if (repeat != null)
        {
            audioSource.clip = repeat;
            audioSource.loop = true;
            audioSource.pitch = 1f;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }

    private void StopSpinning()
    {
        // 停止旋转
        currentSpeed = 0f;
        isSpinning = false;

        // 停止转盘声音
        audioSource.Stop();

        // 播放结束声音
        if (end != null)
        {
            audioSource.PlayOneShot(end);
        }

        // 播放庆祝粒子
        if (celebrationEffect != null)
        {
            celebrationEffect.Play();
        }

        Debug.Log("抽签结束！");
    }
}
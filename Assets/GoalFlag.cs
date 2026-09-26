using UnityEngine;

public class GoalFlag : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isTriggered = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查是否是玩家觸發，且尚未觸發過
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            // 1. 觸發 Animator 切換動畫至 starAnimi（注意：名稱必須與 Parameters 裡的 PlayStar 完全一致）
            if (animator != null)
            {
                animator.SetTrigger("PlayStar");
            }

            // 2. 播放勝利音效（如果有加 AudioSource 且裡面有音效檔，就會播放；沒有也不影響動畫）
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }

            Debug.Log("抵達終點！播放 starAnimi 動畫。");
        }
    }
}
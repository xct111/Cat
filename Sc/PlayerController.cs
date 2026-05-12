using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float forwardSpeed = 5f;    // 向前移动速度
    public float sideSpeed = 3f;       // 左右移动速度
    public float jumpSpeed = 8f;
    private int currentLane = 1;       // 当前道路（0:左，1:中，2:右）
    private float[] laneXPositions = { -5f, 0f, 5f }; // 三条道路的X坐标

    [Header("组件引用")]
    private Rigidbody rb;
    private Animator anim;
    
    public AudioSource coinAudio;
    public AudioSource damageAudio;
    public AudioSource jumpAudio;
    public AudioSource victoryAudio;
    
    
    private CapsuleCollider capsuleCollider; // 胶囊碰撞体组件

    [Header("冲刺配置")]
    private float originalColliderHeight; // 保存碰撞体原始高度
    private Vector3 originalColliderCenter; // 保存碰撞体原始中心位置
    private bool isDashing = false; // 冲刺状态标记（防止重复触发）

    
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = capsuleCollider.height;
        originalColliderCenter = capsuleCollider.center;
        // 初始播放跑步动画
        anim.SetBool("isRunning", true);
    }

    private void Update()
    {
        // 游戏结束则停止所有操作
        if (GameManager.Instance.isGameOver) return;

        // 左右移动控制（AD键）
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
        {
            currentLane--;
        }
        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            currentLane++;
        }

        // 跳跃控制（示例：空格键跳跃，可根据需求调整）
        if (Input.GetKeyDown(KeyCode.W) && !anim.GetBool("isJumping"))
        {
            jumpAudio.Play();
            anim.SetBool("isJumping", true);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }


        if (Input.GetKeyDown(KeyCode.S) && !isDashing)
        {
            TriggerDash();
        }
    }
    
    private void TriggerDash()
    {
        anim.SetTrigger("Dash"); // 触发冲刺动画
        
        // 1. 计算新高度（原高度的1/3）
        float dashColliderHeight = originalColliderHeight / 3f;
        // 2. 计算新中心：y值改为新高度的一半（确保碰撞体底部贴合角色脚部）
        Vector3 dashColliderCenter = originalColliderCenter;
        dashColliderCenter.y = dashColliderHeight / 2f; // 关键：让碰撞体从脚部开始
        
        // 3. 应用新的高度和中心
        capsuleCollider.height = dashColliderHeight;
        capsuleCollider.center = dashColliderCenter;
        
        isDashing = true; // 标记为冲刺中
    }
    
    public void OnDashAnimationEnd()
    {
        // 恢复原始的高度和中心
        capsuleCollider.height = originalColliderHeight;
        capsuleCollider.center = originalColliderCenter;
        
        isDashing = false; // 重置冲刺状态
        Debug.Log("冲刺动画结束，碰撞体高度和位置已恢复");
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver)
        {
            rb.velocity = Vector3.zero; // 停止移动
            return;
        }

        // 持续向前的力
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        // 左右移动（平滑切换道路）
        Vector3 targetPosition = new Vector3(laneXPositions[currentLane], transform.position.y, transform.position.z);
        Vector3 sideMovement = Vector3.Lerp(transform.position, targetPosition, sideSpeed * Time.fixedDeltaTime) - transform.position;

        // 合并移动
        rb.MovePosition(transform.position + forwardMovement + sideMovement);
    }

    // 碰撞检测（障碍物/终点）
    private void OnCollisionEnter(Collision collision)
    {
        // 碰到障碍物死亡
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            forwardSpeed = 0f;
            capsuleCollider.enabled = false;
            rb.useGravity = false;
            anim.SetBool("isRunning", false);
            anim.SetTrigger("Die");
            damageAudio.Play();
            Invoke("GameOver",2f);
        }
    }

    public void GameOver()
    {
        GameManager.Instance.GameFail();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 到达终点
        if (other.gameObject.CompareTag("FinishLine"))
        {
            if (GameManager.Instance.goldCount >= GameManager.Instance.winCount)
            {
             victoryAudio.Play();   
            }
            else
            {
                damageAudio.Play();
            }
            GameManager.Instance.CheckWin();
            //victoryAudio.Play();
            

            anim.SetBool("isRunning", false);
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            coinAudio.Play();
            GameManager.Instance.AddGold();
            Destroy(other.gameObject);
        }

    }

    // 跳跃动画重置（动画事件调用，需在jump动画末尾添加）
    public void ResetJumpAnimation()
    {
        anim.SetBool("isJumping", false);
    }
}
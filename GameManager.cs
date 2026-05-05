using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI组件")]
    public Text goldText;       // 金币文本
    public GameObject failPanel;// 失败面板
    public GameObject winPanel; // 胜利面板

    private int goldCount = 0;  // 金币数量
    public bool isGameOver = false; // 游戏是否结束

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddGold()
    {
        if (isGameOver) return;
        goldCount++;
        goldText.text = "Obtain Quantity：" + goldCount;
    }

    // 游戏失败
    public void GameFail()
    {
        isGameOver = true;
        failPanel.SetActive(true);
    }

    // 游戏胜利
    public void GameWin()
    {
        isGameOver = true;
        winPanel.SetActive(true);
    }
}
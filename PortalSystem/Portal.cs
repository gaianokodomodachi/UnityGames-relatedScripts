using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("目标设置")]
    public GameObject targetMap;      // 要切换到的地图
    public Transform targetPoint;     // 玩家出生点

    private bool playerInRange = false;
    private Transform player;
    private bool isTransferring = false;

    // 关键：刚启用时的冷却（防止传送进来立刻又失效）
    private bool readyForInteract = false;

    private void OnEnable()
    {
        // 每次地图激活时，重置传输状态和冷却
        isTransferring = false;
        readyForInteract = false;

        // 下一帧再允许交互，防止按键穿透（即按一下E连续穿过两扇门）
        StartCoroutine(EnableInteractNextFrame());
    }

    private IEnumerator EnableInteractNextFrame()
    {
        yield return null;
        readyForInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        player = other.transform;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // 核心：保持范围内状态
        playerInRange = true;
        player = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
    }

    private void Update()
    {
        if (!readyForInteract || isTransferring) return;

        // 补救措施：如果已经在触发器内但引用丢失（常见于SetActive切换后）
        if (playerInRange && player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            // 防御检查
            if (MapManager.Instance == null || targetMap == null || player == null)
            {
                Debug.LogWarning($"Portal {name}: 引用缺失，无法传送");
                return;
            }

            StartCoroutine(DoTransfer());
        }
    }

    private IEnumerator DoTransfer()
    {
        isTransferring = true;
        readyForInteract = false;

        // 缓存引用
        Transform cachedPlayer = player;
        Transform cachedTargetPoint = targetPoint;

        // 执行地图切换
        MapManager.Instance.SwitchMap(targetMap);

        // 移动玩家位置
        if (cachedPlayer != null && cachedTargetPoint != null)
        {
            cachedPlayer.position = cachedTargetPoint.position;
        }

        // 如果你项目中有 InputMgr，保持它的调用
        // InputMgr.GetInstance().StartOrEndCheck(true);

        yield return null;
        isTransferring = false;
    }
}
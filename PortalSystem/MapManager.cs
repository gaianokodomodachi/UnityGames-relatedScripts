using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("在 Inspector 里指定初始地图")]
    public GameObject initialMap;

    private GameObject currentMap;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (initialMap == null)
        {
            Debug.LogError("MapManager: initialMap 没有指定！");
            return;
        }

        // 获取所有地图并隐藏
        GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");
        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }

        // 打开初始地图
        initialMap.SetActive(true);
        currentMap = initialMap;
    }

    public void SwitchMap(GameObject newMap)
    {
        if (newMap == null || currentMap == newMap)
            return;

        if (currentMap != null)
            currentMap.SetActive(false);

        newMap.SetActive(true);
        currentMap = newMap;

        // 【新增逻辑】强制唤醒玩家物理组件
        // 这样可以确保玩家传送到新地图后，即使不动也能触发新地图 Portal 的 OnTriggerStay
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.WakeUp(); // 强制唤醒物理检测
            }
        }
    }
}
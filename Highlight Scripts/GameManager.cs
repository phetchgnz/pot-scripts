using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Declare Singleton
    public static GameManager instance;

    [Header("Player Informations")]
    public Vector2 _spawnPoint;

    [Header("Scoreboard")]
    public int killCounts;
    public int deadCounts;
    public int healCounts;

    [Header("References")]
    [SerializeField] GameObject Player;

    [Header("Upgrade Points (ห้ามแก้ไข)")]
    public float attackUpgradePoint;
    public float defenseUpgradePoint;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
        } else {
            Destroy(gameObject);
        }
        SetNewSpawnPoint(Player.transform.position);
    }

    public void SetNewSpawnPoint(Vector2 _newPoint) {

        _spawnPoint = _newPoint;
    }

    public void RespawnPlayer(Transform player) {
        player.position = _spawnPoint;
    }

    public void ResetAllCounts() {
        killCounts = 0;
        deadCounts = 0;
        healCounts = 0;
    }
}

using System.Runtime.CompilerServices;
using System.Globalization;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{
    public static Checkpoint instance;
    public float _bonfireRange = 1.5f;
    private GameObject player;
    public bool hasBeenActivated;
    public Light2D light2D;
    public float targetIntensity = 0;


    void Awake() {
        if (instance == null) {
            instance = this;
        }

        if (PlayerPrefs.HasKey("Bonfire_" + gameObject.name)) {
            int hba = PlayerPrefs.GetInt("Bonfire_" + gameObject.name);

            if (hba == 1) {
                hasBeenActivated = true;
            }
        }

        if (PlayerPrefs.HasKey("Bonfire_Light2D_" + gameObject.name)) {
            targetIntensity = PlayerPrefs.GetFloat("Bonfire_Light2D_" + gameObject.name);
        }
    }
    
    private void Start() {
        player = GameObject.FindWithTag("Player");
    }
    
    private void Update() {

        LerpLight();

        if (IsPlayerInRange()) {
            GameManager.instance.SetNewSpawnPoint(gameObject.transform.position); //Respawb method
        }
    }

    public void Interact() {

        if (IsPlayerInRange() == true) {    //Check if player is in range.

            //SET PLAYER NEW SPAWN POINT AT THE SAME PLACE WITH THE MARY STATUE
            Debug.Log($"Checkpoint!" + GameManager.instance._spawnPoint); //log player position
            GameManager.instance.SetNewSpawnPoint(gameObject.transform.position); //Respawb method

            //SWITCH SCENE TO SCOREBOARD AND UPGRADE SCENE
            SceneController.instance.LoadScene("Bonfire");

            //SAVE PLAYER CURRENT WEAPON MOOD
            PlayerPrefs.SetInt("CurrentCrystal", NewWeaponMood.instance.currentCrystal);
            PlayerPrefs.SetInt("CurrentCrystalState", NewWeaponMood.instance.isGoodMood ? 1 : 0);

            //SAVE PLAYER STATS
            PlayerPrefs.SetFloat("HP", player.GetComponent<HealthManager>().healthAmount);
            PlayerPrefs.SetFloat("Attack", player.GetComponent<PlayerScript>().baseAttack + GameManager.instance.attackUpgradePoint);
            PlayerPrefs.SetInt("Defense", player.GetComponent<PlayerScript>()._def + (int)GameManager.instance.defenseUpgradePoint);
            PlayerPrefs.SetInt("CurrentCoins", player.GetComponent<Character>().currentCurrency);

            //SAVE NEEDED COINS AND LEVEL
            PlayerPrefs.SetInt("Level", player.GetComponent<Character>().currentLevel);
            PlayerPrefs.SetInt("CoinNeeded", player.GetComponent<Character>().PriceUpgrade);

            //SAVE PLAYER SCORE
            PlayerPrefs.SetInt("Kills", GameManager.instance.killCounts);
            PlayerPrefs.SetInt("Deads", GameManager.instance.deadCounts);

            //SAVE UPGRADE POINT
            PlayerPrefs.SetFloat("AtkUpgradePoint", GameManager.instance.attackUpgradePoint);
            PlayerPrefs.SetFloat("DefUpgradePoint", GameManager.instance.defenseUpgradePoint);

            //SAVE ATTACK & DEFENSE POINT [IT ACTUALLY CLEAR THEM TO 0]
            PlayerPrefs.SetFloat("UpAtkDefPoint", 0);
            
        } else Debug.Log("Not in Range!");
    }

    //Check if player is in Checkpoint object range

    public bool IsPlayerInRange() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,_bonfireRange);

        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag("Player")) {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected() {
        if (this.gameObject == null) {
            return;
        }
        Gizmos.DrawWireSphere(transform.position, _bonfireRange);
    }

    public void LerpLight() {
        light2D.intensity = Mathf.Lerp(light2D.intensity,targetIntensity,4*Time.deltaTime);
    }
}

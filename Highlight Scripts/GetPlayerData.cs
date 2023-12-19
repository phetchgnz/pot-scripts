using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetPlayerData : MonoBehaviour
{
    [Header("Player Stats & Scoreboard")]
    public TextMeshProUGUI hpTMP;
    public TextMeshProUGUI coinTMP;
    public TextMeshProUGUI attackTMP;
    public TextMeshProUGUI defenseTMP;
    public TextMeshProUGUI killTMP;
    public TextMeshProUGUI deadTMP;
    public TextMeshProUGUI playTimeTMP;


    [Header("Upgrade Stats")]
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI coinsNeededTMP;
    public TextMeshProUGUI coinsTMP;
    public TextMeshProUGUI upgradeAttackTMP;
    public TextMeshProUGUI upgradeDefenseTMP;

    [Header("Upgraded Status")]
    private float upgradedAttackTotal = 0;
    private float upgradedDefenseTotal = 0;
    public float upgradedAttackPoint;
    public float upgradedDefensePoint;

    [Header("Data Variables")]
    private float getHp;
    private float getAttack;
    private int getDefense;
    private int getCoin;
    private int getKills;
    private int getDeads;
    private int getLevel;
    private int getCoinNeeded;
    private float getAttackUpgradePoint;
    private float getDefenseUpgradePoint;

    public bool hasUpgrade;

    private void Start() {
        
        GetAndSetData();

        ShowStatsAndScore();

        if (PlayerPrefs.HasKey("UpAtkDefPoint")) {
            upgradedAttackPoint = PlayerPrefs.GetFloat("UpAtkDefPoint");
        } else Debug.Log("No data of ATK DEF POINT");
    }

    private void FixedUpdate() {
        UpdateTexts();
    }

    public void GetAndSetData() {
        if (PlayerPrefs.HasKey("HP")) {
            // Set player stats to variables
            getHp = PlayerPrefs.GetFloat("HP");
            getAttack = PlayerPrefs.GetFloat("Attack");
            getDefense = PlayerPrefs.GetInt("Defense");
            getCoin = PlayerPrefs.GetInt("CurrentCoins");

            //Set player Score to variables
            getKills = PlayerPrefs.GetInt("Kills");
            getDeads = PlayerPrefs.GetInt("Deads");

            //Set Upgrade data to vaeiables
            getLevel = PlayerPrefs.GetInt("Level");
            getCoinNeeded = PlayerPrefs.GetInt("CoinNeeded");
            getAttackUpgradePoint = PlayerPrefs.GetFloat("AtkUpgradePoint");
            getDefenseUpgradePoint = PlayerPrefs.GetFloat("DefUpgradePoint");
        }
    }

    public void ShowStatsAndScore() {
        if (PlayerPrefs.HasKey("HP")) {
            //Set player Stats to TMP
            hpTMP.text = "Hp : " + getHp.ToString("0");
            attackTMP.text = "Attack : " + getAttack;
            defenseTMP.text = "Defense : " + getDefense;
            coinTMP.text = "Coin : " + getCoin;

            //Set player Scores to TMP
            killTMP.text = "Kills : " + getKills;
            deadTMP.text = "Deads : " + getDeads;

            //Set player upgrade data to TMP
            levelTMP.text = "Level :  " + getLevel;
            coinsNeededTMP.text = "Coins Needed : " + getCoinNeeded;
            upgradeAttackTMP.text = "" + getAttack;
            upgradeDefenseTMP.text = "" + getDefense;
            coinsTMP.text = "Coins : " + getCoin;

            upgradedAttackPoint = 0;
            upgradedDefensePoint = 0;

            hasUpgrade = false;
        }
    }

    public void UpgradeAttack() {

        if (getCoin >= getCoinNeeded)
        {
            if (PlayerPrefs.HasKey("UpgradedAttackPoint")) {
                upgradedAttackTotal = PlayerPrefs.GetFloat("UpgradedAttackPoint");
            }
            upgradedAttackPoint += 1;

            levelTMP.text = "Level : " + getLevel++;

            getCoin -= getCoinNeeded;
            Character.instance.LevelUp();

            SaveUpgradeAttack();        //save to GameManager

            getCoinNeeded = Character.instance.PriceUpgrade;
            coinsNeededTMP.text = "Coins Needed : " + Character.instance.PriceUpgrade;

            hasUpgrade = true;
        } else Debug.Log("You don't have enough coins!");
    }
    public void UpgradeDefense() {
        
        if (getCoin >= getCoinNeeded)
        {
            if (PlayerPrefs.HasKey("UpgradedDefensePoint")) {
                upgradedDefenseTotal = PlayerPrefs.GetFloat("UpgradedDefensePoint");
            }
            upgradedDefensePoint += 1;

            levelTMP.text = "Level : " + getLevel++;

            getCoin -= getCoinNeeded;
            Character.instance.LevelUp();

            SaveUpgradeDefense();       //save to GameManager

            getCoinNeeded = Character.instance.PriceUpgrade;
            coinsNeededTMP.text = "Coins Needed : " + Character.instance.PriceUpgrade;

            hasUpgrade = true;

        } else Debug.Log("You don't have enough coins!");
    }

    public void UpdateTexts() {
        //Update Coin Texts
        coinsTMP.text = "Coins : " + getCoin;
        coinTMP.text = "Coins: " + getCoin;

        //Update Level & Coins Needed
        levelTMP.text = "Level : " + getLevel;
        coinsNeededTMP.text = "Coins Needed : " + getCoinNeeded;

        //Attack and Defense
        attackTMP.text = "Attack : " + (getAttack + upgradedAttackPoint);
        defenseTMP.text = "Defense : " + (getDefense + upgradedDefensePoint);

        //Upgrade attack and Defense in upgrade
        upgradeAttackTMP.text = "" + (getAttack + upgradedAttackPoint);
        upgradeDefenseTMP.text = "" + (getDefense + upgradedDefensePoint);
    }

    public void SaveUpgradeAttack() {
        PlayerPrefs.SetFloat("UpgradedAttackPoint", upgradedAttackPoint);
        PlayerPrefs.SetString("IsAttackUpgraded", "True");
    }

    public void SaveUpgradeDefense() {
        PlayerPrefs.SetFloat("UpgradedDefensePoint", upgradedDefensePoint);
        PlayerPrefs.SetString("IsDefenseUpgraded", "True");
    }

    public void SaveCurrentCurrencysAndLevel() {
        PlayerPrefs.SetInt("CurrentCoins", getCoin);

        PlayerPrefs.SetInt("LeveledUp", getLevel);
        PlayerPrefs.SetInt("PriceUpgraded", getCoinNeeded);
    }

    public void SaveUpgradeData() {
        SaveUpgradeAttack();
        SaveUpgradeDefense();
    }
}

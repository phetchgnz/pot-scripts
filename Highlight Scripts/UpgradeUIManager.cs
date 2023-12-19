using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GetPlayerData getPlayerData;
    [SerializeField] GameObject scoreboardPanel;
    [SerializeField] GameObject upgradePanel;
    private ChangeFirstButton changeFirstButton;

    [Header("Animator")]
    [SerializeField] Animator scoreboardAnim;
    [SerializeField] Animator upgradeAnim;
    public bool upgradeCheck = false;


    private void Start() {
        changeFirstButton = GetComponent<ChangeFirstButton>();
    }

    public void ShowUpgradePanel() 
    {
        upgradeCheck = true;
        changeFirstButton.stop = false;
        StartCoroutine(DelayAnim());

        IEnumerator DelayAnim() {
            scoreboardAnim.SetBool("Hide", true);
            yield return new WaitForSeconds(.5f);
            upgradeAnim.SetBool("Show", true);
        }
    }

    public void BackToScoreboard() {
        changeFirstButton.stop = false;
        upgradeCheck = false;
        StartCoroutine(DelayAnim());

        getPlayerData.SaveUpgradeData();
        getPlayerData.SaveCurrentCurrencysAndLevel();

        IEnumerator DelayAnim() {
            upgradeAnim.SetBool("Show", false);
            yield return new WaitForSeconds(.5f);
            scoreboardAnim.SetBool("Hide", false);
        }
    }

    public void BackToGameFromBonfire() {
        SceneController.instance.LoadScene("GamePlay");
        getPlayerData.SaveCurrentCurrencysAndLevel();

        if (getPlayerData.hasUpgrade == false) {
            PlayerPrefs.DeleteKey("UpgradedDefensePoint");
            PlayerPrefs.DeleteKey("UpgradedAttackPoint");
        } else Debug.Log("Get Upgraded Data");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Panel")]
    [SerializeField] GameObject gameplayPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject deadPanel;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject statueActivated;


    private PlayerInput playerInput;
    public bool CheckShowDeadScene = false;
    public bool Check = false;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        deadPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);
        statueActivated.SetActive(false);
    }

    
    //Pause Methods
    public void Pause(InputAction.CallbackContext context) {        //Method using by New Input System
        Pause();
    }

    public void Pause() {                                           //Pause Method
        //Switch active UI and Pause Time.timeScale
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
        Check = false;
        CheckShowDeadScene = false;
        Time.timeScale = 0;

        //Deactive player input
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void Resume() {                                         //Unpause Method
        //Switch active UI and Continue Time.timeScale
        pausePanel.SetActive(false);
        gameplayPanel.SetActive(true);
        Time.timeScale = 1;

        //Active player input
        playerInput.enabled = true;
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    //Dead Methods
    public void ShowDeadScene() {                                   //ShowDeadScene methods - use this when player dead
        //Switch active UI and Pause Time.timeScale
        gameplayPanel.SetActive(false);
        deadPanel.SetActive(true);
        CheckShowDeadScene = true;
        Check = false;
        StartCoroutine(Dead());

        //Deactive player input
        playerInput.enabled = false;

        IEnumerator Dead() {
            yield return new WaitForSeconds(1.3f);
            Time.timeScale = 0;
        }
    }
    
    //Buttons For UI
    public void RestartOnDeadScene() {                              //Restart button on Dead Scene

        //Check if Time.timeScale is equal to 0, if it is, change to 1
        if (Time.timeScale == 0) Time.timeScale = 1;
 
        //Load Scene back to Gameplay
        SceneManager.LoadScene("Gameplay");
    }

    //Statue Activated Panel Control
    public void ShowStatueActivated() {

        StartCoroutine(StatueActivateAnim());

        IEnumerator StatueActivateAnim() {
            statueActivated.SetActive(true);
            statueActivated.GetComponent<Animator>().SetTrigger("TriggerStatue");
            yield return new WaitForSeconds(6f);
            statueActivated.SetActive(false);
        }
    }

    //Go to Setting
    public void GoToSetting() {
        pausePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    //Back to Pause
    public void BackToPause() {
        settingPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}

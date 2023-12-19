using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("References")]
    [Header("Animator")]
    [SerializeField] Animator transitionAnim;
    [SerializeField] Animator buttonAnim1, buttonAnim2, buttonAnim3;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else DontDestroyOnLoad(gameObject);
    }

    //Load Scene with Scene Name and get player pref
    public void LoadScene(string sceneName) {                       //Load new scene with transition only
        StartCoroutine(LoadScene(sceneName));

        //Play Fade In&Out while switch scene
        IEnumerator LoadScene(string name) {
            transitionAnim.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(name);
            transitionAnim.SetTrigger("FadeIn");
        }
    }
    
    //Load Scene with Scene Name (Async)
    public void LoadSceneWButton(string nextSceneName) {          //Load new scene with transition and buttons
        StartCoroutine(LoadSceneAsync(nextSceneName));

        //Play Fade In&Out while switch scene
        IEnumerator LoadSceneAsync(string name) {
            transitionAnim.SetTrigger("FadeOut");
            buttonAnim1.SetTrigger("buttonTrig");
            buttonAnim2.SetTrigger("buttonTrig");
            buttonAnim3.SetTrigger("buttonTrig");
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(name);
            buttonAnim1.SetTrigger("buttonTrig2");
            buttonAnim2.SetTrigger("buttonTrig2");
            buttonAnim3.SetTrigger("buttonTrig2");
            transitionAnim.SetTrigger("FadeIn");
        }
    }

    //Quit the game
    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

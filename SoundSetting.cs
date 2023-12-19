using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSetting : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMusic (float volume) {
        audioMixer.SetFloat("music",volume);
        PlayerPrefs.SetFloat("music", volume);
    }
    public void SetAmbient (float ambient) {
        audioMixer.SetFloat("ambient",ambient);
        PlayerPrefs.SetFloat("ambient", ambient);
    }
    public void SetSFX (float sfx) {
        audioMixer.SetFloat("sfx",sfx);
        PlayerPrefs.SetFloat("sfx", sfx);
    }
}

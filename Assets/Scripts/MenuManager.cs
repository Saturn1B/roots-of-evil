using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField] Button play, options, quit;
    [SerializeField] GameObject menuPanel, optionPanel, controlPanel;
    [SerializeField] Slider soundSlider, musicSlider;

    [SerializeField] AudioClip music;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayMusic(music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene("Jolie");
    }

    public void Options()
    {
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);

        float soundValue;
        SoundManager.Instance.EffectsMixer.GetFloat("Sound", out soundValue);
        soundValue = Mathf.Pow(10, (soundValue / 20));
        soundSlider.value = soundValue;

        float musicValue;
        SoundManager.Instance.MusicMixer.GetFloat("Music", out musicValue);
        musicValue = Mathf.Pow(10, (musicValue / 20));
        musicSlider.value = musicValue;
    }

    public void Back()
    {
        menuPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void Control()
    {
        controlPanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void Back2()
    {
        controlPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

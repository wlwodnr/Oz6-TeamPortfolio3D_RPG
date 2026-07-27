using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    [Header("Tab Buttons")]
    [SerializeField] private UIButton Btn_GameplayTab;
    [SerializeField] private UIButton Btn_SoundTab;
    [SerializeField] private UIButton Btn_Close;

    [Header("Layout Panels")]
    [SerializeField] private GameObject layoutGameplay;
    [SerializeField] private GameObject layoutSound;

    [Header("Sound Controls")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private AudioSource bgmAudioSource;

    private void Awake()
    {
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }
    }

    private void OnEnable()
    {
        Btn_GameplayTab.BindOnClickButtonEvent(OnClickGameplayTab);
        Btn_SoundTab.BindOnClickButtonEvent(OnClickSoundTab);
        Btn_Close.BindOnClickButtonEvent(OnClickClose);

        if (layoutGameplay != null)
        {
            layoutGameplay.SetActive(true);
        }
        if (layoutSound != null)
        {
            layoutSound.SetActive(false);
        }

        // 사운드 매니저의 실제 현재 볼륨을 슬라이더 바에 동기화
        //if (bgmSlider != null && SoundManager.Inst != null)
        //{
        //    bgmSlider.value = SoundManager.Inst.GetBGMVolume();
        //}
    }
    
    private void OnDisable()
    {
        Btn_GameplayTab.UnBindAllOnClickButtonEvent();
        Btn_SoundTab.UnBindAllOnClickButtonEvent();
        Btn_Close.UnBindAllOnClickButtonEvent();
    }

    private void OnClickGameplayTab()
    {
        if (layoutGameplay != null)
        {
            layoutGameplay.SetActive(true);
        }
        if (layoutSound != null)
        {
            layoutSound.SetActive(false);
        }
    }

    private void OnClickSoundTab()
    {
        if (layoutGameplay != null)
        {
            layoutGameplay.SetActive(false); 
        }
        if (layoutSound != null)
        {
            layoutSound.SetActive(true);
        }
    }

    private void OnBgmVolumeChanged(float value)
    {
        //if (SoundManager.Inst != null)
        //{
        //    SoundManager.Inst.SetBGMVolume(value);
        //}
    }

    private void OnClickClose()
    {
       UIManager.Instance.CloseSettingUI();
    }
}
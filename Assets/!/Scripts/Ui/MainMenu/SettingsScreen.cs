using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the settings screen
/// </summary>
public class SettingsScreen : MonoBehaviour
{
    [SerializeField] UIDocument ui;
    [SerializeField] TitleScreen title;

    private void Awake()
    {
        if (ui == null)
            ui = GetComponent<UIDocument>();

        ui.rootVisualElement.style.display = DisplayStyle.None;

        // register action callbacks
        Button closeButton = ui.rootVisualElement.Q<Button>("CloseButton");
        closeButton.clicked += Hide;

        Slider sfxSlider = ui.rootVisualElement.Q<Slider>("SfxSlider");
        sfxSlider.RegisterValueChangedCallback(evt => { SetSfxVolume(sfxSlider.value); });

        Slider bgmSlider = ui.rootVisualElement.Q<Slider>("BgmSlider");
        bgmSlider.RegisterValueChangedCallback(evt => { SetBgmVolume(bgmSlider.value); });

        Slider masterSlider = ui.rootVisualElement.Q<Slider>("MasterSlider");
        masterSlider.RegisterValueChangedCallback(evt => { SetMasterVolume(masterSlider.value); });
    }

    /// <summary>
    /// Hides the screen
    /// </summary>
    private void Hide()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
        title.Show();
    }

    public void Show()
    {
        ui.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void SetSfxVolume(float volume)
    {
        Debug.Log(volume);
        AudioManager.Instance.audioMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 20f);
    }

    private void SetBgmVolume(float volume)
    {
        Debug.Log(volume);
        AudioManager.Instance.audioMixer.SetFloat("BgmVolume", Mathf.Log10(volume) * 20f);
    }

    private void SetMasterVolume(float volume)
    {
        Debug.Log(volume);
        AudioManager.Instance.audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
    }
}

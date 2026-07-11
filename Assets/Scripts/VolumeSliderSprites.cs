using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderSprites : MonoBehaviour
{
    private const string VolumePrefKey = "MasterVolume";
    private const string MutePrefKey = "MasterMuted";

    [Header("Slider")]
    public Slider volumeSlider;

    [Header("Barra por frames")]
    public Image volumeBarImage;
    public Sprite[] volumeFrames;

    [Header("Volume inicial")]
    [Range(0f, 1f)]
    public float defaultVolume = 1f;

    [Header("Botao mute")]
    public Image muteButtonImage;
    public Sprite unmutedSprite;
    public Sprite mutedSprite;

    private bool isMuted;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, defaultVolume);
        isMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.wholeNumbers = false;
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = isMuted ? 0f : volume;
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        UpdateVolumeFrame(volume);
        UpdateMuteSprite();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MutePrefKey, isMuted ? 1 : 0);

        float volume = volumeSlider != null ? volumeSlider.value : defaultVolume;
        AudioListener.volume = isMuted ? 0f : volume;

        UpdateMuteSprite();
    }

    private void UpdateVolumeFrame(float volume)
    {
        if (volumeBarImage == null || volumeFrames == null || volumeFrames.Length == 0)
        {
            return;
        }

        int frameIndex = Mathf.RoundToInt(volume * (volumeFrames.Length - 1));
        frameIndex = Mathf.Clamp(frameIndex, 0, volumeFrames.Length - 1);
        volumeBarImage.sprite = volumeFrames[frameIndex];
    }

    private void UpdateMuteSprite()
    {
        if (muteButtonImage == null)
        {
            return;
        }

        muteButtonImage.sprite = isMuted ? mutedSprite : unmutedSprite;
    }
}

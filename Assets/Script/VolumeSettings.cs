using UnityEngine;
using UnityEngine.UI;


public class VolumeSettings : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    public static VolumeSettings instance;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        LoadVolumeValues();
        /*
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
            Load();*/
    }
    public void SaveVolume()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("VolumeValue", volumeValue);
        LoadVolumeValues();
    }

    public void LoadVolumeValues()
    {
        float volumeValue = PlayerPrefs.GetFloat("VolumeValue");
        volumeSlider.value = volumeValue;
        AudioListener.volume = volumeValue;
    }
    /*
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void Load()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }*/
}

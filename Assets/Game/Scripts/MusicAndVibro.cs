using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicAndVibro : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private GameObject _offMusic;
    [SerializeField] private GameObject _onMusic;
    [SerializeField] private GameObject _offVibration;
    [SerializeField] private GameObject _onVibration;
    private int _isVibro;

    private void Start()
    {
        Vibration.Init();

        _musicSource.volume = PlayerPrefs.GetInt("MusicVolume", 1);
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (PlayerPrefs.GetInt("MusicVolume", 1) == 0)
            {
                _onMusic.SetActive(false);
                _offMusic.SetActive(true);
            }
            else
            {
                _offMusic.SetActive(false);
                _onMusic.SetActive(true);
            }

            _isVibro = (PlayerPrefs.GetInt("VibroState", 1));
            if (PlayerPrefs.GetInt("VibroState", 1) == 0)
            {
                _onVibration.SetActive(false);
                _offVibration.SetActive(true);
            }
            else
            {
                _offVibration.SetActive(false);
                _onVibration.SetActive(true);
            }
        }
    }

    public void MusicOFF()
    {
        _onMusic.SetActive(false);
        _offMusic.SetActive(true);
        _musicSource.volume = 0;
        PlayerPrefs.SetInt("MusicVolume", 0);
    }

    public void MusicON()
    {
        _offMusic.SetActive(false);
        _onMusic.SetActive(true);
        _musicSource.volume = 1;
        PlayerPrefs.SetInt("MusicVolume", 1);
    }

    public void VibrationOFF()
    {
        _onVibration.SetActive(false);
        _offVibration.SetActive(true);
        _isVibro = 0;
        PlayerPrefs.SetInt("VibroState", 0);
    }

    public void VibrationON()
    {
        _offVibration.SetActive(false);
        _onVibration.SetActive(true);
        _isVibro = 1;
        PlayerPrefs.SetInt("VibroState", 1);
    }

    public void PopVibration()
    {
        if (_isVibro == 1) Vibration.VibratePop();
    }

    public void PeekVibration()
    {
        if (_isVibro == 1) Vibration.VibratePeek();
    }

    public void NopeVibration()
    {
        if (_isVibro == 1) Vibration.VibrateNope();
    }

    public void ClassicVibration()
    {
        if (_isVibro == 1) Vibration.Vibrate();
    }
}
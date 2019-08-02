using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer; // mixer de áudio para controle do mesmo
    [SerializeField] private string _mixerParameter; // informa se é o mixer de SFX ou músicas
    private float _lastVolumeSFX, _lastVolumeMusic; // informa os valores das últimas configurações ao slider
    public Slider slider; // slider de volume

    private void Start() // verificação se já há um valor salvo previamente dos volumes, caso sim, esse valor é obtido, caso não, é definido um valor padrão para o mesmo
    {
        if (PlayerPrefs.HasKey(this._mixerParameter))
        {
            this._audioMixer.SetFloat(_mixerParameter, PlayerPrefs.GetFloat(this._mixerParameter));
        }
        else
        {
            this._audioMixer.SetFloat(_mixerParameter, 0);
        }

        if (this._mixerParameter == "MusicVolume")
        {
            if (PlayerPrefs.HasKey("LastVolumeMusic"))
            {
                this._lastVolumeMusic = PlayerPrefs.GetFloat("LastVolumeMusic");
                slider.value = _lastVolumeMusic;
            }
            else
            {
                this._lastVolumeMusic = 0;
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("LastVolumeSFX"))
            {
                this._lastVolumeSFX = PlayerPrefs.GetFloat("LastVolumeSFX");
                slider.value = _lastVolumeSFX;
            }
            else
            {
                this._lastVolumeSFX = 0;
            }
        }
    }
    public void ChangeAudioLevel(float audioLevel) // altera o nivel do áudio tanto da música quanto dos efeitos sonoros de acordo com o parâmetro passado pelo slider,
                                                   // logo após, salva os valores atualizados no playerprefs 
    {
        this._audioMixer.SetFloat(_mixerParameter, audioLevel);
        PlayerPrefs.SetFloat(this._mixerParameter, audioLevel);

        if (this._mixerParameter == "MusicVolume")
        {
            this._lastVolumeMusic = audioLevel;
            PlayerPrefs.SetFloat("LastVolumeMusic", _lastVolumeMusic);
        }
        else
        {
            this._lastVolumeSFX = audioLevel;
            PlayerPrefs.SetFloat("LastVolumeSFX", _lastVolumeSFX);
        }
    }
}

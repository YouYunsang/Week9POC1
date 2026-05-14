using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private AudioSource _ambienceAudioSource;
    [SerializeField] private AudioClip _underwaterAmbienceClip;
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private float _ambienceVolume = 0.45f;

    private void Awake()
    {
        if (_ambienceAudioSource == null)
        {
            Debug.LogError($"{nameof(AudioManager)}: Ambience AudioSource가 연결되지 않았습니다.");
            return;
        }

        // 수중 환경음은 반복 재생되어야 한다.
        _ambienceAudioSource.loop = true;
        _ambienceAudioSource.playOnAwake = false;
        _ambienceAudioSource.volume = _ambienceVolume;
        _ambienceAudioSource.clip = _underwaterAmbienceClip;
    }

    private void Start()
    {
        if (!_playOnStart)
        {
            return;
        }

        PlayUnderwaterAmbience();
    }

    public void PlayUnderwaterAmbience()
    {
        if (_ambienceAudioSource == null || _underwaterAmbienceClip == null)
        {
            Debug.LogWarning($"{nameof(AudioManager)}: 수중 환경음 클립 또는 AudioSource가 없습니다.");
            return;
        }

        if (_ambienceAudioSource.isPlaying)
        {
            return;
        }

        // 수중 환경음을 반복 재생한다.
        _ambienceAudioSource.Play();
    }

    public void StopUnderwaterAmbience()
    {
        if (_ambienceAudioSource == null)
        {
            return;
        }

        // 수중 환경음을 정지한다.
        _ambienceAudioSource.Stop();
    }

    public void SetAmbienceVolume(float volume)
    {
        _ambienceVolume = Mathf.Clamp01(volume);

        if (_ambienceAudioSource == null)
        {
            return;
        }

        // 환경음 볼륨을 조정한다.
        _ambienceAudioSource.volume = _ambienceVolume;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 볼륨은 0~1 사이로 제한한다.
        _ambienceVolume = Mathf.Clamp01(_ambienceVolume);
    }
#endif
}
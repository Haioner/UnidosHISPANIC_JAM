using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource _audioSource;
    private static AudioSource _continousAudioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _continousAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public static void PlayAudioClip(AudioClip clip)
    {
        _audioSource.volume = 1f;
        _audioSource.pitch = 1f;
        _audioSource.PlayOneShot(clip);
    }

    public static void PlayAudioClipPitch(AudioClip clip, float pitch)
    {
        _audioSource.volume = 1f;
        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(clip);
    }

    public static void PlayAudioClipVolume(AudioClip clip, float volume)
    {
        _audioSource.volume = volume;
        _audioSource.pitch = 1f;
        _audioSource.PlayOneShot(clip);
    }

    public static void PlayAudioClipVolumeAndPitch(AudioClip clip, float volume, float pitch)
    {
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(clip);
    }

    public static void PlayContinousAudioClipVolumeAndPitch(AudioClip clip, float volume, float pitch)
    {
        _continousAudioSource.volume = volume;
        _continousAudioSource.pitch = pitch;
        _continousAudioSource.PlayOneShot(clip);
    }

    public static void PlayLoopClip(AudioClip clip)
    {
        if (!_continousAudioSource.isPlaying)
        {
            _continousAudioSource.PlayOneShot(clip);
        }
    }

    public static void StopLoopClip()
    {
        if (_continousAudioSource.isPlaying)
        {
            _continousAudioSource.Stop();
        }
    }
}

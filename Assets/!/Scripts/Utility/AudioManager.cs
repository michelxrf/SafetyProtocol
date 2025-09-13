using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// Plays SFXs independent of game object
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private GameObject sfxObjectPrefab;
    public AudioMixer audioMixer;

    public enum DEFAULT_UISFX { CLICK, HOVER }

    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip hoverSFX;

    Dictionary<DEFAULT_UISFX, AudioClip> defaultUiSfxs = new Dictionary<DEFAULT_UISFX, AudioClip>();



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            defaultUiSfxs[DEFAULT_UISFX.CLICK] = clickSFX;
            defaultUiSfxs[DEFAULT_UISFX.HOVER] = hoverSFX;
        }
    }

    /// <summary>
    /// Spawns a audio object at a spot and plays it
    /// </summary>
    /// <param name="audioclip">The clip list that will play, one will picked at random.</param>
    /// <param name="playAt">the spot where it'll be played</param>
    /// <param name="volume">volume override</param>
    public void PlaySFX(AudioClip[] audioclips, Transform playAt)
    {
        int rand = Random.Range(0, audioclips.Length);

        PlaySFX(audioclips[rand], playAt);
    }

    /// <summary>
    /// Spawns a audio object at a spot and plays it
    /// </summary>
    /// <param name="audioclip">The clip that will play</param>
    /// <param name="playAt">the spot where it'll be played</param>
    /// <param name="volume">volume override</param>
    public void PlaySFX(AudioClip audioclip, Transform playAt)
    {
        AudioSource audioSource = Instantiate(sfxObjectPrefab, playAt.position, Quaternion.identity).GetComponent<AudioSource>();

        audioSource.clip = audioclip;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioclip.length);
    }

    /// <summary>
    /// Spawns a default Ui sound
    /// </summary>
    /// <param name="sfxCode">enum to identify what sound to play</param>
    /// <param name="playAt"></param>
    /// <param name="volume"></param>
    public void PlaySFX(DEFAULT_UISFX sfxCode, Transform playAt)
    {
        AudioSource audioSource = Instantiate(sfxObjectPrefab, playAt.position, Quaternion.identity).GetComponent<AudioSource>();

        audioSource.clip = defaultUiSfxs[sfxCode];
        audioSource.Play();

        Destroy(audioSource.gameObject, defaultUiSfxs[sfxCode].length);
    }
}

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
        }
    }

    /// <summary>
    /// Spawns a audio object at a spot and plays it
    /// </summary>
    /// <param name="audioclip">The clip list that will play, one will picked at random.</param>
    /// <param name="playAt">the spot where it'll be played</param>
    /// <param name="volume">volume override</param>
    public void PlaySFX(AudioClip[] audioclips, Transform playAt, float volume = 1f)
    {
        int rand = Random.Range(0, audioclips.Length);

        PlaySFX(audioclips[rand], playAt, volume);
    }

    /// <summary>
    /// Spawns a audio object at a spot and plays it
    /// </summary>
    /// <param name="audioclip">The clip that will play</param>
    /// <param name="playAt">the spot where it'll be played</param>
    /// <param name="volume">volume override</param>
    public void PlaySFX(AudioClip audioclip, Transform playAt, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(sfxObjectPrefab, playAt.position, Quaternion.identity).GetComponent<AudioSource>();

        audioSource.clip = audioclip;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioclip.length);
    }
}

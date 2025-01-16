using UnityEngine;

/// Enum representing different audio clips available in the game.
public enum AudioClipType
{
    BallGroundPitch,
    BallHoopHit,
    ScoreSound
}

public class AudioManager : MonoBehaviour
{
    // Singleton instance of AudioManager
    public static AudioManager Instance;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip[] audioClips;  // Array of audio clips indexed by AudioClipType

    private void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate AudioManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optionally persist across scenes
        DontDestroyOnLoad(gameObject);
    }

    public static void PlayAudio(AudioClipType clipType)
    {
        if (Instance == null)
        {
            Debug.LogError("AudioManager instance is null. Ensure an AudioManager exists in the scene.");
            return;
        }

        if (Instance.audioClips == null || Instance.audioClips.Length <= (int)clipType)
        {
            Debug.LogError($"Audio clip for {clipType} is not assigned or out of range.");
            return;
        }

        // Play the audio clip with a randomized pitch for added realism
        Instance.audioSource.pitch = Random.Range(0.8f, 1.2f);
        Instance.audioSource.PlayOneShot(Instance.audioClips[(int)clipType]);
    }
}

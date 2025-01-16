using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the activation and deactivation of particle effects when triggered.
/// </summary>
public class ParticleManager : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem particleSystem0;
    [SerializeField] private ParticleSystem particleSystem1;

    [Header("Particle Settings")]
    [SerializeField] private float particleDuration = 2f; // Duration for which particles will play

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            StartCoroutine(PlayParticles());
        }
    }

    private IEnumerator PlayParticles()
    {
        if (particleSystem0 != null)
        {
            particleSystem0.Play();
        }

        if (particleSystem1 != null)
        {
            particleSystem1.Play();
        }

        // Wait for the specified duration before stopping the particles
        yield return new WaitForSeconds(particleDuration);

        if (particleSystem0 != null)
        {
            particleSystem0.Stop();
        }

        if (particleSystem1 != null)
        {
            particleSystem1.Stop();
        }
    }
}

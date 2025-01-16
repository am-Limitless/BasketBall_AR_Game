using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int score;
    public int throws;
    public TMP_Text scoreText;


    private void OnEnable()
    {
        BallManager.onBallThrown += BallThrown;
    }
    private void OnDisable()
    {
        BallManager.onBallThrown -= BallThrown;
    }

    private void BallThrown()
    {
        throws++;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hoop"))
        {
            score++;
            scoreText.text = "Score: " + score;
            AudioManager.PlayAudio(global::AudioClipType.ScoreSound);
        }

        if (other.CompareTag("Ground"))
        {
            score--;
            scoreText.text = "Score: " + score;
        }
    }

    
}

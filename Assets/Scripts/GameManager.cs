using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Difficulty selectedDifficulty = Difficulty.Normal;
    public int currentWave = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetDamageMultiplier()
    {
        return selectedDifficulty switch
        {
            Difficulty.Easy => 0.6f,
            Difficulty.Hard => 1.5f,
            _ => 1.0f,
        };
    }

    public float GetWaveScalingMultiplier()
    {
        return selectedDifficulty switch
        {
            Difficulty.Easy => 0.5f,
            Difficulty.Hard => 2.0f,
            _ => 1.0f,
        };
    }

    public float GetEnemyBaseDamage(float originalDamage)
    {
        return originalDamage * GetDamageMultiplier();
    }

    public float GetWaveDamageBonus(float baseIncrease, int wave)
    {
        return (wave - 1) * baseIncrease * GetWaveScalingMultiplier();
    }
}

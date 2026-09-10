using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("Stage Data")]
    [SerializeField] private List<StageData> stages = new List<StageData>();

    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLevel(int level)
    {
        CurrentLevel = level;
    }

    public StageData GetCurrentStage()
    {
        foreach (StageData stage in stages)
        {
            if (stage.levelNumber == CurrentLevel)
            {
                return stage;
            }
        }

        Debug.LogError("Stage not found: " + CurrentLevel);
        return null;
    }
}
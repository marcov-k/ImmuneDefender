using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerData;

public class Loading : MonoBehaviour
{
    [SerializeField] LevelData[] levelDatas;
    [SerializeField] GameObject enemyCont;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject defenceCont;
    [SerializeField] GameObject defencePrefab;

    void Start()
    {
        ShowLevelData(levelDatas[startedLevel - 1]);
    }

    void ShowLevelData(LevelData levelData)
    {
        foreach (var enemy in levelData.newEnemies)
        {
            Instantiate(enemyPrefab, enemyCont.transform).GetComponent<EnemyInfo>().ShowEnemy(enemy);
        }

        foreach (var defence in levelData.newDefences)
        {
            Instantiate(defencePrefab, defenceCont.transform).GetComponent<DefenceInfo>().ShowDefence(defence);
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level");
    }
}

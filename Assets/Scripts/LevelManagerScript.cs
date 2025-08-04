using UnityEngine;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour
{
    public int enemies;
    public Text enemiesText;

    private void Awake()
    {
        enemiesText.text = enemies.ToString();
        Enemy.OnEnemyKilled += OnEnemyKilledAction;
    }
    void OnEnemyKilledAction()
    {
        enemies--;
        enemiesText.text = enemies.ToString();
    }
}

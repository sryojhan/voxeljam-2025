using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    // Acrobat enemy
    [SerializeField] GameObject pivot;
    [SerializeField] Vector2 acrobatEnemySpawnX;
    [SerializeField] GameObject acrobatEnemyPrefab;

    // Basic Enemy
    [SerializeField] GameObject basicEnemyPrefab;
    [SerializeField] Vector2 basicEnemySpawnX;
    [SerializeField] float basicEnemySpawnY;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SpawnAcrobatEnemy();
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SpawnBasicEnemy();
        }
    }

    void SpawnAcrobatEnemy()
    {
        float x = Random.Range(acrobatEnemySpawnX.x, acrobatEnemySpawnX.y);

        int side = Random.Range(0, 2);

        if (side == 0)
            x *= -1;

        Vector3 enemyPosition = new Vector3(x, pivot.transform.position.y, 0);

        GameObject enemy = Instantiate(acrobatEnemyPrefab, enemyPosition, Quaternion.identity, null);

        AcrobatEnemy acrobat = enemy.GetComponent<AcrobatEnemy>();
        acrobat.SetSpawnSide((AcrobatEnemy.Side)side);
        acrobat.SetPivotPosition(pivot.transform.position);
    }

    void SpawnBasicEnemy()
    {
        float x = Random.Range(basicEnemySpawnX.x, basicEnemySpawnX.y);

        Vector3 enemyPosition = new Vector3(x, basicEnemySpawnY, 0);

        GameObject enemy = Instantiate(basicEnemyPrefab, enemyPosition, Quaternion.identity, null);
    }
}

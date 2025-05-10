using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] GameObject pivot;
    [SerializeField] Vector2 acrobatEnemySpawnX;
    [SerializeField] GameObject acrobatEnemyPrefab;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnAcrobatEnemy();
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
}

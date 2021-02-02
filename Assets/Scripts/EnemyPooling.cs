using UnityEngine;

public class EnemyPooling : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private int maxEnemyCount = 0;
    [SerializeField] private float spawnTimeMin = 1f;
    [SerializeField] private float spawnTimeMax = 4f;

    private EnemyController[] enemies;
    private Vector3 objectPoolPosition = Vector2.zero;
    private float spawnMinY = 0;
    private float spawnMaxY = 0;
    private float spawnX = 0;
    private float deleteX = 0;

    private void Start()
    {
        objectPoolPosition = new Vector2((GameManager.instance.camWidth / 2f) + .5f, (GameManager.instance.camHeight / 2f) + 100f);
        spawnMinY = (-GameManager.instance.camHeight / 2f) + .25f;
        spawnMaxY = (GameManager.instance.camHeight / 2f) - .25f;
        spawnX = (GameManager.instance.camWidth / 2f) + 1.5f;
        deleteX = (GameManager.instance.camWidth / 2f) + 2f;

        enemies = new EnemyController[maxEnemyCount];

        for (int i = 0; i < maxEnemyCount; i++)
            enemies[i] = Instantiate(enemyPrefab, objectPoolPosition, Quaternion.identity).GetComponent<EnemyController>();

        InitialSpawn();
    }

    private void Update()
    {
        HandleSpawningAndDeleting();
    }

    public void Reset()
    {
        foreach (EnemyController enemy in enemies)
        {
            DeleteEnemy(enemy);
        }
    }

    private void HandleSpawningAndDeleting()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.transform.position.x >= deleteX || enemy.transform.position.x <= -deleteX)
                DeleteEnemy(enemy);
            
            if (enemy.deleted)
            {
                if (!GameManager.instance.gamePaused)
                    enemy.timeSinceDeleted += Time.deltaTime;

                if (enemy.timeSinceDeleted >= enemy.timeToSpawn)
                {
                    enemy.deleted = false;
                    SpawnEnemy(enemy);
                }
            }
            else
            {
                if (enemy.transform.localScale.x < 0)
                    enemy.Move(new Vector2(enemy.speed, 0));
                else if (enemy.transform.localScale.x > 0)
                    enemy.Move(new Vector2(-enemy.speed, 0));
            }
        }
    }

    private void SpawnEnemy(EnemyController enemy)
    {
        enemy.RandomizeSize();

        if (Random.Range(0, 2) == 0)
        {
            enemy.transform.position = new Vector2(-spawnX, Random.Range(spawnMinY, spawnMaxY));
            float scale = enemy.transform.localScale.x;
            enemy.transform.localScale = new Vector2(scale * -1f, scale);
        }
        else
        {
            enemy.transform.position = new Vector2(spawnX, Random.Range(spawnMinY, spawnMaxY));
        }
        
        enemy.deleted = false;
        enemy.speed = Random.Range(enemy.speedMin, enemy.speedMax);
        enemy.UpdateColorBasedOnSize();
    }

    private void InitialSpawn()
    {
        foreach (EnemyController enemy in enemies)
            DeleteEnemy(enemy);
    }

    public void DeleteEnemy(EnemyController enemy)
    {
        enemy.transform.position = objectPoolPosition;
        enemy.transform.localScale = new Vector2(1f, 1f);
        enemy.deleted = true;
        enemy.timeSinceDeleted = 0;
        enemy.timeToSpawn = Random.Range(spawnTimeMin, spawnTimeMax);
    }
}
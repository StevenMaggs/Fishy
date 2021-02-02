using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb2d;
    public Vector2 velocity = Vector2.zero;

    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private float speed = 7.5f;
    [SerializeField] private float maxMagnitude = 10f;

    private SpriteRenderer spriteRenderer;
    private float moveHorizontal = 0;
    private float moveVertical = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.AddForce(new Vector2(0, -250f));
    }

    private void FixedUpdate()
    {
        HandlePhysicsMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Mathf.Abs(other.transform.localScale.magnitude) > Mathf.Abs(transform.localScale.magnitude))
        {
            // player death
            AudioManager.instance.Play("PlayerDeath", GameManager.instance.volumeSFX);
            GameManager.instance.ResetGame();
        }
        else
        {
            // player eats enemy
            GameManager.instance.poolScript.DeleteEnemy(other.gameObject.GetComponent<EnemyController>());
            GameManager.instance.playerScript.IncreaseSize(other.gameObject);
            AudioManager.instance.Play("PlayerEat", GameManager.instance.volumeSFX);
            transform.localScale = new Vector2(transform.localScale.x * -1f, transform.localScale.y);
        }
    }

    private void Update()
    {
        HandleInput();

        if (GameManager.instance.gameOver || GameManager.instance.gamePaused)
            return;

        HandleWrapping();
        HandleAnimation();
    }

    public void Reset()
    {
        rb2d.velocity = Vector2.zero;
        transform.position = playerPrefab.transform.position;
        transform.localScale = playerPrefab.transform.localScale;
        rb2d.AddForce(new Vector2(0, -250f));
    }

    public void IncreaseSize(GameObject enemy)
    {
        float increaseAmount = Mathf.Abs(enemy.transform.localScale.x * .01f);

        if (transform.localScale.x < 0)
            transform.localScale = new Vector2(-increaseAmount + transform.localScale.x, increaseAmount + transform.localScale.y);
        else if (transform.localScale.x > 0)
            transform.localScale = new Vector2(increaseAmount + transform.localScale.x, increaseAmount + transform.localScale.y);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.instance.gamePaused)
            GameManager.instance.PauseGame();
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.gamePaused && !GameManager.instance.gameOptions)
            GameManager.instance.ResumeGame();
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.gamePaused && GameManager.instance.gameOptions && GameManager.instance.changesSaved)
        {
            GameManager.instance.CloseOptionsMenu();
            GameManager.instance.PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.gamePaused && GameManager.instance.gameOptions && !GameManager.instance.changesSaved)
        {
            GameManager.instance.OpenOptionsQuitConfirmationMenu();
        }

        if (GameManager.instance.gameOver || GameManager.instance.gamePaused)
        {
            moveHorizontal = 0;
            moveVertical = 0;
        }
        else
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        }
    }

    private void HandlePhysicsMovement()
    {
        Vector2 movement = new Vector2(moveHorizontal * speed, moveVertical * speed);
        rb2d.AddForce(movement);

        rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxMagnitude);

        if (!GameManager.instance.gamePaused)
            velocity = rb2d.velocity;
    }

    private void HandleWrapping()
    {
        if (transform.position.x > GameManager.instance.camWidth / 2f + .25f)
            transform.position = new Vector2(-GameManager.instance.camWidth / 2f - .25f, transform.position.y);
        else if (transform.position.x < -GameManager.instance.camWidth / 2f - .25f)
            transform.position = new Vector2(GameManager.instance.camWidth / 2f + .25f, transform.position.y);
        
        if (transform.position.y > GameManager.instance.camHeight / 2f + .25f)
            transform.position = new Vector2(transform.position.x , -GameManager.instance.camHeight / 2f - .25f);
        else if (transform.position.y < -GameManager.instance.camHeight / 2f - .25f)
            transform.position = new Vector2(transform.position.x , GameManager.instance.camHeight / 2f + .25f);
    }

    private void HandleAnimation()
    {
        if (moveHorizontal > 0 && transform.localScale.x > 0)
            transform.localScale = new Vector2(transform.localScale.x * -1f, transform.localScale.y);
        else if (moveHorizontal < 0 && transform.localScale.x < 0)
            transform.localScale = new Vector2(transform.localScale.x * -1f, transform.localScale.y);
    }
}
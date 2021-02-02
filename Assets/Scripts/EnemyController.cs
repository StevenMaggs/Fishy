using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float timeSinceDeleted = 0;
    public float timeToSpawn = 0;
    public bool deleted = true;
    public float speed = 0;
    public float sizeMin = .15f;
    public float sizeMax = 2f;
    public float speedMin = 0;
    public float speedMax = 0;
    public Color colorMin;
    public Color colorMax;

    private SpriteRenderer spriteRenderer;
    private Gradient gradient;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gradient = new Gradient();

        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = colorMin;
        colorKey[0].time = 0;
        colorKey[1].color = colorMax;
        colorKey[1].time = 1f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 1f;

        gradient.SetKeys(colorKey, alphaKey);
    }

    public void RandomizeSize()
    {
        float random = Random.Range(sizeMin, sizeMax);

        transform.localScale = new Vector2(random, random);
    }

    public void UpdateColorBasedOnSize()
    {
        float size = transform.localScale.x;
        float percentage = Mathf.Abs(((size - sizeMin) / (sizeMax - sizeMin)));

        spriteRenderer.color = gradient.Evaluate(percentage);
    }

    public void Move(Vector2 movement)
    {
        if (!deleted && !GameManager.instance.gamePaused)
            transform.Translate(movement * Time.deltaTime);
    }
}
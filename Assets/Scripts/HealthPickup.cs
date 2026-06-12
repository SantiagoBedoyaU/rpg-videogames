using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float pickupRadius = 0.2f;

    private void Start()
    {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = pickupRadius;

        SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateCircleSprite();
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 100;
        renderer.color = new Color(0.2f, 1f, 0.2f, 1f);
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        transform.localScale = Vector3.one * 0.4f;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * 3f) * 0.15f;
        Vector3 pos = transform.position;
        pos.y += yOffset * Time.deltaTime * 3f;
        transform.position = pos;
    }

    private Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color c = Color.white;
        Color t = new Color(0, 0, 0, 0);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, d <= radius ? c : t);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealToFull();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupSound);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class NegativePrefab : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private BuildingManager buildingManager; // Добавим ссылку на BuildingManager
    public TextMeshProUGUI devourerDestroyedText; // Ссылка на TextMeshProUGUI для отображения сообщения
    public static event Action NegativePrefabDestroyed; // Событие для оповещения об уничтожении NegativePrefab
    public ParticleSystem particleSystemGameObject;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Найдем BuildingManager в иерархии объектов
        buildingManager = FindObjectOfType<BuildingManager>();

        if (buildingManager == null)
        {
            Debug.LogError("BuildingManager not found in the scene.");
        }

        StartCoroutine(MoveInRandomDirections());
    }

    private IEnumerator MoveInRandomDirections()
    {
        while (true)
        {
            float randomX = UnityEngine.Random.Range(-1f, 1f);
            float randomY = UnityEngine.Random.Range(-1f, 1f);

            Vector2 dir = new Vector2(randomX, randomY).normalized;
            // Установка скорости
            rb.velocity = speed * dir;

            // Ожидание перед следующим изменением направления
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));

            // Проверка и коррекция, чтобы объект оставался в пределах экрана
            ClampPositionToScreen();

            // Отнимаем 1 душу каждую секунду
            if (buildingManager != null)
            {
                buildingManager.souls--;
                buildingManager.UpdateSoulsText();
            }
        }
    }

    private void ClampPositionToScreen()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        transform.position = Camera.main.ViewportToWorldPoint(viewPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверка столкновения с душами
        if (other.CompareTag("Souls"))
        {
            // Уничтожение души
            Destroy(other.gameObject);
            
            // Вызов события об уничтожении NegativePrefab
            OnNegativePrefabDestroyed();
        }
    }

    private void OnNegativePrefabDestroyed()
    {
        Debug.Log("NegativePrefab Destroyed Event Triggered");
        // Включаем систему частиц
        particleSystemGameObject.Play();
        NegativePrefabDestroyed?.Invoke(); // Вызов события
    }
}

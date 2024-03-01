using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel; // Ссылка на панель с сообщением "GAME OVER"
    public BuildingManager buildingManager; // Ссылка на BuildingManager для доступа к количеству душ

    private void Start()
    {
        // Скрываем панель GAME OVER при запуске
        gameOverPanel.SetActive(false);

        // Проверяем, чтобы была ссылка на BuildingManager
        if (buildingManager == null)
        {
            Debug.LogError("BuildingManager reference not set in GameOver script.");
        }
    }

    private void Update()
    {
        // Проверяем условие GAME OVER
        if (buildingManager != null && buildingManager.souls < 0)
        {
            // Показываем панель GAME OVER
            ShowGameOverPanel();

            // Останавливаем игровое время (по желанию)
            Time.timeScale = 0f;
        }
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
}

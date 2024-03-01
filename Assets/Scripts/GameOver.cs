using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel; // ������ �� ������ � ���������� "GAME OVER"
    public BuildingManager buildingManager; // ������ �� BuildingManager ��� ������� � ���������� ���

    private void Start()
    {
        // �������� ������ GAME OVER ��� �������
        gameOverPanel.SetActive(false);

        // ���������, ����� ���� ������ �� BuildingManager
        if (buildingManager == null)
        {
            Debug.LogError("BuildingManager reference not set in GameOver script.");
        }
    }

    private void Update()
    {
        // ��������� ������� GAME OVER
        if (buildingManager != null && buildingManager.souls < 0)
        {
            // ���������� ������ GAME OVER
            ShowGameOverPanel();

            // ������������� ������� ����� (�� �������)
            Time.timeScale = 0f;
        }
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
}

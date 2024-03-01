using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BuildingManager : MonoBehaviour
{
    public int[] buildingCosts = { 10, 20, 30 }; // ��������� ������
    public GameObject[] buildingPrefabs; // ������� ������
    public GameObject negativeEffectPrefab; // ������ � ���������� ��������
    public TextMeshProUGUI soulsText; // UI Text ��� ����������� ���������� ���
    public TextMeshProUGUI errorMessageText; // UI Text ��� ����������� ������
    public int souls = 0;
    private float soulGenerationRate = 1f;
    private float nextSoulGenerationTime;
    private float spawnRadius = 2f; // ������ ������
    private float errorMessageDuration = 2f; // ����������������� ����������� ������
    public AudioClip notEnoughSoulsSound; // ��������� ��� �����, ����� ��� ������������
    private AudioSource audioSource;
    public TextMeshProUGUI devourerOfSoulsText; // UI Text ��� ����������� ��������� ���������� ���
    public GameObject soulsPrefab;
    private bool building0Constructed = false; // ����, �����������, ��������� �� ������ � �������� 0
    public TextMeshProUGUI devourerOfSoulsDeadText; // �������� ��� ����
    public float devourerOfSoulsDeadDuration = 2f;


    private void Start()
    {
        nextSoulGenerationTime = Time.time + soulGenerationRate;
        audioSource = GetComponent<AudioSource>(); // �������� ��������� AudioSource �� �������, � �������� ���������� ������
        // ��������� �������������
        UpdateSoulsText();

        // ������ �������� ��� �������� �������� � ���������� ��������
        StartCoroutine(GenerateNegativeEffects());
        // ������ �������� ��� �������� �������� Souls ��� ����������� ������ � �������� 0
        StartCoroutine(GenerateSoulsPrefab());
        // ������������� �� ������� NegativePrefabDestroyed
        NegativePrefab.NegativePrefabDestroyed += OnNegativePrefabDestroyed;

    }
    private void OnNegativePrefabDestroyed()
    {
        ShowDevourerDestroyedMessage("���������� ��� ���������!");
    }
    private void ShowDevourerDestroyedMessage(string message)
    {
        devourerOfSoulsDeadText.text = message;
        StartCoroutine(ClearDevourerDestroyedMessageAfterDelay());
    }

    private IEnumerator ClearDevourerDestroyedMessageAfterDelay()
    {
        yield return new WaitForSeconds(devourerOfSoulsDeadDuration);
        ClearDevourerDestroyedMessage();
    }


    private void ClearDevourerDestroyedMessage()
    {
        devourerOfSoulsDeadText.text = "";
    }
    private IEnumerator GenerateSoulsPrefab()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // ��������� 5 ������

            // ���������, ��������� �� ������ � �������� 0
            if (building0Constructed)
            {
                // ������� ������ Souls � ��������� ����� � �������� ������
                Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
                Instantiate(soulsPrefab, new Vector2(randomPosition.x,randomPosition.y), Quaternion.identity);
            }
        }
    }

    public void BuyBuilding(int index)
    {
        if (souls >= buildingCosts[index])
        {
            souls -= buildingCosts[index];
            Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
            GameObject newBuilding = Instantiate(buildingPrefabs[index], new Vector2(randomPosition.x, randomPosition.y), Quaternion.identity);

            // �������� ��������� "Audio Source" ��� ���������������� ������ � �������������� ���������
            AudioSource buildingAudio = newBuilding.GetComponent<AudioSource>();
            if (buildingAudio != null)
            {
                buildingAudio.Play();
            }

            // ��������� ��������
            Animator buildingAnimator = newBuilding.GetComponent<Animator>();
            if (buildingAnimator != null)
            {
                buildingAnimator.SetTrigger("BuildTrigger");
            }

            UpdateSoulsText();
            ClearErrorMessage();
            // ��������, ���� ��������� ������ � �������� 0, ������������� ���� � true
            if (index == 0)
            {
                building0Constructed = true;
            }

            // ��������� �������� ��������� ��� � ����������� �� ������� ������������ ������
            AdjustSoulGenerationRate(index);
        }
        else
        {
            audioSource.PlayOneShot(notEnoughSoulsSound, 0.5f); // ������������� ����, ���� ��� ������������
            ShowErrorMessage("������������ ��� ��� ������� ������!");
        }
    }

    private void AdjustSoulGenerationRate(int index)
    {
        // ��������� �������� ��������� ��� � ����������� �� ������� ������������ ������
        switch (index)
        {
            case 0:
                soulGenerationRate *= 0.9f; // ���������� �� 10%
                break;
            case 1:
                soulGenerationRate *= 0.5f; // ���������� �� 50%
                break;
            case 2:
                soulGenerationRate *= 0.1f; // ���������� �� 90%
                break;
        }
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        StartCoroutine(ClearErrorMessageAfterDelay());
    }

    private IEnumerator ClearErrorMessageAfterDelay()
    {
        yield return new WaitForSeconds(errorMessageDuration);
        ClearErrorMessage();
    }

    private void ClearErrorMessage()
    {
        errorMessageText.text = "";
    }

    public void UpdateSoulsText()
    {
        soulsText.text = $"����: {souls}";
    }

    private void Update()
    {
        if (Time.time >= nextSoulGenerationTime)
        {
            souls++;
            nextSoulGenerationTime = Time.time + soulGenerationRate;
            UpdateSoulsText();  // ���������� ������ ������ ��� ��������� ���������� ���
        }
    }

    private IEnumerator GenerateNegativeEffects()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // ��������� 10 ������

            // ������� ������ � ���������� �������� � ��������� �����
            Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
            Instantiate(negativeEffectPrefab, new Vector2(randomPosition.x,randomPosition.y), Quaternion.identity);
            // ���������� ����� � ���������� ���
            devourerOfSoulsText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f); // ���������� ����� �� 2 ������
            devourerOfSoulsText.gameObject.SetActive(false);
        }
    }
}
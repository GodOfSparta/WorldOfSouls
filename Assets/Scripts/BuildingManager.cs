using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BuildingManager : MonoBehaviour
{
    public int[] buildingCosts = { 10, 20, 30 }; // Стоимости зданий
    public GameObject[] buildingPrefabs; // Префабы зданий
    public GameObject negativeEffectPrefab; // Префаб с негативным эффектом
    public TextMeshProUGUI soulsText; // UI Text для отображения количества душ
    public TextMeshProUGUI errorMessageText; // UI Text для отображения ошибок
    public int souls = 0;
    private float soulGenerationRate = 1f;
    private float nextSoulGenerationTime;
    private float spawnRadius = 2f; // Радиус спавна
    private float errorMessageDuration = 2f; // Продолжительность отображения ошибки
    public AudioClip notEnoughSoulsSound; // Аудиоклип для звука, когда душ недостаточно
    private AudioSource audioSource;
    public TextMeshProUGUI devourerOfSoulsText; // UI Text для отображения появления пожирателя душ
    public GameObject soulsPrefab;
    private bool building0Constructed = false; // Флаг, указывающий, построено ли здание с индексом 0
    public TextMeshProUGUI devourerOfSoulsDeadText; // Добавьте это поле
    public float devourerOfSoulsDeadDuration = 2f;


    private void Start()
    {
        nextSoulGenerationTime = Time.time + soulGenerationRate;
        audioSource = GetComponent<AudioSource>(); // Получите компонент AudioSource из объекта, к которому прикреплен скрипт
        // Начальная инициализация
        UpdateSoulsText();

        // Запуск корутины для создания префабов с негативным эффектом
        StartCoroutine(GenerateNegativeEffects());
        // Запуск корутины для создания префабов Souls при построенном здании с индексом 0
        StartCoroutine(GenerateSoulsPrefab());
        // Подписываемся на событие NegativePrefabDestroyed
        NegativePrefab.NegativePrefabDestroyed += OnNegativePrefabDestroyed;

    }
    private void OnNegativePrefabDestroyed()
    {
        ShowDevourerDestroyedMessage("Пожиратель душ уничтожен!");
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
            yield return new WaitForSeconds(5f); // Подождать 5 секунд

            // Проверяем, построено ли здание с индексом 0
            if (building0Constructed)
            {
                // Создать префаб Souls в случайном месте в пределах экрана
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

            // Получите компонент "Audio Source" для соответствующего здания и воспроизведите аудиоклип
            AudioSource buildingAudio = newBuilding.GetComponent<AudioSource>();
            if (buildingAudio != null)
            {
                buildingAudio.Play();
            }

            // Запустите анимацию
            Animator buildingAnimator = newBuilding.GetComponent<Animator>();
            if (buildingAnimator != null)
            {
                buildingAnimator.SetTrigger("BuildTrigger");
            }

            UpdateSoulsText();
            ClearErrorMessage();
            // Проверка, если построено здание с индексом 0, устанавливаем флаг в true
            if (index == 0)
            {
                building0Constructed = true;
            }

            // Увеличьте скорость появления душ в зависимости от индекса построенного здания
            AdjustSoulGenerationRate(index);
        }
        else
        {
            audioSource.PlayOneShot(notEnoughSoulsSound, 0.5f); // Воспроизвести звук, если душ недостаточно
            ShowErrorMessage("Недостаточно душ для покупки здания!");
        }
    }

    private void AdjustSoulGenerationRate(int index)
    {
        // Увеличьте скорость появления душ в зависимости от индекса построенного здания
        switch (index)
        {
            case 0:
                soulGenerationRate *= 0.9f; // Увеличение на 10%
                break;
            case 1:
                soulGenerationRate *= 0.5f; // Увеличение на 50%
                break;
            case 2:
                soulGenerationRate *= 0.1f; // Увеличение на 90%
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
        soulsText.text = $"Души: {souls}";
    }

    private void Update()
    {
        if (Time.time >= nextSoulGenerationTime)
        {
            souls++;
            nextSoulGenerationTime = Time.time + soulGenerationRate;
            UpdateSoulsText();  // Обновление текста только при изменении количества душ
        }
    }

    private IEnumerator GenerateNegativeEffects()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Подождать 10 секунд

            // Создать префаб с негативным эффектом в случайном месте
            Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
            Instantiate(negativeEffectPrefab, new Vector2(randomPosition.x,randomPosition.y), Quaternion.identity);
            // Отобразить текст о пожирателе душ
            devourerOfSoulsText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f); // Отобразить текст на 2 секунд
            devourerOfSoulsText.gameObject.SetActive(false);
        }
    }
}
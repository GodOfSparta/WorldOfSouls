using System.Collections;
using UnityEngine;

public class Souls : MonoBehaviour
{
    public float speed = 2f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ������ ��������� ����������� ��� ��������� ��������
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.velocity = speed * randomDirection;
    }

    private void Update()
    {
        BounceOffScreenEdges();
    }

    private void BounceOffScreenEdges()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        // ��������� ������������ � ��������� ������
        if (screenPosition.x <= 0 && rb.velocity.x < 0 || screenPosition.x >= Screen.width && rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }

        if (screenPosition.y <= 0 && rb.velocity.y < 0 || screenPosition.y >= Screen.height && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �������� ������������ � ���������� ��������
        if (other.CompareTag("NegativePrefab"))
        {
            Debug.Log("���������� ��� ���������!");
            // ����������� ����������� �������
            Destroy(other.gameObject, 1f);

            // ����������� ����
            Destroy(gameObject);
        }
    }
}

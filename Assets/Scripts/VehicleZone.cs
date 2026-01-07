using UnityEngine;

public class VehicleZone : MonoBehaviour
{
    public Player.Player vehicle; // Транспорт, к которому привязана эта зона
    public float transferTime = 2f; // Время, необходимое для пересадки

    private float stayTimer = 0f; // Таймер нахождения игрока в зоне
    private bool playerInZone = false; // Флаг нахождения игрока в зоне

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Проверяем, что в зону вошёл игрок
        {
            playerInZone = true;
            stayTimer = 0f; // Сбрасываем таймер
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Если игрок вышел из зоны
        {
            playerInZone = false;
            stayTimer = 0f; // Сбрасываем таймер
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (playerInZone)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= transferTime) // Если игрок простоял достаточно времени
            {
                if (other.CompareTag("Player")) // Если игрок вышел из зоны
                {
                    EnterVehicle(other); // Пересаживаемся в транспорт
                }

                playerInZone = false; // Выключаем флаг
            }
        }

    }

    private void EnterVehicle(Collider other)
    {
        Debug.Log($"Player entered {vehicle.name}");

        var player = other.gameObject.GetComponent<Player.Player>();
        LevelManager.Instance.currentVehicle.DisableVechicle();
        if (!vehicle.EnableVechicle())
            player.EnableVechicle();
        if (player.changeEffect != null)
            player.changeEffect.Play();
        stayTimer = 0f; // Сбрасываем таймер
    }
}

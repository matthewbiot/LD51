using UnityEngine;

public class Trap : MonoBehaviour
{
    private Room m_Room;

    public void RegisterRoom(Room room)
    {
        m_Room = room;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || other.isTrigger)
            return;
        if (m_Room == null)
            return;

        m_Room?.Restart();
        GameManager.instance.IncrementDeathCounter();
    }
}

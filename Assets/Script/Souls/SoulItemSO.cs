using UnityEngine;

[CreateAssetMenu(menuName = "Souls/SoulItem")]
public class SoulItemSO : ScriptableObject
{
    public string soulName = "Lost Soul";
    public int soulValue = 10; // Kolik duší hráè získá
    public Sprite icon;
}
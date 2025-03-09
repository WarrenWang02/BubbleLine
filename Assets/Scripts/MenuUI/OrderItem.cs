using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Orders
{
    public GameObject Product; // You can change this to GameObject if needed
    public int RequireAmount;
    public int Price;
    public float Time;
    public float AppearRate; // How much chance by default it will appear
    public int StoredAmount;
}

[CreateAssetMenu(fileName = "OrderItem", menuName = "Game/OrderItem")]
public class OrderItem : ScriptableObject
{
    public List<Orders> orders = new List<Orders>();
}
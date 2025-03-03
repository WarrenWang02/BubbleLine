using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1OrderControl : MonoBehaviour
{
    [SerializeField] private OrderItem currentOrder;
    [SerializeField] private OrderItem Level1Orders;
    [SerializeField] private LevelControl levelControl;

    private float lastCheckTime;
    private int currentPhase = 0;
    private List<Orders> activeOrders = new List<Orders>();

    private void Start()
    {
        lastCheckTime = levelControl.GetLevelTime(); // Get initial level time
        
        // Ensure currentOrder starts empty
        if (currentOrder.orders != null)
        {
            currentOrder.orders.Clear();
        }
        
        AddNextOrder(); // Start with the first order
    }

    private void Update()
    {
        float levelTime = levelControl.GetLevelTime(); // Continuously sync level time

        // Check if we are due to add a new order based on time progression
        if (currentPhase < 3 && (levelTime <= lastCheckTime - 30f) || currentOrder.orders.Count < currentPhase)
        {
            AddNextOrder();
            lastCheckTime = levelTime; // Reset last check time
        }

        // Ensure current order always contains at least 3 elements in later phases
        if (currentPhase >= 3 && currentOrder.orders.Count < 3)
        {
            AddRandomOrder();
        }
    }

    private void AddNextOrder()
    {
        if (currentPhase < Level1Orders.orders.Count)
        {
            currentOrder.orders.Add(Level1Orders.orders[currentPhase]);
            activeOrders.Add(Level1Orders.orders[currentPhase]);
            currentPhase++;
        }
    }

    private void AddRandomOrder()
    {
        if (Level1Orders.orders.Count == 0) return;

        float totalRate = 0f;
        foreach (var order in Level1Orders.orders)
        {
            totalRate += order.AppearRate;
        }

        float randomValue = Random.Range(0f, totalRate);
        float cumulative = 0f;

        foreach (var order in Level1Orders.orders)
        {
            cumulative += order.AppearRate;
            if (randomValue < cumulative)
            {
                currentOrder.orders.Add(order);
                activeOrders.Add(order);
                break;
            }
        }
    }
}

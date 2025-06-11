using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private List<WorkerEquipment> equipmentList = new();

    private void Awake()
    {
        foreach (WorkerEquipment equipment in equipmentList)
        {
            equipment.model.SetActive(equipment.isEquipped);
        }
    }

    public void FullyEquip(bool newState)
    {
        // used to fully equip a worker

        foreach (WorkerEquipment equipment in equipmentList)
        {
            equipment.isEquipped = newState;
            equipment.model.SetActive(newState);
        }
    }

    public void EquipItem(string itemName, bool newState)
    {
        // used to equip an item to the worker and update the model's visibility

        WorkerEquipment foundEquipment = equipmentList.Find(e => e.equipmentName == itemName);

        if (foundEquipment != null)
        {
            Debug.LogError($"trying to equip a {itemName} failed, verify if the name is correct.");
            return;
        }

        Worker workerScript = GetComponent<Worker>();
        if (foundEquipment.assossiatedJob != workerScript.workerType)
        {
            Debug.LogWarning($"{itemName} just equiped to {workerScript.gameObject.name} and it doesn't match it's job.");
        }

        foundEquipment.isEquipped = newState;
        foundEquipment.model.SetActive(newState);
    }
}

[Serializable]
public class WorkerEquipment
{
    public string equipmentName;
    public GameObject model;
    public bool isEquipped;
    public Worker.JOB_TYPE assossiatedJob;
}
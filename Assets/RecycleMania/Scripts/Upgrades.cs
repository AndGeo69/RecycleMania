using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades
{

    public enum UpgradeType {
        CarryingCapacity,
        MovementSpeed,
        PickupRange
        
    }

    public static int GetCost(UpgradeType upgradeType) {
        switch (upgradeType) {
            default:
            case UpgradeType.CarryingCapacity:  return 100;
            case UpgradeType.MovementSpeed:     return 50;
            case UpgradeType.PickupRange:       return 200;
        }
    }

    public static float GetUpgradeValue(UpgradeType upgradeType) {
        switch (upgradeType) {
            default:
            case UpgradeType.CarryingCapacity:  return 10;
            case UpgradeType.MovementSpeed:     return 2f;
            case UpgradeType.PickupRange:       return 1f;
        }
    }

    public static float GetUpgradeMaxValue(UpgradeType upgradeType) {
        switch (upgradeType) {
            default:
            case UpgradeType.CarryingCapacity:  return 200;
            case UpgradeType.MovementSpeed:     return 15f;
            case UpgradeType.PickupRange:       return 5f;
        }
    }

}

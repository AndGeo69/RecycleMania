using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopCustomer
{
  void BoughtUpgrade(Upgrades.UpgradeType upgradeType);
  bool ReachedMaxUpgrade(Upgrades.UpgradeType upgradeType);
  bool TrySpendLeafPointsAmount(int amount);
}

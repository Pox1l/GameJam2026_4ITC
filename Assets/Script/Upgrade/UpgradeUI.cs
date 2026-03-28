using UnityEngine;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [Header("Texty na tlaèítkách")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI multiText;
    public TextMeshProUGUI swordText;
    public TextMeshProUGUI bowText;

    public void UpdateButtons()
    {
        var m = UpgradeManager.Instance;
        if (m == null) return;

        hpText.text = $"Upgrade HP\n{m.CalculateCost(m.data.hpLevel)} Souls\nLvl: {m.data.hpLevel}";
        multiText.text = $"Soul Multi\n{m.CalculateCost(m.data.multiplierLevel)} Souls\nLvl: {m.data.multiplierLevel}";
        swordText.text = $"Sword Dmg\n{m.CalculateCost(m.data.swordLevel)} Souls\nLvl: {m.data.swordLevel}";
        bowText.text = $"Bow Dmg\n{m.CalculateCost(m.data.bowLevel)} Souls\nLvl: {m.data.bowLevel}";
    }

    public void OnClickHP() { UpgradeManager.Instance.UpgradeHP(); UpdateButtons(); }
    public void OnClickMulti() { UpgradeManager.Instance.UpgradeMultiplier(); UpdateButtons(); }
    public void OnClickSword() { UpgradeManager.Instance.UpgradeSword(); UpdateButtons(); }
    public void OnClickBow() { UpgradeManager.Instance.UpgradeBow(); UpdateButtons(); }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WeaponDraw : MonoBehaviour, IItemObserver
{
    [Space(10)]
    [SerializeField] private Canvas weaponDrawUI;
    [SerializeField] private TextMeshProUGUI crystalInfo;
    [SerializeField] private TMP_InputField countInputField;
    [SerializeField] private Button drawButton;
    [SerializeField] private CanvasGroup info;
    [SerializeField] private CanvasGroup viewer;

    [Space(10)]
    [SerializeField] private Transform list;
    [SerializeField] private GameObject slot;

    [Space(10)]
    [SerializeField] private int drawCost;
    
    private List<WeaponDrawSlot> slots = new List<WeaponDrawSlot>();
    private PlayerCTRL playerCTRL;
    private Animator animator;

    private int drawCount = 0;
    private long crystalCount = 0;

    private CancellationTokenSource drawToken = new CancellationTokenSource();

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerCTRL = FindObjectOfType<PlayerCTRL>();
        Item crystal = (Item)playerCTRL["Crystal"];
        crystal.Subscribe(this);
        ItemUpdate(crystal);

        crystalInfo.text = $"1회에 {drawCost} 크리스탈";

        foreach (var item in playerCTRL.weapons) {
            WeaponDrawSlot weaponDrawSlot = Instantiate(slot, list).GetComponent<WeaponDrawSlot>();
            weaponDrawSlot.SetSlot(item);
            slots.Add(weaponDrawSlot);
        }
    }

    public void Enable()
    {
        GameManager.gm.PauseGame();
        weaponDrawUI.enabled = true;
    }

    public void DIsable()
    {
        GameManager.gm.ResumeGame();
        weaponDrawUI.enabled = false;
        CancelViewer();
    }

    public void CloseViewer()
    {
        CancelViewer();
        TapChange(true);

        foreach (var slot in slots) {
            slot.ResetSlot();
        }
    }

    private void CancelViewer()
    {
        drawToken.Cancel();
        drawToken = new CancellationTokenSource();
    }

    private void TapChange(bool infoVisible)
    {
        info.alpha = infoVisible ? 1 : 0;
        info.blocksRaycasts = infoVisible;

        viewer.alpha = infoVisible ? 0 : 1;
        viewer.blocksRaycasts = !infoVisible;
    }

    public void ItemUpdate(Item item)
    {
        crystalCount = item.count;
        drawButton.interactable = drawCount * drawCost <= crystalCount;
    }

    public void SetDrawCount()
    {
        try {
            drawCount = int.Parse(countInputField.text);
        }
        catch (System.FormatException) {
            drawButton.interactable = false;
        }

        drawButton.interactable = drawCount * drawCost <= crystalCount;
    }

    public void DrawWeapon()
    {
        if (drawCount == 0 || drawCount * drawCost > crystalCount) {
            return;
        }

        animator.SetTrigger("OnWeaponDraw");
    }

    public async void OnDrawWeapon()
    {
        animator.ResetTrigger("OnWeaponDraw");

        foreach (var item in slots) {
            item.ResetSlot();
        }

        TapChange(false);

        for (int i = 0; i < drawCount; i++) {
            WeaponRate weaponRate = (WeaponRate)ChooseRate();
            Weapon weapon = ChooseWeapon(weaponRate);
            playerCTRL.AddWeapon(weapon.itemName);

            foreach (var slot in slots) {
                if (weapon.itemName == slot.weaponName) {
                    slot.CountUp();
                }
            }
        }

        Item cryStal = (Item)playerCTRL["Crystal"];
        cryStal.Count -= drawCount * drawCost;

        foreach (var slot in slots) {
            if (slot.count > 0) {
                slot.gameObject.SetActive(true);

                try {
                    await Task.Delay(300, drawToken.Token);
                }
                catch (TaskCanceledException) {
                    return;
                }
            }
        }
    }

    [ContextMenu("Test Draw")]
    private void TestDraw()
    {
        playerCTRL = FindObjectOfType<PlayerCTRL>();
        for (int i = 0; i < 10000; i++) {
            WeaponRate weaponRate = (WeaponRate)ChooseRate();
            print(ChooseWeapon(weaponRate).weaponTitle);
        }
    }

    private int ChooseRate()
    {
        float randomPoint = Random.value * 100f;
        float[] probs = { 3, 6.2f, 8.5f, 10f, 15f, 20f, 40f };

        for (int i = 0; i < probs.Length; i++) {
            if (randomPoint < probs[i]) {
                return i;
            }
            else {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    public Weapon ChooseWeapon(WeaponRate weaponRate)
    {
        List<Weapon> weapons = new List<Weapon>();
        foreach (var weapon in playerCTRL.weapons) {
            if (weapon.weaponRate == weaponRate) {
                weapons.Add(weapon);
            }
        }

        return weapons[Random.Range(0, weapons.Count - 1)];
    }
}

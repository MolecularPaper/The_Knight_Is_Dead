using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class WeaponDraw : MonoBehaviour, IItemObserver
{

    [Space(10)]
    [SerializeField] private TMP_InputField countInputField;
    [SerializeField] private Button drawButton;
    [SerializeField] private CanvasGroup info;
    [SerializeField] private CanvasGroup viewer;

    [Space(10)]
    [SerializeField] private Transform list;
    [SerializeField] private GameObject slot;
    
    private List<WeaponDrawSlot> slots = new List<WeaponDrawSlot>();
    private PlayerCTRL playerCTRL;
    private Animator animator;

    private int drawCount = 0;
    private ulong crystalCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerCTRL = FindObjectOfType<PlayerCTRL>();
        Item crystal = (Item)playerCTRL["Crystal"];
        crystal.Subscribe(this);
        ItemUpdate(crystal);

        foreach (var item in playerCTRL.weapons) {
            WeaponDrawSlot weaponDrawSlot = Instantiate(slot, list).GetComponent<WeaponDrawSlot>();
            weaponDrawSlot.SetSlot(item);
            slots.Add(weaponDrawSlot);
        }
    }

    public void ItemUpdate(Item item)
    {
        crystalCount = item.count;
        drawButton.interactable = (ulong)(drawCount * 100) <= crystalCount;
    }

    public void SetDrawCount()
    {
        drawCount = int.Parse(countInputField.text);
        drawButton.interactable = (ulong)(drawCount * 100) <= crystalCount;
    }

    public void DrawWeapon()
    {
        if (drawCount == 0 || (ulong)(drawCount * 100) > crystalCount) {
            return;
        }

        animator.SetTrigger("OnDrawWeapon");

        print("Test");
    }

    public async void OnDrawWeapon()
    {
        foreach (var item in slots) {
            item.ResetSlot();
        }

        info.alpha = 0;
        info.blocksRaycasts = false;

        viewer.alpha = 1;
        viewer.blocksRaycasts = true;

        List<Weapon> drawedWeapons = new List<Weapon>();
        for (int i = 0; i < drawCount; i++) {
            WeaponRate weaponRate = (WeaponRate)ChooseRate();
            Weapon weapon = ChooseWeapon(weaponRate);
            playerCTRL.AddWeapon(weapon.itemName);
            drawedWeapons.Add(weapon);
        }

        foreach (var drawed in drawedWeapons) {
            foreach (var slot in slots) {
                if(drawed.itemName == slot.weaponName) {
                    slot.CountUp();
                    await Task.Delay(100);
                }
            }
        }
    }

    [ContextMenu("Test Draw")]
    public void TestDraw()
    {
        playerCTRL = FindObjectOfType<PlayerCTRL>();
        for (int i = 0; i < 10000; i++) {
            WeaponRate weaponRate = (WeaponRate)ChooseRate();
            print(ChooseWeapon(weaponRate));
        }
    }

    public int ChooseRate()
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

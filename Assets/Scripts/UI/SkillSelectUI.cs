using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    [SerializeField] private GameObject slot;
    [SerializeField] private Image slotIcon;

    private bool isEnabled;

    public SkillSlot(GameObject slotObject)
    {
        slot = slotObject;
        slotIcon = slotObject.transform.GetChild(0).GetComponent<Image>();
    }

    public async void Enabled(Skill skill) 
    {
        isEnabled = true;
        slotIcon.sprite = skill.icon;
        slotIcon.gameObject.SetActive(true);

        PlayerCTRL playerCTRL = GameObject.FindObjectOfType<PlayerCTRL>();
        while (isEnabled) {
            EnemyCTRL enemyCTRL = null;
            if (enemyCTRL == null) {
                GameObject.FindObjectOfType<EnemyCTRL>();
                try {
                    await GameManager.gm.Delay(100);
                }
                catch (TaskCanceledException) {
                    return;
                }
            }

            skill.Execute(playerCTRL, enemyCTRL);

            try {
                await GameManager.gm.Delay((int)(skill.coolTime * 1000f));
            }
            catch (TaskCanceledException) {
                return;
            }
        }
    }

    public void Disable()
    {
        isEnabled = false;
        slotIcon.sprite = null;
        slotIcon.gameObject.SetActive(false);
        slot.transform.SetAsLastSibling();
    }
}

public class SkillSelectUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup skillTapBlock;
    [SerializeField] private CanvasGroup selectBarBlock;

    private List<SkillSlot> slots = new List<SkillSlot>();
    private Skill skillTemp;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);

            SkillSlot skillSlot = new SkillSlot(child.gameObject);
            slots.Add(skillSlot);

            Button button = child.GetComponent<Button>();
            button.onClick.AddListener(() => {
                SelectSlot(i);
            });
        }
    }

    public void SelectSkill(string skillName)
    {
        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        skillTemp = (Skill)playerCTRL[skillName];

        skillTapBlock.alpha = 1.0f;
        skillTapBlock.blocksRaycasts = true;

        selectBarBlock.blocksRaycasts = true;
    }

    public void SelectSlot(int slotIndex)
    {
        slots[slotIndex].Enabled(skillTemp);
        skillTemp = null;

        skillTapBlock.alpha = 0.0f;
        skillTapBlock.blocksRaycasts = false;

        selectBarBlock.blocksRaycasts = false;
    }
}

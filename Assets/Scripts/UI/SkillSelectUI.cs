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

    public Skill skill;

    public SkillSlot(GameObject slotObject)
    {
        slot = slotObject;
        slotIcon = slotObject.transform.GetChild(0).GetComponent<Image>();
    }

    public async void Enabled(Skill skill) 
    {
        this.skill = skill;
        skill.isEnabled = true;

        slotIcon.sprite = skill.icon;
        slotIcon.gameObject.SetActive(true);

        PlayerCTRL playerCTRL = GameObject.FindObjectOfType<PlayerCTRL>();
        while (skill.isEnabled) {
            EnemyCTRL enemyCTRL = null;
            while (enemyCTRL == null) {
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
        skill.isEnabled = false;
        skill = null;
        slotIcon.sprite = null;
        slotIcon.gameObject.SetActive(false);
        slot.transform.SetAsLastSibling();
    }
}

public class SkillSelectUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup skillTapBlock;
    [SerializeField] private CanvasGroup selectBarBlock;
    [SerializeField] private Transform skillSlots;

    private List<SkillSlot> slots = new List<SkillSlot>();
    private Skill skillTemp;

    private void Start()
    {
        for (int i = 0; i < skillSlots.childCount; i++) {
            Transform child = skillSlots.GetChild(i);

            SkillSlot skillSlot = new SkillSlot(child.gameObject);
            slots.Add(skillSlot);

            Button button = child.GetComponent<Button>();
            button.onClick.AddListener(() => {
                SelectSlot(i);
            });
        }

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        foreach (var skill in playerCTRL.skills) {
            if (skill.isEnabled) {
                slots[skill.slotIndex].Enabled(skill);
            }
        }
    }

    public void SelectSkill(string skillName)
    {
        foreach (SkillSlot skillSlot in slots) {
            if (skillSlot.skill != null && skillSlot.skill.skillName == skillName) {
                skillSlot.Disable();
                return;
            }
        }

        PlayerCTRL playerCTRL = FindObjectOfType<PlayerCTRL>();
        skillTemp = (Skill)playerCTRL[skillName];
        
        if(skillTemp.level == 0) {
            return;
        }

        skillTapBlock.alpha = 1.0f;
        skillTapBlock.blocksRaycasts = true;
        selectBarBlock.blocksRaycasts = true;
    }

    public void SelectSlot(int slotIndex)
    {
        skillTemp.slotIndex = slotIndex;
        slots[slotIndex].Enabled(skillTemp);
        skillTemp = null;

        skillTapBlock.alpha = 0.0f;
        skillTapBlock.blocksRaycasts = false;
        selectBarBlock.blocksRaycasts = false;
    }
}

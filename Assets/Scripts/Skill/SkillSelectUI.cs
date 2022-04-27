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

    public Skill skill = null;
    public int slotIndex;

    public SkillSlot(GameObject slotObject)
    {
        slot = slotObject;
        slotIcon = slotObject.transform.GetChild(0).GetComponent<Image>();
    }

    public async void Enabled(Skill skill) 
    {
        this.skill = skill;
        skill.isEnabled = true;

        slotIcon.sprite = skill.skillicon;
        slotIcon.gameObject.SetActive(true);

        PlayerCTRL playerCTRL = GameObject.FindObjectOfType<PlayerCTRL>();
        while (skill.isEnabled) {
            EnemyCTRL enemyCTRL = null;
            while (playerCTRL.IsMove || enemyCTRL == null) {
                enemyCTRL = GameObject.FindObjectOfType<EnemyCTRL>();
                try {
                    await GameManager.gm.Delay(100);
                }
                catch (TaskCanceledException) {
                    return;
                }
            }

            try {
                await GameManager.gm.Delay(200 * slotIndex);
            }
            catch (TaskCanceledException) {
                return;
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
    [SerializeField] private List<GameObject> skillSlots;

    private List<SkillSlot> slots = new List<SkillSlot>();
    private Skill skillTemp;

    private void Start()
    {
        for (int i = 0; i < skillSlots.Count; i++) {
            SkillSlot skillSlot = new SkillSlot(skillSlots[i]);
            skillSlot.slotIndex = i;

            slots.Add(skillSlot);

            Button button = skillSlots[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                SelectSlot(skillSlot.slotIndex);
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

        skillTapBlock.alpha = 1.0f;
        skillTapBlock.blocksRaycasts = true;
        selectBarBlock.blocksRaycasts = true;
    }

    public void SelectSlot(int slotIndex)
    {
        if(slots[slotIndex].skill != null) {
            slots[slotIndex].skill = null;
        }

        slots[slotIndex].Enabled(skillTemp);
        skillTemp.slotIndex = slotIndex;

        skillTapBlock.alpha = 0.0f;
        skillTapBlock.blocksRaycasts = false;
        selectBarBlock.blocksRaycasts = false;
    }
}

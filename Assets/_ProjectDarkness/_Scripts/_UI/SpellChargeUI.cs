using UnityEngine;
using UnityEngine.UI;

namespace ProjectDarkness
{
    public class SpellChargeUI : MonoBehaviour
    {
        [SerializeField] 
        private Image _spellChargeImageUI;
        
        private void Start()
        {
            HideSpellChargeUI();
        }
        
        private void Update()
        {
            if(WandManager.Instance.CurrentWand == null)
            {
                return;
            }
            
            if(WandManager.Instance.CurrentWand.SpellChargeTimer.IsRunning())
            {
                ShowSpellChargeUI();
                
                float fillAmount = WandManager.Instance.CurrentWand.SpellChargeTimer.GetPercentComplete();
                _spellChargeImageUI.fillAmount = fillAmount;
            }
            else
            {
                HideSpellChargeUI();
            }
        }
        
        private void ShowSpellChargeUI()
        {
            if(_spellChargeImageUI.gameObject.activeInHierarchy) return;
        
            _spellChargeImageUI.gameObject.SetActive(true);
        }
        
        private void HideSpellChargeUI()
        {
            if(!_spellChargeImageUI.gameObject.activeInHierarchy) return;
        
            _spellChargeImageUI.gameObject.SetActive(false);
        }
    }
}

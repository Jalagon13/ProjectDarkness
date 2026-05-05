using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ProjectDarkness
{
    public class TransitionPanelUI : MonoBehaviour
    {
        [SerializeField] private Image _transitionPanel;
        
        public Tween FadeToBlack(float duration)
        {
            return _transitionPanel.DOFade(1f, duration);
        }

        public Tween FadeToClear(float duration)
        {
            return _transitionPanel.DOFade(0f, duration);
        }
    }
}

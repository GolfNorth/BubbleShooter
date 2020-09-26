using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BubbleShooter
{
    public class HyperlinkText : MonoBehaviour, IPointerClickHandler
    {
        #region Fields

        private TMP_Text _tmpText;

        #endregion

        #region Methods

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex =
                TMP_TextUtilities.FindIntersectingLink(_tmpText, eventData.position, eventData.pressEventCamera);

            if (linkIndex != -1)
            {
                var linkInfo = _tmpText.textInfo.linkInfo[linkIndex];

                Application.OpenURL(linkInfo.GetLinkID());
            }
        }

        #endregion
    }
}
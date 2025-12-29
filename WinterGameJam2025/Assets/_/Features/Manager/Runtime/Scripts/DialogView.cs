
using UnityEngine;
using Foundation.Runtime;
using TMPro;

namespace Manager.Runtime
{
    public class DialogView : FBehaviour
    {
         #region Public

   
        #endregion

        #region Unity API

   
        #endregion

        #region Main Methods

        public void SetDialog(string dialogText)
        {
            _fullText = dialogText;
            _totalCharacters = _fullText.Length;
            _maxVisibleCharacters = 0;
            
            _text.text = _fullText;
            _text.maxVisibleCharacters = 0;
            // _currentRevealProgress = 0f;
            _canvasGroup.alpha = 1f;
            
            //gameObject.SetActive(true);
        }

        public void SetRevealProgress(float progress)
        {
            // _currentRevealProgress = Mathf.Clamp01(progress);
            if (progress > .99f) progress = 1;
            
            progress = Mathf.Clamp01(progress);

            int targetVisible = Mathf.FloorToInt(_totalCharacters * progress);
            
            targetVisible = Mathf.Clamp(targetVisible, 0, _totalCharacters);
            
            _maxVisibleCharacters = Mathf.Max(_maxVisibleCharacters, targetVisible);
            
            // _text.text = _fullText.Substring(0, _maxVisibleCharacters);
            _text.maxVisibleCharacters = _maxVisibleCharacters;
        }

        public void SetOpacity(float opacity)
        {
            _currentOpacity = Mathf.Clamp01(opacity);
            _canvasGroup.alpha = _currentOpacity;
            
            var color = _text.color;
            color.a = opacity;
            _text.color = color;
            
            if(opacity <= 0.05f)
                _text.text = string.Empty;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            // _text.text = string.Empty;
            _text.maxVisibleCharacters = 0;
            //gameObject.SetActive(false);
        }
        #endregion

        #region Utils

   
        #endregion

        #region Private & Protected

        [Header("References")] 
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        private string _fullText;
        private int _totalCharacters;
        private int _maxVisibleCharacters;

        private float _currentRevealProgress;
        private float _currentOpacity = 1f;

        #endregion
    }
}
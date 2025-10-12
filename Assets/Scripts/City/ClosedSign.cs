using System;
using System.Collections;
using Helpers;
using TMPro;
using Types;
using UnityEngine;

namespace City
{
    public class ClosedSign : MonoBehaviour
    {
        [SerializeField] private float signScaleTime = 0.25f;
        [SerializeField] private float typeSpeed = 0.02f;
        [SerializeField, TextArea] private string signText = "Sorry, we're closed!\nOpen from {openFrom} to {openTo}.";

        private TMP_Text _text;

        public event Action OnSignClosed;

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }

        public void Show(string openFrom, string openTo)
        {
            StartCoroutine(ShowSign(openFrom, openTo));
            StartCoroutine(HideOnMouseClick());
        }

        private IEnumerator ShowSign(string openFrom, string openTo)
        {
            transform.localScale = Vector3.zero;
            yield return transform.Scale(Vector3.one, signScaleTime);
            yield return _text.TypeMessage(signText.Interpolate(openFrom, openTo));
        }

        private IEnumerator HideOnMouseClick()
        {
            yield return FunctionLibrary.WaitForMouseClick();
            OnSignClosed?.Invoke();
            Destroy(gameObject);
        }
    }
}

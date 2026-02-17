using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TracingApp.Core;

namespace TracingApp.UI
{
    /// <summary>
    /// Handles the User ID entry screen.
    /// Validates format: 3 letters + 3 numbers (e.g., "ABC123").
    /// </summary>
    public class UserIDEntry : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_InputField UserIDInput;
        public Button SubmitButton;
        public Button SkipButton;
        public TextMeshProUGUI ErrorText;
        public TextMeshProUGUI TitleText;

        [Header("Navigation")]
        public bool GoToSettingsOnSubmit = true;

        private void Start()
        {
            if (SubmitButton != null)
                SubmitButton.onClick.AddListener(OnSubmit);

            if (SkipButton != null)
                SkipButton.onClick.AddListener(OnSkip);

            if (UserIDInput != null)
            {
                UserIDInput.characterLimit = 6;
                UserIDInput.onValueChanged.AddListener(OnInputChanged);
            }

            if (ErrorText != null)
                ErrorText.gameObject.SetActive(false);
        }

        private void OnInputChanged(string value)
        {
            // Force uppercase
            if (UserIDInput != null)
            {
                UserIDInput.text = value.ToUpper();
            }

            if (ErrorText != null)
                ErrorText.gameObject.SetActive(false);
        }

        private void OnSubmit()
        {
            string input = UserIDInput != null ? UserIDInput.text.Trim().ToUpper() : "";

            if (ValidateUserID(input))
            {
                GameManager.Instance?.SetUserID(input);
                NavigateNext();
            }
            else
            {
                ShowError("Please enter 3 letters followed by 3 numbers (e.g., ABC123)");
            }
        }

        private void OnSkip()
        {
            GameManager.Instance?.SetUserID("Anon");
            NavigateNext();
        }

        /// <summary>
        /// Validate that the user ID is 3 letters + 3 numbers.
        /// </summary>
        private bool ValidateUserID(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 6)
                return false;

            for (int i = 0; i < 3; i++)
            {
                if (!char.IsLetter(id[i]))
                    return false;
            }

            for (int i = 3; i < 6; i++)
            {
                if (!char.IsDigit(id[i]))
                    return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            if (ErrorText != null)
            {
                ErrorText.text = message;
                ErrorText.gameObject.SetActive(true);
            }
        }

        private void NavigateNext()
        {
            if (GoToSettingsOnSubmit)
                GameManager.Instance?.LoadSettings();
        }
    }
}

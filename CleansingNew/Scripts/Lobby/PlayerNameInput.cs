using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TheCleansing.Lobby
{
    public class PlayerNameInput : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField nameInputField = null;
        [SerializeField] private Button continueButton = null;
        public static string DisplayName { get; private set; }          //allows the display name to be returned but not set

        private const string PlayerPrefsNameKey = "PlayerName";          //saves the nickname of players, so they don't have to retype


        // Start is called before the first frame update
        private void Start() => SetUpInputField();

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }           //if player doesn't have a name, don't do anything

            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

            nameInputField.text = defaultName;

            SetPlayerName(defaultName);
        }

        public void SetPlayerName(string name)
        {
            continueButton.interactable = nameInputField.text.Length >= 3;               //button only activiates once the name field is written
        }

        public void SavePlayerName()
        {
            DisplayName = nameInputField.text;                                    //saves nickname to server and player-pref

            PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
        }

    }
}

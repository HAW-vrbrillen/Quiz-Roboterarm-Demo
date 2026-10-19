using UnityEngine;
using Meta.WitAi.TTS.Utilities;

namespace RIS
{
    public class TTSSpeakerInput : MonoBehaviour
    {
        public string readme = "Hallo";
        public TextPoolRIS textPoolRIS; // Referenz zum TextPoolRIS

        [SerializeField] private TTSSpeaker _speaker;
        [SerializeField]
        private string _voice;
        public bool speakFlag = false;

        private void Start()
        {
            if (speakFlag) {
                _speaker.Speak(readme);
            }
        }

        // Methode zum Sprechen des Textes basierend auf textId oder direktem String
        public void SpeakIt(string textId)
        {
            // Stoppt alle laufenden Sprechvorg�nge
            _speaker.Stop();

            // Holt den Text aus dem Dictionary, falls es einen Key gibt
            string phrase = textPoolRIS.GetTextById(textId);

            // Wenn der Key nicht existiert, wird der direkte Text verwendet
            if (string.IsNullOrEmpty(phrase))
            {
                phrase = textId; // Wenn kein Dictionary-Eintrag gefunden wurde, wird der eingegebene Text direkt gesprochen
            }

            // Spricht den Text
            _speaker.Speak(phrase);
        }

        // OnEnable und OnDisable bleiben unver�ndert, um sicherzustellen, dass andere Funktionen weiterhin funktionieren
        private void OnEnable()
        {
            _voice = _speaker.VoiceID;
        }

        private void OnDisable()
        {
        }

        private void Update()
        {
            if (!string.Equals(_voice, _speaker.VoiceID))
            {
                _voice = _speaker.VoiceID;
            }
        }
    }
}

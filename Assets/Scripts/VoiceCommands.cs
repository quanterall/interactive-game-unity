using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceCommands : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Start is called before the first frame update
    void Start()
    {

        keywords.Add("go", () =>
        {
            JumpCalled();
        });
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += KeywordRecogznierOnPhraseRecognized;
        keywordRecognizer.Start();
    }
    void JumpCalled()
    {
       print("logSpeechJump");
    }

    void KeywordRecogznierOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("We recognized \"" + "\" with confidence " + args.confidence);

        System.Action keyWordAction;

        if (keywords.TryGetValue(args.text, out keyWordAction))
        {
            keyWordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

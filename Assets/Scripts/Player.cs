using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
public class Player : MonoBehaviour
{
    private bool jumpKeyWasPressed;
    private float horizontalInput;
    private Rigidbody rigidBodyComponent;
    public Transform groundCheckTransform;

    // Speach Recognition
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start()
    {
        keywords.Add("jump", () =>
        {
            JumpCalled();
        });
        keywords.Add("go", () =>
        {
            GoCalled();
        });
        keywords.Add("stop", () =>
        {
            StopCalled();
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += KeywordRecogznierOnPhraseRecognized;
        keywordRecognizer.Start();

        rigidBodyComponent = GetComponent<Rigidbody>();
    }

    public Transform targetPos;
    void GoCalled()
    {
        rigidBodyComponent.AddForce(Vector3.MoveTowards(transform.position, targetPos.position, 0.5f));

        jumpKeyWasPressed = false;
        print("logSpeechMove");
    }
    void StopCalled()
    {
        rigidBodyComponent.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
        jumpKeyWasPressed = false;
        print("logSpeechJump");
    }
    void JumpCalled()
    {
        rigidBodyComponent.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
        jumpKeyWasPressed = false;
        print("logSpeechJump");
    }
    void KeywordRecogznierOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("We recognized \""+ args.text + "\" with confidence " + args.confidence);

        System.Action keyWordAction;

        if (keywords.TryGetValue(args.text, out keyWordAction))
        {
            keyWordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyWasPressed = true;
            // Debug.Log("logSpaceIsPressed");
        }
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidBodyComponent.velocity = new Vector3(horizontalInput, rigidBodyComponent.velocity.y, 0);

        if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f).Length == 1)
        {
            return;
        }

        if (jumpKeyWasPressed)
        {
            rigidBodyComponent.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
            jumpKeyWasPressed = false;
            Debug.Log("logSpaceIsPressed");
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0F))
        {
            float distanceToGround = hit.distance;
           // print(distanceToGround);

            if (distanceToGround <= 0.5 && (gameObject.layer == 8))
            {
                GoCalled();
                print("DISTANCE HERER");
            }

        }
        else {
        }
    }
    private void onCollisionEnter(Collision collison)
    {
        print(gameObject.layer);
    }

    private void onCollisionExit(Collision collison)
    {
    }

    public LayerMask layerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Destroy(other.gameObject);
        }
        if ((layerMask.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            JumpCalled();
            Debug.Log("Hit with Layermask");
        }
        else
        {
            Debug.Log("Not in Layermask");
        }
    }
}
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
    private Animator animator;

    // Speech Recognition
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // whether the player is currently going, or not
    private bool isGoing = false;
    private int lastHDir = 1;  // -1=left, 1=right


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
        animator = GetComponentInChildren<Animator>();
    }

    void OnDestroy()
    {
        keywordRecognizer.Stop();
        keywordRecognizer.Dispose();
        keywordRecognizer = null;
    }

    //public Transform targetPos;
    void GoCalled()
    {
        isGoing = true;
        //print("GO!");
    }

    void StopCalled()
    {
        isGoing = false;
        //print("STOP!");
    }

    void JumpCalled()
    {
        //rigidBodyComponent.AddForce(Vector3.up * 7, ForceMode.VelocityChange);
        jumpKeyWasPressed = true;
        //print("JUMP!");
    }

    void KeywordRecogznierOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Recognized \""+ args.text + "\" with confidence " + args.confidence);

        System.Action keyWordAction;

        if (keywords.TryGetValue(args.text, out keyWordAction))
        {
            keyWordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpKeyWasPressed = true;
            // Debug.Log("logSpaceIsPressed");
        }

        horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput > 0f)
        {
            lastHDir = 1;
            isGoing = false;
        }
        else if(horizontalInput < 0f)
        {
            lastHDir = -1;
            isGoing = false;
        }

        // set walking speed
        float speed = !isGoing ? horizontalInput : lastHDir;
        animator.SetFloat("speedv", Mathf.Abs(speed));

        // player rotation
        if (speed > 0.5f)
        {
            animator.transform.rotation = Quaternion.LookRotation(Vector3.right);
        }
        if (speed < -0.5f)
        {
            animator.transform.rotation = Quaternion.LookRotation(Vector3.left);
        }
    }

    private void FixedUpdate()
    {
        rigidBodyComponent.velocity = new Vector3(!isGoing ? horizontalInput : lastHDir, rigidBodyComponent.velocity.y, 0);

        if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f).Length == 1)
        {
            return;
        }

        if (jumpKeyWasPressed)
        {
            rigidBodyComponent.AddForce(Vector3.up * 7, ForceMode.VelocityChange);

            jumpKeyWasPressed = false;
            //Debug.Log("Started jumping...");
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0F))
        {
            float distanceToGround = hit.distance;
           // print(distanceToGround);

            if (distanceToGround <= 0.5 && (gameObject.layer == 8))
            {
                GoCalled();
                Debug.Log("DISTANCE HERER");
            }

        }
    }

    //private void onCollisionEnter(Collision collison)
    //{
    //    print(gameObject.layer);
    //}

    //private void onCollisionExit(Collision collison)
    //{
    //}

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
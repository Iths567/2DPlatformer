using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{

    Rigidbody2D rigidBody;
    public float speed = 5.0f;
    public float jumpForce = 8.0f;
    public float airControlForce = 10.0f;
    public float airControlMax = 1.5f;
    Vector2 boxExtents;
    Animator animator;
    public AudioSource coinSound;
    public TextMeshProUGUI uiText;
    int totalCoins;
    int coinsCollected;
    string curLevel;
    string nextLevel;
    public TextMeshProUGUI finText;
    public AudioSource winSound;
    public AudioSource deathSound;
    public TextMeshProUGUI deathText;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxExtents = GetComponent<BoxCollider2D>().bounds.extents;

        animator = GetComponent<Animator>();

        coinsCollected = 0;
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        curLevel = SceneManager.GetActiveScene().name;
        if (curLevel == "Level1")
            nextLevel = "Level2";
        else if (curLevel == "Level2")
            nextLevel = "Finished";

        finText.gameObject.SetActive(false);
        deathText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (rigidBody.velocity.x * transform.localScale.x < 0.0f)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        float xSpeed = Mathf.Abs(rigidBody.velocity.x);
        animator.SetFloat("xspeed", xSpeed);

        float ySpeed = Mathf.Abs(rigidBody.velocity.y);
        animator.SetFloat("yspeed", ySpeed);

        float blinkVal = Random.Range(0.0f, 5000.0f);
        if (blinkVal < 1.0f)
        {
            animator.SetTrigger("blinkTrigger");
        }
        string uiString = "x " + coinsCollected + "/" + totalCoins;
        uiText.text = uiString;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        // check if we are on the ground
        Vector2 bottom =
        new Vector2(transform.position.x, transform.position.y - boxExtents.y);

        Vector2 hitBoxSize = new Vector2(boxExtents.x * 2.0f, 0.05f);

        RaycastHit2D result = Physics2D.BoxCast(bottom, hitBoxSize, 0.0f,
        new Vector3(0.0f, -1.0f), 0.0f, 1 << LayerMask.NameToLayer("Ground"));

        bool grounded = result.collider != null && result.normal.y > 0.9f;
        if (grounded)
        {
            if (Input.GetAxis("Jump") > 0.0f)
                rigidBody.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
            else
                rigidBody.velocity = new Vector2(speed * h, rigidBody.velocity.y);
        }
        else
        {
            float vx = rigidBody.velocity.x;
            if (h * vx < airControlMax)
                rigidBody.AddForce(new Vector2(h * airControlForce, 0));
        }
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Coin")
        {
            Destroy(coll.gameObject);
            coinSound.Play();
            coinsCollected++;
        }

        if (coll.gameObject.tag == "LevelEnd")
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            coll.gameObject.SetActive(false);
            StartCoroutine(LoadNextLevel());
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Death")
        {
            StartCoroutine(DoDeath());
        }

    }

    IEnumerator DoDeath()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        
        GetComponent<Renderer>().enabled = false;
        deathText.gameObject.SetActive(true);
        deathSound.Play();

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(curLevel);
    }

    IEnumerator LoadNextLevel()
    {
        if (nextLevel != "Finished")
        {
            GetComponent<Renderer>().enabled = false;
            winSound.Play();
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            winSound.Play();
            GetComponent<Renderer>().enabled = false;
            finText.gameObject.SetActive(true);

        }
    }
}

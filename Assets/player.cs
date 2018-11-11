using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour {
    private CharacterController cont;
    private float speed = 8.0f;
    private bool movingCamera = false;
    private Vector3 mv = Vector3.zero;
    private float gravity = 33.0f;
    public Camera gc;
    private Animator anim;
    public AudioSource oof;
    public Text speedText;
    private Quaternion rev;
    private Quaternion fow;
    private float factor = 1.0f;
    public RawImage img;
    private float dark = 1.0f;

    public void setFactor(float x)
    {
        factor = x;
    }
	// Use this for initialization
	void Start () {
        cont = this.GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        fow = Quaternion.Euler(gc.transform.rotation.x, 0f, gc.transform.rotation.z);
        rev = Quaternion.Euler(gc.transform.rotation.x, 165f, gc.transform.rotation.z);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "smf" || hit.gameObject.tag == "crawl guy")
        {
            hit.transform.parent.GetComponent<gameController>().removeZombie(hit.gameObject);
            if (!(hit.gameObject.tag == "crawl guy" && GetComponent<CharacterController>().velocity.y < -.1)) {
                oof.Play();
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1.0f);
                StartCoroutine(FadeImage());
                //speed = 0.7f * speed;
                dark *= 1.5f;
                RenderSettings.fogDensity = .0625f * dark;
            }
        }
    }
    IEnumerator FadeImage()
    {
        // fade from opaque to transparent
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(img.color.r, img.color.g, img.color.b, i);
                yield return null;
            }
    }

    // Update is called once per frame
    void Update () {
        if (transform.position.z > 70.0f)
        {
            //speed += .005f;
            //if (factor < 0.5)
            //    speed += .005f;
            //else
            //    speed -= .001f;
            speed = (2.0f - factor) * 10f;
        }
        speedText.text = "Distance: " + ((int)transform.position.z).ToString();
        //speed += (0.5f - factor)*.01f; //
        //speed = (2.0f - factor) * 10f; // .35  
        if (cont.transform.position.y < -2.0f)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload scene
            SceneManager.LoadScene(sceneName: "Menu 3D"); // go to menu
        }
        mv.x = Input.GetAxis("Horizontal") * 12.0f;
        if (Input.GetKey(KeyCode.S))
        {
            gc.transform.rotation = Quaternion.Lerp(gc.transform.rotation, rev, Time.deltaTime * 10);
            //gc.transform.rotation = rev;
        }
        else
        {
            gc.transform.rotation = Quaternion.Lerp(gc.transform.rotation, fow, Time.deltaTime * 10);
        }
        if (Input.GetKey(KeyCode.A))
        {
            anim.Play("RUN00_L");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anim.Play("RUN00_R");
        }
        else
        {
            anim.Play("RUN00_F");
        }
        /*
         * 
         *      if (controller.isGrounded && Input.GetButton("Jump")) {
         moveDirection.y = jumpSpeed;
     }
         * /*/
        if (cont.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                mv.y = 7.0f;
            }
        }
        else
        {
            mv.y -= gravity * Time.deltaTime;
        }
        mv.z = speed;
        cont.Move((mv)* Time.deltaTime);
	}
}

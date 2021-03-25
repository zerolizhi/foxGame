using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Collider2D coll;
    public Collider2D DisColl;
    public Transform CellingCheck,GroundCheck;
    //public AudioSource jumpAudio,hurtAudio,cherryAudio;

    [Space]
    public float speed;
    public float JumpForce;
    [Space]
    public LayerMask ground;
    [SerializeField]
    private int Cherry,Gem;
    public TextMeshProUGUI CherryNum;
    private bool isHurt;//默认是false
    private bool isGround;
    private int extraJump;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

   
    void FixedUpdate()
    {
        if (!isHurt)
        {
            Movement();
        }
        SwitchAnim();
        isGround = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, ground);
    }

    private void Update()
    {
        //Jump();
        Crouch();
        CherryNum.text = Cherry.ToString();
        newJump();
    }

    //移动
    void Movement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float facedircetion = Input.GetAxisRaw("Horizontal");

        //角色移动
        if (horizontalMove != 0f)
		{
			rb.velocity = new Vector2(horizontalMove * speed * Time.fixedDeltaTime, rb.velocity.y);
			anim.SetFloat("running",Mathf.Abs(horizontalMove));
		}
		if (facedircetion != 0f)
		{
				transform.localScale = new Vector3(facedircetion, 1, 1);
			
		}
        
    }

    //切换动画效果
    void SwitchAnim()
    {
        //anim.SetBool("idle", false);
        if (anim.GetBool("jumping"))
        {
            if(rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }else if (isHurt)
        {
            anim.SetBool("hurt", true);
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {

                anim.SetBool("hurt", false);
                //anim.SetBool("idle", true);
                isHurt = false;
            }
        }
        else if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            //anim.SetBool("idle", true);
        }

    }
    
    //碰撞触发器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //收集物品
        if (collision.tag == "Cherry")
        {
            //cherryAudio.Play();
            //Destroy(collision.gameObject);
            //Cherry += 1;
            collision.GetComponent<Animator>().Play("isGot");
            //CherryNum.text = Cherry.ToString();
            
        }
        if (collision.tag == "Gem")
        {
            //cherryAudio.Play();
            Destroy(collision.gameObject);
        }
        //死亡线
        if (collision.tag == "DeadLine")
        {
            //GetComponent<AudioSource>().enabled = false;
            Invoke("Restart", 1f);
        }

    }

    //消灭敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", true);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))
            {
                enemy.JumpOn();
                rb.velocity = Vector2.up * JumpForce;
                anim.SetBool("jumping", true);

            //受伤
            }else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-10, rb.velocity.y);
                //hurtAudio.Play();
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(10, rb.velocity.y);
                //hurtAudio.Play();
                SoundManager.instance.HurtAudio();
                isHurt = true;
            }
        }
    }
    //下蹲趴下
    void Crouch()//修正
    {
        if (Input.GetButton("Crouch"))
        {
            anim.SetBool("crouching", true);
            DisColl.enabled = false;
        }
        else if (!Physics2D.OverlapCircle(CellingCheck.position, 0.2f, ground))
        {
            anim.SetBool("crouching", false);
            DisColl.enabled = true;
        }   
    }

    //角色跳跃
    /*void Jump()
    {

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(0, JumpForce * Time.deltaTime);
            jumpAudio.Play();
            anim.SetBool("jumping", true);

        }
    }*/

    void newJump()
    {
        if (isGround)
        {
            extraJump = 1;
        }
        if(Input.GetButtonDown("Jump") && extraJump > 0)
        {
            rb.velocity = Vector2.up * JumpForce;
            extraJump--;
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
        }
        if(Input.GetButtonDown("Jump")&& extraJump == 0 && isGround)
        {
            rb.velocity = Vector2.up * JumpForce;
            SoundManager.instance.JumpAudio();
            anim.SetBool("jumping", true);
        }
    }

    //重制当前场景
    void Restart()
    {
        
         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void CherryCount()
    {
        Cherry += 1;
    }
}

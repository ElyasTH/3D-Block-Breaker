using UnityEngine;
using UnityEngine.Events;

public class BallMovement : MonoBehaviour
{

    public bool locked = true;
    public Transform lockPosition;
    public Rigidbody rb;
    public int startForce = 150;
    public GameObject player;
    public GameObject powerUpLight;
    public GameHandler gameHandler;
    private float lastZ = 0;
    public float checkZDelay = 3f;

    public static bool lockAndLaunch = false;
    private float lockAndLaunchTime = 5;
    public UnityEvent onCollisionEvent;
    public float minSpeed, maxSpeed;

    void Start(){
        rb.AddForce(0,0,startForce);
        transform.localScale = new Vector3(0.5f,0.5f,0.5f);
    }

    private void FixedUpdate()
    {
        if (!locked){
            if (rb.velocity.magnitude < minSpeed) rb.AddForce(rb.velocity.normalized * Time.deltaTime * 100);
            if (rb.velocity.magnitude > maxSpeed) rb.AddForce(rb.velocity.normalized * Time.deltaTime * -100);

            if (Mathf.Abs(gameObject.transform.position.z-lastZ) < 0.1){
                checkZDelay -= Time.deltaTime;
            }
            else {
                checkZDelay = 3f;
            }
            if (checkZDelay <= 0){
                rb.AddForce(0,0,-10);
                checkZDelay = 3f;
            }
            lastZ = transform.position.z;

            if (transform.position.z < -13f)
            {
                checkZDelay = 3f;
                if (gameHandler.ballCount == 1){
                    gameHandler.loseLife(this.gameObject);
                }
                else{
                    gameHandler.ballCount--;
                    Destroy(this.gameObject);
                }
                gameHandler.resetCombo();
            } 
        }
        else{
            gameObject.transform.position = lockPosition.position;
        }

        if (lockAndLaunch){
            lockAndLaunchTime -= Time.deltaTime;
            powerUpLight.SetActive(true);
            if (lockAndLaunchTime <= 0){
                lockAndLaunch = false;
                lockAndLaunchTime = 5;
                powerUpLight.SetActive(false);
            }
        }
    }

    public void reset(){
        transform.position = new Vector3(0, 0.9f,  -5.22f);
        locked = true;
        player.GetComponent<PlayerMovement>().locked_balls.Add(this);
    }

    void OnCollisionEnter(Collision col){
        if (col.gameObject.tag == "Player" && lockAndLaunch && !locked){
            locked = true;
            player.GetComponent<PlayerMovement>().locked_balls.Add(this);
            gameHandler.resetCombo();
        } 
        onCollisionEvent?.Invoke();
    }

    public void Launch(){
        if (locked){
            locked = false;
            rb.velocity = new Vector3(0,0,0);
            rb.AddForce(0, 0, startForce);
            player.GetComponent<PlayerMovement>().locked_balls.Remove(this);
        }
    }
}

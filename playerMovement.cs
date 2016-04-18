using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;


public class playerMovement : MonoBehaviour {

    //Myo game object to connect with
    //This obeject must have a THalmicMyo script attached.
    public GameObject myo = null;
    //Last post from previous updated, used to determine if pose has changed
    private Pose p_lastPose = Pose.Unknown;

    public float Speed = 10.0f;
    public float m_JumpSpeed = 5f;
    public LayerMask GroundLayers;

    private Transform m_GroundCheck;
    

    void Start () {
        m_GroundCheck = transform.FindChild("GroundCheck");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();
        float m_Speed = Input.GetAxis("Horizontal"); // for keyboard
        //if (hSpeed < 0)
        //    transform.localScale = new Vector3(-1, 1, 1);
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(m_Speed * Speed, this.GetComponent<Rigidbody2D>().velocity.y);
        
        if(thalmicMyo.pose != p_lastPose){
            p_lastPose = thalmicMyo.pose;
            
            //Fist Pose
            if(thalmicMyo.pose == Pose.Fist){
                //flip gravity
                thalmicMyo.Vibrate(VibrationType.Medium);
                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }
            else if(thalmicMyo.pose == Pose.WaveIn){
                //Wave in (left) Pose
                //move character to left
                float hSpeed = Input.GetAxis("Horizontal"); // for keyboard
                if( hSpeed < 0){
                    Debug.Log("made it");
                    transform.localScale = new Vector3(-1,1,1);
                }
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(hSpeed * Speed, this.GetComponent<Rigidbody2D>().velocity.y);
            }
            else if( thalmicMyo.pose == Pose.WaveOut){
                //Wave out (right) Pose
                //move character to the right
                float hSpeed = Input.GetAxis("Horizontal");
                //if( hSpeed > 0 )
                //    transform.localScale = new Vector3(1,1,1);
                this.GetComponent<Rigidbody2D>().velocity = new Vector2(hSpeed * Speed, this.GetComponent<Rigidbody2D>().velocity.y);
            }
            else if( thalmicMyo.pose == Pose.DoubleTap) {
                //not yet thought off
            }
            else if( thalmicMyo.pose == Pose.FingersSpread){
                //float hSpeed = Input.GetAxis("Horizontal");
                bool isGrounded = Physics2D.OverlapPoint(m_GroundCheck.position);
                if (isGrounded){
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * m_JumpSpeed, ForceMode2D.Impulse);
                }
            }        	    
	    }
    }

    void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo) {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard)
            myo.Unlock(UnlockType.Timed);
        myo.NotifyUserAction();
    }
}

using System;
using UnityEngine;
using static GameControll;
using CommandTerminal;
using Mirror;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        }

        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        //public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        [SerializeField] public float NormalSpeed;
        private float speed;

        private static Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private AudioSource m_AudioSource;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
        private MouseLook mouseLook = new MouseLook();

        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private float m_StepInterval;

        public float RunMultiplier = 1.5f;   // Speed when sprinting

        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Running;

        public static bool flymode = false;

        public static RigidbodyFirstPersonController rfpc;
        public static Camera c;
        public static AudioListener al;
        public Canvas goverlay;

        public bool releaseControll = true;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool lockPosition = false;


        private void Start()
        {
            if (!GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                gameObject.layer = 0;
                foreach (Transform tran in GetComponentsInChildren<Transform>())
                {
                    tran.gameObject.layer = 0;
                }

                GetComponent<Rigidbody>().isKinematic = true;
                Destroy(this);
                return;
            }

            rfpc = this;
            c = cam;
            al = cam.GetComponent<AudioListener>();
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_AudioSource = GetComponent<AudioSource>();
            mouseLook.Init(transform, cam.transform);

            Time.timeScale = 0;
            TerrainManager.tm.follower = cam.transform.parent.gameObject;
            UIManager.ui.enabled = true;
        }


        private void Update()
        {
            if (releaseControll)
            {
                RotateView();
                if (InputManager.GetKey("Jump") && releaseControll && !m_Jump)
                {
                    m_Jump = true;
                }
            }
        }

        [RegisterCommand(Name = "fly",Help = "Manipulate flying state", MinArgCount = 0, MaxArgCount = 1)]
        public static void PlayerFly(CommandArg[] args)
        {
            flymode = args.Length > 0 ? args[0].Bool : !flymode;
            m_RigidBody.useGravity = !flymode;
            m_RigidBody.drag = flymode ? 2 : 0;
        }


        private void FixedUpdate()
        {
            cam.transform.position = (localControll.rightcamera.transform.position + localControll.rightcamera.transform.TransformVector(new Vector3(0,0,-localControll.animator.GetComponent<SkinnedMeshRenderer>().localBounds.extents.z*2/3)) + localControll.leftcamera.transform.position + localControll.leftcamera.transform.TransformVector(new Vector3(0, 0, -localControll.animator.GetComponent<SkinnedMeshRenderer>().localBounds.extents.z*2/3))) /2;
            if (lockPosition)
                return;
            GroundCheck();
            Vector2 input = GetInput();

            if (releaseControll && (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = (Vector3.forward* input.y + Vector3.right* input.x).normalized; //Problem
                //desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                //desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
                //desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
                //desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;
                /*if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
                {*/
                if (m_Running)
                {
                    localControll.animator.GetComponent<Animator>().SetBool("Run", true);
                    localControll.animator.GetComponent<Animator>().SetBool("Walk", false);
                }
                else
                {
                    localControll.animator.GetComponent<Animator>().SetBool("Walk", true);
                    localControll.animator.GetComponent<Animator>().SetBool("Run", false);
                }

                transform.Translate(desiredMove*speed/75,Space.Self);//desiredMove * SlopeMultiplier()*0.01f);//, ForceMode.Impulse);
                physicQueue.RunQueue();
                //}
                ProgressStepCycle(speed);
            }
            else
            {
                localControll.animator.GetComponent<Animator>().SetBool("Walk", false);
                localControll.animator.GetComponent<Animator>().SetBool("Run", false);
            }

            if (releaseControll && InputManager.GetKey("Jump") && flymode)
            {
                m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce * 0.1f, 0f), ForceMode.Impulse);
            }
            if (releaseControll && InputManager.GetKey("Sneak") && flymode)
            {
                m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce * -0.1f, 0f), ForceMode.Impulse);
            }

            if (m_IsGrounded)
            {
                //m_RigidBody.drag = 5f;
                localControll.animator.GetComponent<Animator>().SetBool("Jump", false);

                if (m_Jump)
                {
                    localControll.animator.GetComponent<Animator>().SetBool("Jump", true);
                    PlayJumpSound();
                    //m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                //m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (GetComponent<Rigidbody>().velocity.sqrMagnitude > 0)
            {
                m_StepCycle += (GetComponent<Rigidbody>().velocity.magnitude + speed) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_IsGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        /*private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }*/


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, 1 <<LayerMask.NameToLayer("Default") | 1<<18, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            
            Vector2 input = new Vector2
                {
                    x = InputManager.GetAxis("Horizontal"),
                    y = InputManager.GetAxis("Vertical")
                };
#if !MOBILE_INPUT
            if (releaseControll && InputManager.GetKey("Sprint"))
            {
                if (m_IsGrounded || flymode)
                {
                    speed = NormalSpeed * RunMultiplier;
                    m_Running = true;
                }
            }
            else
            {
                speed = NormalSpeed;
                m_Running = false;
            }
#endif
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            //TODO: ChangeHeadDirection
            mouseLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, 1 << LayerMask.NameToLayer("Default") | 1<<18, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded)
            {
                PlayLandingSound();
                if (m_Jumping)
                {
                    m_Jumping = false;
                }
            }
        }
    }
}

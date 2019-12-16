using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    // 1) Handles sound in some way.
    private void Awake()
    {
        //Calls audio source attached to scene
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    //Plays audio through multiple scenes if audio source file is attached
    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    //If audio source cannont be found, music stops 
    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        //Plays audio source from scene depending on win/lose condition
        //If you win the game, the scene manager goes to the YouWin scene and starts attached auido source accordingly
        if (sceneName == "YouWin")

        {
            StopMusic();
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }

        //If you lose the game, the scene manager goes to the YouLose scene and starts attached auido source accordingly
        if (sceneName == "You Lose")

        {
            //If audio source cannont be found, music stops on that scene
            StopMusic();

            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }

    }



    // 2) Handles deaths and respawning
    public class Death : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        //In this instance for our game, when the player or AI falls of the map, they fall below the -10 y axis and the game object is removed from the scene
        void Update()
        {
            if (gameObject.transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }
    }



    // 3) Detects and send over a network player position and behavior
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField]
        private float directionDampTime = .2f;
        private Animator animator;

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        //Debugging sends a log error if the animation cannot be located
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

        }

        // Update is called once per frame
        void Update()
        //Checks to see if photon network is running 
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }
            if (!animator)
            {
                return;
            }
            // Deals with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // only allow jumping if we are running.
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // When using trigger parameter
                if (Input.GetButtonDown("Fire2"))
                {
                    animator.SetTrigger("Jump");
                }
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0)
            {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion
    }


    // 4) Saves permanent data to a server or locally
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants


        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";


        #endregion


        #region MonoBehaviour CallBacks

        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        void Start()
        {

            //Stores name entered in launcher scene to player UI
            //Stores in photon network, so name only has to be enetered once
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        #endregion
        #region Public Methods

        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.

        public void SetPlayerName(string value)
        {
            //Calls error if player name is not entered in launcher scene
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            //Stores nick name as value in network
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion
    }


    // 5) Set up a custom UI for a unity network 
    //UI Setup over photon network 
    protected virtual void UiSetupApp()
    {
        GUI.skin.label.wordWrap = true;
        if (!this.isSetupWizard)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CurrentLang.MainMenuButton, GUILayout.ExpandWidth(false)))
            {
                this.photonSetupState = PhotonSetupStates.MainUi;
            }

            GUILayout.EndHorizontal();
        }


        // setup header
        this.UiTitleBox(CurrentLang.SetupWizardTitle, BackgroundImage);

        // setup info text
        GUI.skin.label.richText = true;
        GUILayout.Label(CurrentLang.SetupWizardInfo);

        // input of appid or mail
        EditorGUILayout.Separator();
        GUILayout.Label(CurrentLang.EmailOrAppIdLabel);
        this.minimumInput = false;
        this.useMail = false;
        this.useAppId = false;
        this.mailOrAppId = EditorGUILayout.TextField(this.mailOrAppId);
        if (!string.IsNullOrEmpty(this.mailOrAppId))
        {
            this.mailOrAppId = this.mailOrAppId.Trim();
            // note: we trim all input
            if (AccountService.IsValidEmail(this.mailOrAppId))
            {
                // this should be a mail address
                this.minimumInput = true;
                this.useMail = this.minimumInput;
            }
            else if (ServerSettings.IsAppId(this.mailOrAppId))
            {
                // this should be an appId
                this.minimumInput = true;
                this.useAppId = this.minimumInput;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Utilities;

public class PlayerController : MonoBehaviour, IDamageable
{
    [HideInInspector] public PlayerManager playerManager;

    [Header("Movement Settings")]
    [SerializeField] CharacterController controller;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    public float speed = 12f;
    public float sprintSpeed = 18f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    Vector3 velocity;
    bool isGrounded;

    [Header("Mouse Settings")]
    [SerializeField] Transform cameraHolder;

    float xRotation = 0f;

    [Header("Health")]
    public const float maxHealth = 100f;
    public float hitCache = 5f;
    public float currentHealth = maxHealth;

    [Header("UI")]
    [SerializeField] GameObject ui;
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject deathUI;
    [SerializeField] TMP_Text deathMessage;
    [SerializeField] GameObject crosshair1;
    [SerializeField] GameObject crosshair2;
    [SerializeField] int crosshairHitTime;

    [Header("Other")]
    [SerializeField] ItemManager itemManager;
    [SerializeField] MenuManager menuManager;
    [SerializeField] NotificationManager notificationManager;
    [SerializeField] Transform itemHolder;
    [SerializeField] Transform secondaryItemHolder;
    [SerializeField] DamageEffectController damageEffectController;
    [SerializeField] TMP_Text teamText;

    [Header("Colours")]
    [SerializeField] GameObject gfx;
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;
    [SerializeField] Color32 redColor;
    [SerializeField] Color32 blueColor;

    [HideInInspector] public PhotonView PV;
    string lastHitUser;
    float lastHitTime;

    float redCrosshairExpireTime;

    [HideInInspector] public bool isPaused;

    void Awake()
    {
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int) PV.InstantiationData[0]).GetComponent<PlayerManager>();
        NotificationEvents.SendNotificiation += (string message, RpcTarget target) => { SendNotification(message, target); };
        NotificationEvents.PlayerDied += (string message, string killed, string killer) => { PlayerDied(message, killed, killer);  };

    }

    void OnDestroy()
    {
        NotificationEvents.SendNotificiation -= SendNotification;
        NotificationEvents.PlayerDied -= PlayerDied;
    }

    void Start()
    {
        // For Team Death Match - change the player's colour, this needs to run whether it is a local player or another client.

        if ((string)PhotonNetwork.CurrentRoom.CustomProperties["gamemode"] == "tdm")
        {
            Player player = PV.Owner;
            if (player.GetTeam() != -1)
            {
                int team = player.GetTeam();

                if (team == 0)
                { // Red
                    gfx.GetComponent<Renderer>().material = redMaterial;
                }
                else if (team == 1)
                { // Blue
                    gfx.GetComponent<Renderer>().material = blueMaterial;
                }
            }
        } else if ((string)PhotonNetwork.CurrentRoom.CustomProperties["gamemode"] == "ffa")
        {
            teamText.gameObject.SetActive(false);
        }
        

        if (!PV.IsMine)
        {
            Destroy(cameraHolder.GetComponentInChildren<Camera>().gameObject);
            Destroy(ui);
            Destroy(GetComponent<GameSettingsLink>());
            Destroy(notificationManager);
            Destroy(damageEffectController);
            Destroy(GetComponent<DropManager>());
            itemHolder.position = secondaryItemHolder.position;
            return;
        }

        Room r = PhotonNetwork.CurrentRoom;
        Debug.Log("One Shot: " + r.GetOneShot());
        Debug.Log("TDM Win: " + r.GetTDMPointsToWin());
        Debug.Log("TDM Kill: " + r.GetTDMPointsKill());
        Debug.Log("TDM Headshot: " + r.GetTDMPointsHeadshot());

        Cursor.lockState = CursorLockMode.Locked;

        if (PhotonNetwork.LocalPlayer.GetTeam() != -1)
        {
            Debug.Log("T: " + PhotonNetwork.LocalPlayer.GetTeam());
            int team = PhotonNetwork.LocalPlayer.GetTeam();

            if (team == 0)
            { // Red
                teamText.text = "Red Team";
                teamText.color = redColor;
            }
            else if (team == 1)
            { // Blue
                teamText.text = "Blue Team";
                teamText.color = blueColor;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            ScoreboardManager.Instance.SetOpen(true);
        } else if (ScoreboardManager.Instance.IsOpen())
        {
            ScoreboardManager.Instance.SetOpen(false);
        }

        if (crosshair2.activeSelf && Time.time > redCrosshairExpireTime)
        {
            crosshair1.SetActive(true);
            crosshair2.SetActive(false);
        }

        if (damageEffectController.damageEffect == DamageEffectController.DAMAGE_EFFECT_PULSE && currentHealth > 0.3 * maxHealth)
        {
            damageEffectController.SetDamageEffect(DamageEffectController.DAMAGE_EFFECT_SINGLE);
        }

        if (Input.GetKeyDown(KeyCode.Escape) )
        {
            if (isPaused)
            {
                isPaused = false;
                menuManager.OpenMenu("general");
                Cursor.lockState = CursorLockMode.Locked;
            } else
            {
                isPaused = true;
                Cursor.lockState = CursorLockMode.None;
                menuManager.OpenMenu("paused");
            }
            
        }

        Jump(); // This makes sure that the player will fall when paused instead of hanging stuck in the air however the jumping functionality is still disabled when paused.

        if (isPaused)
        {
            return;
        }

        Look();
        Move();
        

        if (transform.position.y <= -10f) // Fell into the void
        {
            if (lastHitUser != null && Time.time < lastHitTime + hitCache)
            {
                Die("void_" + lastHitUser);
            } else
            {
                Die("The Void");
            }
        }

        DisplayInteractable();
    }

    public void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * (CameraZoom.isZoomed ? GameSettings.zoomSensitivity : GameSettings.mouseSensitivity) * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * (CameraZoom.isZoomed ? GameSettings.zoomSensitivity : GameSettings.mouseSensitivity) * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void Move()
    {
        float _speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * _speed * Time.deltaTime);
    }

    public void Jump()
    {
        if (Input.GetButton("Jump") && isGrounded && !isPaused)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void DisplayInteractable()
    {
        // Find & display a text billboard if the player is looking at an interactable object
        Ray ray = GetComponentInChildren<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.collider.gameObject.GetComponentInParent<Interactable>() != null)
            {
                Interactable interactable = hit.collider.gameObject.GetComponentInParent<Interactable>();
                interactable.text.gameObject.SetActive(true);
                DropManager.Instance.lookingAtDrop = interactable;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Use(this);
                }

                return;
            }
        }

        DropManager.Instance.lookingAtDrop = null;
    }

    public void HitOther()
    {
        redCrosshairExpireTime = Time.time + crosshairHitTime;
        crosshair2.SetActive(true);
        crosshair1.SetActive(false);
    }

    /*
     * TakeDamage() is called locally on the shooters computer from StandardGun or any weapon. This method is run on the object that was hit (the object
     * representing a networked player).
     * We then send this through an RPC to RPC_TakeDamage, in the rpc we check if it's mine. 'Mine' meaning that the object that it was hit client side,
     * is the same as the damaged client.
     * e.g We're shooting an object representing a networked player, this code then runs to tell the networked player that he's been hit. The TakeDamage() is
     * on the object representing a networked player, the RPC goes out to the networked player's local object.
     * 
     * called on this client, when it is damaged
     */
    public bool TakeDamage(float damage, string damager)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, damager);
        if ((currentHealth -= damage) <= 0f)
        {
            return true;
        }
        return false;
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, string damager)
    {
        if (!PV.IsMine)
            return;

        lastHitUser = damager;
        lastHitTime = Time.time;

        currentHealth -= damage;

        healthBarImage.fillAmount = currentHealth / maxHealth;

        damageEffectController.AddHit();

        damageEffectController.SetDamageEffect(DamageEffectController.DAMAGE_EFFECT_SINGLE);

        if (currentHealth < 0.3 * maxHealth)
        {
            damageEffectController.SetDamageEffect(DamageEffectController.DAMAGE_EFFECT_PULSE);
        }

        damageEffectController.SetPlaying(true);

        if (currentHealth <= 0f)
        {
            damageEffectController.Clear();
            Die(damager);
        } 
    }

    public void SetHealth(float health)
    {
        PV.RPC("RPC_SetHealth", RpcTarget.All, health);
    }

    [PunRPC]
    void RPC_SetHealth(float health)
    {
        currentHealth = health;
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }


    public void Die(string damager)
    {
        playerManager.Die(damager);
    }

    [PunRPC]
    void RPC_PlayImpactEffect(Vector3 hitPosition, Vector3 hitNormal, string itemName)
    {
        GameObject bulletImpactObj = Instantiate(((Gun)itemManager.GetItem(itemName)).bulletImpactPrefab, hitPosition, Quaternion.LookRotation(hitNormal));
        Destroy(bulletImpactObj, 2f);
    }

    [PunRPC]
    void RPC_PlaySoundEffect(string itemName)
    {
        if (((Gun)itemManager.GetItem(itemName)).soundEffect.isActiveAndEnabled)
        {
            AudioSource audio = ((Gun)itemManager.GetItem(itemName)).soundEffect;
            audio.volume = GameSettings.volume;

            float finalVolume = GameSettings.volume;
            if (finalVolume >= 0.1f)
            {
                audio.PlayOneShot(audio.clip, GameSettings.volume);
            }
        }
        
    }

    public void ShowDeathMessage(string damager)
    {
        deathUI.SetActive(true);
        deathMessage.text = "You were killed by " + damager;

        StartCoroutine(DisableDeathMessage(3.5f));
    }

    IEnumerator DisableDeathMessage(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        deathUI.SetActive(false);
    }
    

    /*
     * RPCs
     */

    [PunRPC]
    void RPC_SendNotification(string message)
    {
        if (!PV.IsMine)
            return;

        notificationManager.SendNotification(message);
    }

    void SendNotification(string message, RpcTarget target)
    {
        PV.RPC("RPC_SendNotification", target, message);
    }

    [PunRPC]
    void RPC_PlayerDied(string message, string killed, string killer)
    {
        if (!PV.IsMine)
            return;

        string _message = message;
        string _killed = killed;
        string _killer = killer;

        if (killed.Equals(PV.Owner.NickName))
        {
            _killed = "You";
            _message = _message.Replace("was", "were");
        }
        if (killer.Equals(PV.Owner.NickName))
        {
            _killer = "You";
        }

        notificationManager.SendNotification(_message.Replace("<KILLED>", _killed).Replace("<KILLER>", _killer));
    }

    void PlayerDied(string message, string killed, string killer)
    {
        PV.RPC("RPC_PlayerDied", RpcTarget.All, message, killed, killer);
    }

}

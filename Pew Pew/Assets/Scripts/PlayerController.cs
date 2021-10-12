using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

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
    private float currentHealth = maxHealth;

    [Header("UI")]
    [SerializeField] GameObject ui;
    [SerializeField] Image healthBarImage;
    [SerializeField] GameObject deathUI;
    [SerializeField] TMP_Text deathMessage;

    [Header("Other")]
    [SerializeField] ItemManager itemManager;
    [SerializeField] MenuManager menuManager;
    [SerializeField] NotificationManager notificationManager;
    [SerializeField] Transform itemHolder;
    [SerializeField] DamageEffectController damageEffectController;

    PhotonView PV;

    [HideInInspector] public bool isPaused;

    void Awake()
    {
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int) PV.InstantiationData[0]).GetComponent<PlayerManager>();
        NotificationEvents.SendNotificiation += (string message, RpcTarget target) => { SendNotification(message, target); };
    }

    void OnDestroy()
    {
        NotificationEvents.SendNotificiation -= SendNotification;
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(cameraHolder.GetComponentInChildren<Camera>().gameObject);
            Destroy(ui);
            Destroy(GetComponent<GameSettingsLink>());
            Destroy(notificationManager);
            //Destroy(playerManager.settings);
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f, "developer");
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
        if (isPaused)
        {
            return;
        }

        Look();
        Move();
        Jump();

        if (transform.position.y <= -10f) // Fell into the void
        {
            Die("The Void");
        }
    }

    public void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * GameSettings.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * GameSettings.mouseSensitivity * Time.deltaTime;

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
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
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
        if ((currentHealth - damage) <= 0f)
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

}

using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour {
    public enum ShootMode {
        Semi, BoltAction
    }
    
    public enum ZoomMode {
        Normal, Scope
    }

    [Header("Weapon Stats")]
    public int damage;
    public float fireRate, spread, reloadTime, aimTime;
    public int magazineSize, ammo, bulletsInMag;
    // public int bulletsPerTap;
    public float originalZoomLevel, zoomLevel;
    public ShootMode shootingMode;
    public ZoomMode zoomMode;
    public bool slowMotion;
    public float muzzleVelocity;
    public float zoomSensitivity = 0.1f;


    // state tracking 
    bool m_Firing, m_Reloading, m_Aiming;
    float m_FireTimer = 0f;
    float m_StandardSensitivity;
    MouseLook m_MouseLook;
    
    [Header("References")]
    public Transform shootingPoint;
    public ParticleSystem muzzleFlash;
    public AudioClip fireAudio;
    public AudioClip triggerAudio;
    public AudioClip loadRoundAudio;
    public AudioClip reloadAudio;
    public Vector3 aimPosition;
    public TextMeshProUGUI text;
    public Camera weaponCamera;
    public GameObject crossHair; 
    public GameObject scope; 
    public FirstPersonController playerFirstPersonController;
    
    // private references
    private Animator m_Animator;
    private AudioSource m_AudioSource;
    private Vector3 m_OriginalPosition;
    private Camera m_PlayerCamera;
    private IEnumerator m_AimCoroutine;
    
    // public CamShake camShake;
    private static readonly int Fire1 = Animator.StringToHash("Fire");
    private static readonly int Reload1 = Animator.StringToHash("Reload");
    private static readonly int Aim1 = Animator.StringToHash("Aim");
    private static readonly int Empty = Animator.StringToHash("Empty");

    private void Awake() {
        m_Reloading = false;
        m_Aiming = false;
        m_FireTimer = fireRate;
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_OriginalPosition = transform.localPosition;
        m_PlayerCamera = Camera.main;
    }

    private void Start() {
        m_MouseLook = playerFirstPersonController.GetMouseLook();
        m_StandardSensitivity = m_MouseLook.XSensitivity;
    }

    private void OnDisable() {
        ResetAim();
    }

    private void Update() {
        // add time to fire rate timer
        if (m_FireTimer < fireRate)
            m_FireTimer += Time.deltaTime;

        HandleInput();

        // SetText
        text.SetText("ammo: " + bulletsInMag + " / " + magazineSize + " (" + ammo + ")");
    }

    private void FixedUpdate() {
        // AnimatorStateInfo animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        // if (animatorStateInfo.IsName("Fire"))
        //     m_Animator.SetBool("Fire", false);
    }

    private void HandleInput() {
        // fire
        HandleFireInput();
        
        // reload
        HandleReloadInput();
        
        // aim
        HandleAimInput();
    }
    
    private void HandleFireInput() {
        bool shootingInput;
        
        if (shootingMode == ShootMode.Semi && bulletsInMag > 0) {
            shootingInput = Input.GetButton("Fire1");
        }
        else if (shootingMode == ShootMode.BoltAction || bulletsInMag == 0) {
            shootingInput = Input.GetButtonDown("Fire1");
        }
        else {
            throw new ArgumentOutOfRangeException();
        }
        
        // fire
        if (shootingInput) {
            m_Firing = true;
            
            if (m_Reloading)
                m_Firing = false;

            if (m_FireTimer < fireRate && bulletsInMag > 0) {
                m_Firing = false;
                return;
            }

            m_FireTimer = 0f;
            
            if (m_Firing) {
                m_Animator.SetBool(Fire1, true);
                Fire();
                if (bulletsInMag > 0 && shootingMode == ShootMode.BoltAction)
                    PlayLoadRoundSound();
            }
        }
        else {
            m_Firing = false;
            m_Animator.SetBool(Fire1, false);
        }
    }

    private void HandleReloadInput() {
        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(StartReload());
        }
    }

    private void HandleAimInput() {
        if (Input.GetButtonDown("Fire2")) {
            if (zoomMode == ZoomMode.Normal)
                ToggleNormalAim();
            else
                ToggleScopeAim();
        }
    }
    
    private void Fire() {

        if (bulletsInMag > 0) {
            
            // deduct bullet
            bulletsInMag--;
            
            // play muzzle flash particle
            muzzleFlash.Play();
            
            // play firing sound
            StartCoroutine(PlayFiringSounds());
            
            if (slowMotion)
                TimeFlowManager.instance.DoSlowMotion();
            
            // define bullet spread
            // float x = Random.Range(-spread, spread);
            // float y = Random.Range(-spread, spread);

            // calculate direction with spread
            // Vector3 direction = shootingPoint.forward + shootingPoint.TransformDirection(new Vector3(x, y, 0));
            Vector3 direction = shootingPoint.forward;

            // raycast
            RaycastHit raycastHit;
            // Debug.DrawRay(shootingPoint.position, direction * range, Color.red, 5f);
            // if (Physics.Raycast(shootingPoint.position, direction, out raycastHit, range, shootableLayer)) {
            //     if (raycastHit.collider.CompareTag("Damageable")) {
            //         raycastHit.collider.GetComponent<HealthController>().TakeDamage(damage, raycastHit);
            //     }   
            // }
            
            // Ray without spread and bullet drop
            // Debug.DrawLine (shootingPoint.position, shootingPoint.position + direction * muzzleVelocity, Color.yellow, 1000f);
            
            bool hit = BulletRaycast(shootingPoint.position, direction * muzzleVelocity, out raycastHit, 1000);
            if (hit && raycastHit.collider.CompareTag("Damageable")) {
                raycastHit.collider.GetComponent<GenericHealthController>().TakeDamage(damage, raycastHit);
            }
        }
        else {
            m_Firing = false;
            m_Animator.SetBool(Empty, true);
            AnimatorStateInfo animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (!animatorStateInfo.IsName("Fire"))
                PlayTriggerSound();
        }
    }

    private IEnumerator StartReload() {
        if (bulletsInMag == magazineSize || m_Reloading || ammo == 0)
            yield break;
        
        m_Animator.SetTrigger(Reload1);
        m_Reloading = true;
        PlayReloadSound();
        yield return new WaitForSeconds(reloadTime);
        
        m_Reloading = false;
        Reload();
    }

    private void Reload() {
        int amountToReload = magazineSize - bulletsInMag;
        bulletsInMag += Math.Min(ammo, amountToReload);
        ammo = Math.Max(0, ammo - amountToReload);
        m_Animator.SetBool(Empty, false);
    }

    private void ToggleNormalAim() {
        if (m_Reloading)
            return;

        m_Aiming = !m_Aiming;
        
        if (m_AimCoroutine != null)
            StopCoroutine(m_AimCoroutine);
        
        if (m_Aiming) {
            m_AimCoroutine = Aim();

            playerFirstPersonController.SetMouseLookSensitivity(zoomSensitivity);
            
            StartCoroutine(m_AimCoroutine);
            
        }
        else {
            m_AimCoroutine = Hip();    
            
            playerFirstPersonController.SetMouseLookSensitivity(m_StandardSensitivity);
            
            StartCoroutine(m_AimCoroutine);
        }
        m_Animator.SetBool(Aim1, m_Aiming);
    }

    private void ToggleScopeAim() {
        ToggleNormalAim();
        crossHair.SetActive(!m_Aiming);
        scope.SetActive(m_Aiming);
        weaponCamera.enabled = !weaponCamera.enabled;
    }

    IEnumerator Aim() {
        Vector3 start = transform.localPosition;
        Vector3 end = aimPosition;
        float zoomStart = m_PlayerCamera.fieldOfView;
        float zoomEnd = zoomLevel;
        float t = 0;
        
        while(t < 1)
        {
            yield return null;
            t += Time.deltaTime / aimTime;
            transform.localPosition = Vector3.Lerp(start, end, t);
            m_PlayerCamera.fieldOfView = Mathf.Lerp(zoomStart, zoomEnd, t);

        }
        transform.localPosition = end;
        m_PlayerCamera.fieldOfView = zoomEnd;
    }
    
    IEnumerator Hip() {
        Vector3 start = transform.localPosition;
        Vector3 end = m_OriginalPosition;
        float zoomStart = m_PlayerCamera.fieldOfView;
        float zoomEnd = originalZoomLevel;
        float t = 0;
        
        while(t < 1)
        {
            yield return null;
            t += Time.deltaTime / aimTime;
            transform.localPosition = Vector3.Lerp(start, end, t);
            m_PlayerCamera.fieldOfView = Mathf.Lerp(zoomStart, zoomEnd, t);

        }
        transform.localPosition = end;
        m_PlayerCamera.fieldOfView = zoomEnd;
    }
    
    public int GetTotalAmmo() {
        return ammo + bulletsInMag;
    }

    private void ResetAim() {
        m_Aiming = false;
        transform.localPosition = m_OriginalPosition;
        m_PlayerCamera.fieldOfView = originalZoomLevel;
        if (zoomMode == ZoomMode.Scope) {
            if (crossHair)
                crossHair.SetActive(true);
            if (scope)
                scope.SetActive(false);
            weaponCamera.enabled = true;
        }
    }

    public bool IsReloading() {
        return m_Reloading;
    }
    
    public bool IsAiming() {
        return m_Aiming;
    }
    
    public bool IsFiring() {
        return m_Firing;
    }

    IEnumerator PlayFiringSounds() {
        yield return null;

        if (bulletsInMag > 0)
            PlayFireSound();

        while (m_AudioSource.isPlaying)
        {
            yield return null;
        }
    }
    
    private void PlayFireSound() {
        m_AudioSource.PlayOneShot(fireAudio);
    }
    
    private void PlayTriggerSound() {
        m_AudioSource.PlayOneShot(triggerAudio);
    }
    
    private void PlayLoadRoundSound() {
        StartCoroutine(PlaySoundSequentially(loadRoundAudio));
    }
    
    private void PlayReloadSound() {
        m_AudioSource.PlayOneShot(reloadAudio);
    }
    
    IEnumerator PlaySoundSequentially(AudioClip clip) {
        yield return null;
        
        while (m_AudioSource.isPlaying)
        {
            yield return null;
        }

        m_AudioSource.PlayOneShot(clip);
    }
    
    public bool BulletRaycast(Vector3 startPoint, Vector3 velocity, out RaycastHit hit, int iterations = 100, float timeStep = 0.01f) {
        Vector3 velocity2 = velocity;
        Vector3 startPoint2 = startPoint;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        
        velocity += shootingPoint.TransformDirection(new Vector3(x, y, 0)) * muzzleVelocity;
        velocity += shootingPoint.TransformDirection(GameManager.instance.GetWindSpeed());
        
        for(int ii = 1; ii < iterations; ii++) {
            Debug.DrawLine (startPoint, startPoint + velocity * timeStep, Color.red, 1000f);
            // Bullet without spread and wind but with bullet drop
            Debug.DrawLine (startPoint2, startPoint2 + velocity2 * timeStep, Color.blue, 1000f);

            startPoint += velocity * timeStep;
            startPoint2 += velocity2 * timeStep;
            
            // Detect collision
            // if (Physics.Raycast(startPoint2, velocity2, out hit, velocity2.magnitude * timeStep)) {
            if (Physics.Raycast(startPoint, velocity, out hit, velocity.magnitude * timeStep)) {
                return true;
            }
            
            velocity.y -= 9.81f * timeStep; // simulate gravitational acceleration
            velocity2.y -= 9.81f * timeStep; // simulate gravitational acceleration
        }
        hit = new RaycastHit();
        return false;
    }
}
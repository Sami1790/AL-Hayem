using UnityEngine;
using System.Collections;
using TMPro;

public class PistolGun : MonoBehaviour
{
    public int ammo = 23;
    public Camera playerCam;
    public float range = 60f;

    public Transform barrelLocation;
    public GameObject muzzleFlash;
    public Transform cameraRoot;
    public TextMeshProUGUI ammoText;

    private Animator anim;
    private Vector3 originalCamPos;
    private float shakeDuration = 0f;
    private float shakeStrength = 0.09f;
    private float shakeFadeSpeed = 8.5f;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator not found! تأكد أن Animator مربوط على M1911 Handgun_Model");

        if (cameraRoot)
            originalCamPos = cameraRoot.localPosition;
    }

    void Start()
    {
        if (anim) anim.ResetTrigger("Fire");
        UpdateAmmoUI();
    }

    void Update()
    {
        // أهم سطر: لا تطلق إذا بانل التولتيب ظاهر
        if (TooltipPanel.TooltipActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ammo > 0)
            {
                Shoot();
                ammo--;
                UpdateAmmoUI();
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayGunShot();
            }
            else
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayGunClick();
            }
        }

        if (cameraRoot)
        {
            if (shakeDuration > 0)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeStrength;
                shakeOffset.z = 0;
                cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCamPos + shakeOffset, Time.deltaTime * shakeFadeSpeed);
                shakeDuration -= Time.deltaTime;
            }
            else
            {
                cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, originalCamPos, Time.deltaTime * shakeFadeSpeed);
            }
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText)
            ammoText.text = ammo.ToString();
    }

    void Shoot()
    {
        // تشغيل الفلاش
        if (muzzleFlash)
            StartCoroutine(DoMuzzleFlash());

        // تشغيل الانميشن
        if (anim)
        {
            anim.ResetTrigger("Fire");
            anim.SetTrigger("Fire");
        }

        // Raycast
        RaycastHit hit;
        Vector3 shootDir = playerCam.transform.forward;
        Vector3 shootOrigin = barrelLocation ? barrelLocation.position : playerCam.transform.position;

        if (Physics.Raycast(shootOrigin, shootDir, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // إذا عليه Tag "Enemy"
            if (hit.transform.CompareTag("Enemy"))
            {
                // جرب كل أنواع السكربتات الممكنة للعدو، ونادي TakeHit إذا لقيته
                var ai1 = hit.transform.GetComponent<EnemyAI>();
                var ai2 = hit.transform.GetComponent<EnemyAI2>();
                var ai3 = hit.transform.GetComponent<EnemyAI3>();
                var ai4 = hit.transform.GetComponent<EnemyAI4>();

                if      (ai1 != null) ai1.TakeHit();
                else if (ai2 != null) ai2.TakeHit();
                else if (ai3 != null) ai3.TakeHit();
                else if (ai4 != null) ai4.TakeHit();
                else Debug.Log("Enemy tag, but no AI script found!");
            }
        }
        else
        {
            Debug.Log("Miss!");
        }

        shakeDuration = 0.18f;
    }

    IEnumerator DoMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        muzzleFlash.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{
    //Gun Stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //Bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Animator anim;
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Grapphics
    //public GameObject muzzleFlash, bulletHoleGraphic;
    //public CamShake camShake;
    //public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI bulletsLeftText;

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Anim
        anim.Play("RevolverRecoil", -1, 0f);

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y);

        //Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);
            
            if (rayHit.collider.CompareTag("Enemy"))   
                rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
        }

        bulletsLeft--;
        bulletsShot--;
        
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        reloading = false;
        bulletsLeft = magazineSize;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();

        //SetText
        bulletsLeftText.SetText(bulletsLeft + " / " + magazineSize);
    }
}
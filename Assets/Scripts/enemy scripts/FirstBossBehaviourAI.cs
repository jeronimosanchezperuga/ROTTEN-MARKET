﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FirstBossBehaviourAI : MonoBehaviour
{
    //setUp
    [Header("Player Reference")]
    private Transform Player;
    public LayerMask capaDelJugador;

    [Header("Ghost body")]
    public GameObject ghostChildren;
    private Transform ghostChildrenTransform;

    //rangos
    [Header("Ranges")]
    public float outerLongRange;
    public float innerLongRange;
    public float meeleRange;
    private bool outerRangeBool, innerRangeBool, meeleRangeBool;

    //fases del enemigo

    private bool isSpawning = true;
    private bool firstFase = false;
    private bool secondFase = false;
    private bool isDying = false;

    //anim
    private bool isAnimating = false;
    private bool isIdling = false;
    private bool isAtacking = false;
    private bool idleOnce = true;


    //Turret
    [Header("Shooting")]
    public Transform shootingPivot;
    public GameObject bulletPrefab;
    public GameObject granadePrefab;
    public float shootingRateOfAttack;
    private float shootingRateOfAttackDelta;
    public float granadeRateOfAttack;
    private float granadeRateOfAttackDelta;

    //spawner
    [Header("Spawning")]
    public GameObject childPrefab;
    public float spawningRate;
    private float spawningRateDelta;

    //teleport
    [Header("Teleport")]
    public float tpAreaX;
    public float tpAreaZ;

    //rutina
    private int rutina;
    private float cronometro;
    private Quaternion angulo;
    private float grado;

    //behaviour
    [Header("Behaviour")]
    private bool canLookToPlayer = true;

    //Vida

    private EnemyHealthBar healthBarController;
    private float health;
    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
        healthBarController = GetComponent<EnemyHealthBar>();

        StartCoroutine(spawnRoutine());

       
    }

    // Update is called once per frame
    void Update()
    {

        ghostChildren.transform.LookAt(Player);   

        //seteo de los rangos
        outerRangeBool = Physics.CheckSphere(transform.position, outerLongRange, capaDelJugador);
        innerRangeBool = Physics.CheckSphere(transform.position, innerLongRange, capaDelJugador);
        meeleRangeBool = Physics.CheckSphere(transform.position, meeleRange, capaDelJugador);

        //chequeo de vida para otorgar una fase
      //  health = healthBarController.currentHealth;

        if(isSpawning == false)
        {

            if (health >= health / 2)
            {
                firstFase = true;
            }
            else if (health <= health / 2)
            {
                firstFase = false;
                secondFase = true;
            }
            else
            {
                firstFase = false;
                secondFase = false;
                isDying = true;
            }

            if (isDying)
            {
                StartCoroutine(isDie());
            }

        }

        //crequea si esta haciendo alguna animacion
        if(isSpawning || isIdling || isAtacking || isDying)
        {
            isAnimating = true;
        }
        else
        {
            isAnimating = false;
        }

        //primera fase
        if (firstFase)
        {
            if (outerRangeBool && !innerRangeBool)
            {
                if (!isAnimating)
                {

                    if (canLookToPlayer)
                    {
                        transform.LookAt(Player);
                    }

                    //spawningChildren
                    spawningRateDelta -= Time.deltaTime;
                    if (spawningRateDelta < 0)
                    {
                        StartCoroutine(spawningAttack());
                        spawningRateDelta = spawningRate;
                    }

                    //bullet-shooting
                    shoot();
                    //granade-spawner
                    granadeRateOfAttackDelta -= Time.timeScale;
                    if (granadeRateOfAttack < 0)
                    {
                        StartCoroutine(granadeShootAttack());
                        granadeRateOfAttackDelta = granadeRateOfAttack;
                    }
                }

            } else if (innerRangeBool)
            {
                if (!isAnimating)
                {

                    //spawningChildren
                    spawningRateDelta -= Time.deltaTime;
                    if (spawningRateDelta < 2)
                    {
                        StartCoroutine(spawningAttack());
                        spawningRateDelta = spawningRate;
                    }

                    //bullet-shooting
                    shootingRateOfAttackDelta -= Time.timeScale;
                    if (shootingRateOfAttack < 0)
                    {
                        StartCoroutine(bulletShootAttack());
                        shootingRateOfAttackDelta = shootingRateOfAttack;
                    }
                }

            }

            if(!outerRangeBool && !innerRangeBool)
            {
                if (idleOnce)
                {
                    StartCoroutine(isIdle());
                }
            }
           
        }

        if (secondFase)
        {
            if (outerRangeBool && !innerRangeBool)
            {

                if (!isAnimating)
                {
                    bossFaseTwoBehaviour();

                    //spawningChildren
                    spawningRateDelta -= Time.deltaTime;
                    spawningRateDelta -= Time.deltaTime;
                    if (spawningRateDelta < 0)
                    {
                        StartCoroutine(spawningAttack());
                        spawningRateDelta = spawningRate;
                    }

                    //bullet-shooting
                    shootingRateOfAttackDelta -= Time.timeScale;
                    shootingRateOfAttackDelta -= Time.timeScale;
                    if (shootingRateOfAttack < 0)
                    {
                        StartCoroutine(bulletShootAttack());
                        shootingRateOfAttackDelta = shootingRateOfAttack;
                    }
                    //granade-spawner
                    granadeRateOfAttackDelta -= Time.timeScale;
                    granadeRateOfAttackDelta -= Time.timeScale;
                    if (granadeRateOfAttack < 0)
                    {
                        StartCoroutine(granadeShootAttack());
                        granadeRateOfAttackDelta = granadeRateOfAttack;
                    }


                }

            }
            else if (innerRangeBool)
            {

            }
            else
            {
                if (idleOnce)
                {
                    StartCoroutine(isIdle());
                }
            }
        }
        

    }

    //bullet-shoot

    private void shoot()
    {
        StartCoroutine(bulletShootAttack());

        GameObject bulletClon;
        bulletClon = Instantiate(bulletPrefab, shootingPivot.transform.position, transform.rotation);
        Destroy(bulletClon, 6);
    }

    IEnumerator bulletShootAttack()
    {
        isAtacking = true;
        gameObject.GetComponent<Animator>().Play("New State");
        
        yield return new WaitForSeconds(1f);
        isAtacking = false;
        gameObject.GetComponent<Animator>().Play("New State");
    }

    //granade-instantiate

    private void granadeShoot()
    {
        StartCoroutine(granadeShootAttack());

        GameObject granadeClon;
        granadeClon = Instantiate(granadePrefab, shootingPivot.transform.position, transform.rotation);
    }

    IEnumerator granadeShootAttack()
    {
        isAtacking = true;
        gameObject.GetComponent<Animator>().Play("New State");

        yield return new WaitForSeconds(6.3f);
        isAtacking = false;
        gameObject.GetComponent<Animator>().Play("New State");
    }

    IEnumerator spawningAttack()
    {
        isAtacking = true;
        gameObject.GetComponent<Animator>().Play("New State");
 
        int childAmount = randomNum();
        for(int i = 0; i <= childAmount; i++)
        {
            float spawningRangeX = Random.Range(-innerLongRange, innerLongRange);
            float spawningPosY = transform.position.y + 2;
            float spawningRangeZ = Random.Range(-innerLongRange, innerLongRange);
            Vector3 spawningArea = new Vector3(spawningRangeX, spawningPosY, spawningRangeZ);

            GameObject children;
            children = Instantiate(childPrefab, spawningArea, Quaternion.identity);
        }

        yield return new WaitForSeconds(2f);
        isAtacking = false;
        gameObject.GetComponent<Animator>().Play("New State");
    }

    IEnumerator teleportBoss()
    {
        isAtacking = true;
        float tpRangeX = Random.Range(-tpAreaX, tpAreaX);
        float tpPosY = transform.position.y;
        float tpRangeZ = Random.Range(-tpAreaZ, tpAreaZ);
        Vector3 spawningPos = new Vector3(tpRangeX, tpPosY, tpRangeZ);

        transform.position = spawningPos;

        
        gameObject.GetComponent<Animator>().Play("New State");

        yield return new WaitForSeconds(3f);

        isAtacking = false;
        gameObject.GetComponent<Animation>().Play("New State");
    }

    //animaciones

    IEnumerator spawnRoutine()
    {
        gameObject.GetComponent<Animator>().Play("New State");

        yield return new WaitForSeconds(5f);

        gameObject.GetComponent<Animator>().Play("New State");
        isSpawning = false;
    }

    IEnumerator isIdle()
    {
        idleOnce = false;
        isIdling = true;
        gameObject.GetComponent<Animator>().Play("idle");

        yield return new WaitForSeconds(1.3f);

        gameObject.GetComponent<Animator>().Play("New State");
        isIdling = false;
        idleOnce = true;
    }

    IEnumerator isDie()
    {
        isDying = true;
        gameObject.GetComponent<Animator>().Play("New State");

        yield return new WaitForSeconds(3f);

        gameObject.GetComponent<Animator>().Play("New State");
        isDying = false;
    }

    //funciones

    private void bossFaseOneBehaviour()
    {
        cronometro += 1 * Time.deltaTime;
        if (cronometro >= 3)
        {
            rutina = Random.Range(0, 9);
            cronometro = Random.Range(-2, 0);
        }

        switch (rutina)
        {
            case 0:
                break;

            case 1:
                canLookToPlayer = true;
                StartCoroutine(spawningAttack());

                break;

            case 2:
                canLookToPlayer = true;
                StartCoroutine(spawningAttack());

                break;

            case 8:
                canLookToPlayer = false;
                grado = Random.Range(0, 360);
                angulo = Quaternion.Euler(0, grado, 0);
                rutina++;

                break;

            case 9:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                transform.Translate(Vector3.forward * 1 * Time.deltaTime);


                break;

            
            

        }
    }

    private void bossFaseTwoBehaviour()
    {
        cronometro += 1 * Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 3);
            cronometro = Random.Range(-2, 0);
        }

        switch (rutina)
        {
            case 0:
                break;

            case 1:
                grado = Random.Range(0, 360);
                angulo = Quaternion.Euler(0, grado, 0);
                rutina++;

                break;

            case 2:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                break;

            case 3:

                bool teleport = true;

                if (teleport)
                {
                    StartCoroutine(teleportBoss());

                    teleport = false;
                }


                break;

        }
    }

    int randomNum()
    {
        int ram = Random.Range(0, 100);
        int result = 0;

        if(ram < 80)
        {
            result = 0;
        }
        else
        {
            result = 1;
        }

        return result;
    }


    //GizmozDraw

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerLongRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerLongRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meeleRange);

    }
}

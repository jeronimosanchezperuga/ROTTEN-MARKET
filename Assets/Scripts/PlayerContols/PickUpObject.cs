﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{

    public GameObject ObjectToPickUp;
    public GameObject PickedObject;
    public Transform[] interactionZone; //Ubicacion donde quiero que quede agarrado el objeto. Poner la mano
    int hand; // hand = 0 -> derecha, hand = 1 -> izquierda
    SFXManager SFX;

    void Start()
    {
        SFX = GameObject.Find("[SFX-MANAGER]").GetComponent<SFXManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Generar un numero aleatorio para saber que mano agarra el objeto
        

        if (ObjectToPickUp != null && ObjectToPickUp.GetComponent<PickableObject>().isPickable == true && PickedObject == null)
        {
            if (Input.GetKeyDown(KeyCode.F))//Si toco la F suceda lo de abajo
            {
                PickedObject = ObjectToPickUp;
                float distanceRight = Vector3.Distance(interactionZone[0].position, PickedObject.transform.position); //Distancia entre la mano derecha y el objeto
                float distanceLeft = Vector3.Distance(interactionZone[1].position, PickedObject.transform.position); //Distancia entre la mano izquierda y el objeto
                if (distanceRight > distanceLeft || PickedObject.name == "Tenedor"|| PickedObject.name == "Cuchillo" || PickedObject.name == "Cuchara") 
                {
                    //Si esta mas cerca la mano derecha, agarra con la mano derecha
                    hand = 1;
                }
                else
                {
                    hand = 0;
                }
                PickedObject.GetComponent<PickableObject>().isPickable = false; //Le avisamos que ya agarramos el objeto
                PickedObject.transform.SetParent(interactionZone[hand]); //Lo parenteamos
                PickedObject.transform.position = interactionZone[hand].position;//Lo ponemos en la posicion de la zona de interaccion o donde queramos
                PickedObject.GetComponent<Rigidbody>().useGravity = false; //Para que lo agarremos y no se nos caiga
                PickedObject.GetComponent<Rigidbody>().isKinematic = true; //Para que no le afecte la fisica
                ObjectToPickUp.GetComponent<PickableObject>().isPicked = true;
                SFX.PlayPickUpSound();

                if (PickedObject.name == "Tenedor")
                {
                    PickedObject.transform.localPosition = new Vector3(-0.0668f, 0.0091f, -0.0023f);
                    PickedObject.transform.localRotation = Quaternion.Euler(-4.554f, 85.15f, -83.033f);
                    PickedObject.transform.localScale = new Vector3(0.005758717f, 0.00575872f, 0.005758714f);
                    //Cambia el collider del hijo del tenedor a trigger
                    PickedObject.transform.GetChild(0).gameObject.GetComponent<MeshCollider>().isTrigger = true;

                }

                if (PickedObject.name == "Cuchillo")
                {
                    PickedObject.transform.localPosition = new Vector3(-0.0585f, -0.0121f, 0.0085f);
                    PickedObject.transform.localRotation = Quaternion.Euler(-12.079f, 96.216f, 270f);
                    PickedObject.transform.localScale = new Vector3(0.005945551f, 0.005945563f, 0.00594556f);
                    PickedObject.transform.GetChild(0).gameObject.GetComponent<MeshCollider>().isTrigger = true;
                }

                if (PickedObject.name == "Cuchara")
                {
                    PickedObject.transform.localPosition = new Vector3(-0.0551f, -0.0269f, -0.0085f);
                    PickedObject.transform.localRotation = Quaternion.Euler(-9.294001f, 86.25101f, -73.23901f);
                    PickedObject.transform.localScale = new Vector3(0.005945551f, 0.005945563f, 0.00594556f);
                    PickedObject.transform.GetChild(0).gameObject.GetComponent<MeshCollider>().isTrigger = true;                    
                }
            }
        }

        else if (PickedObject != null) //Si ya agarramos algo
        {
            if (Input.GetKeyDown(KeyCode.F)) //para soltar
            {

                if (PickedObject.name == "Tenedor" || PickedObject.name == "Cuchillo" || PickedObject.name == "Cuchara")
                {
                    //Cambia el colliders del hijo del tenedor a NO trigger
                    PickedObject.transform.GetChild(0).gameObject.GetComponent<MeshCollider>().isTrigger = false;                    
                }

                PickedObject.GetComponent<PickableObject>().isPicked = false; 
                PickedObject.GetComponent<PickableObject>().isPickable = true;
                PickedObject.transform.SetParent(null);
                PickedObject.transform.position = interactionZone[hand].position;
                PickedObject.GetComponent<Rigidbody>().useGravity = true;
                PickedObject.GetComponent<Rigidbody>().isKinematic = false;
                PickedObject = null;
                SFX.PlayPickUpSound();
            }
        }
    }
}

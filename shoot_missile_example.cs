using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HomingMissile
{
public class shoot_missile_example : MonoBehaviour
{
public GameObject missile_prefab;
public GameObject target;
public void shoot_missile()
{
   GameObject missile= Instantiate(missile_prefab,new Vector3(200,200,200),transform.rotation);
   missile.GetComponent<homing_missile>().target=target;
   missile.GetComponent<homing_missile>().targetpointer.GetComponent<homing_missile_pointer>().target=target;
   missile.GetComponent<homing_missile>().shooter=this.gameObject;
   missile.GetComponent<homing_missile>().usemissile();
}
}
}
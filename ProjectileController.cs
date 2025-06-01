using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using BigRookGames.Weapons;  

namespace BigRookGames.Weapons  
{  
    public class ProjectileController : MonoBehaviour  
    {  
        public bool hasExploded = false;  
        public GameObject boomRangePrefab; // 爆炸范围预制体   
        public float speed = 100;  
        public LayerMask collisionLayerMask;  

        public GameObject rocketExplosion;  
        public MeshRenderer projectileMesh;  
        private bool targetHit;  
        public AudioSource inFlightAudioSource;  
        public ParticleSystem disableOnHit;  

        void Start()  
        {  
            hasExploded = false;  
        }  

        private void Update()  
        {  
            if (targetHit) return;  
            transform.position += transform.forward * (speed * Time.deltaTime);  
        }  

        /// <summary>  
        /// Explodes on trigger.  
        /// </summary>  
        /// <param name="other"></param>  
        private void OnTriggerEnter(Collider other)  
        {  
            // Check if the projectile has already exploded  
            if (hasExploded) return;  

            // Check if the collider's tag is "enemy", "ground", or "Untagged"  
            if (other.CompareTag("enemy") || other.CompareTag("Ground") || other.CompareTag("Untagged"))  
            {  
                Explode(); // Call the explode method  
                projectileMesh.enabled = false;  
                targetHit = true;  
                inFlightAudioSource.Stop();  

                // Disable all colliders on the projectile  
                foreach (Collider col in GetComponents<Collider>())  
                {  
                    col.enabled = false;  
                }  

                disableOnHit.Stop();  

                // Destroy this object after 5 seconds  
                Destroy(gameObject, 1.5f);  
            }  
        }  

        /// <summary>  
        /// Instantiates an explosion object.  
        /// </summary>  
        public void Explode()  
        {  
            GameObject newExplosion = Instantiate(rocketExplosion, transform.position, rocketExplosion.transform.rotation, null);  
            hasExploded = true;  
        }  
    }  
}
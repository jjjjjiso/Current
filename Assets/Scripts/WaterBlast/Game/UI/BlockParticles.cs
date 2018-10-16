using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class BlockParticles : MonoBehaviour
    {
        public ParticleSystem particle = null;

        public void Playing()
        {
            gameObject.SetActive(true);
            StartCoroutine(Co_Playing());
        }

        IEnumerator Co_Playing()
        {
            particle.Play();
            while(particle.isPlaying) yield return null;
            gameObject.GetComponent<PooledObject>().pool.ReturnObject(gameObject);
        }
    }
}
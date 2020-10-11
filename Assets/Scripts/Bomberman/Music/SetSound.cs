using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Bomberman.Bomb;
using UnityEngine;
namespace Bomberman.Character
{
    public class SetSound : MonoBehaviour
    {
        public static AudioClip BombCharge, BombExplosion;
        public static AudioSource AudioSrc;
        
        private void Start()
        {
            AudioSrc = GetComponent<AudioSource>();
            
            BombCharge = Resources.Load<AudioClip>("BombCharge");
            BombExplosion = Resources.Load<AudioClip>("Explosion");
        }

        public static void PlaySound(string Clip)
        {
            switch (Clip)
            {
                case "BombCharge":
                    AudioSrc.PlayOneShot(BombCharge,0.2f);
                    
                    break;
                case "Explosion":
                    AudioSrc.PlayOneShot(BombExplosion,0.1f);
                    break;
            }
        }
    }

}

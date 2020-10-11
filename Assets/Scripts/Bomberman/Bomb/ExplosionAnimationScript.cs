using Bomberman.Character;
using UnityEngine;

namespace Bomberman.Bomb
{
    public class ExplosionAnimationScript : MonoBehaviour
    {
        private SetSound SoundExplosion;
        private void Start()
        {
            ParticleSystem exp = GetComponent<ParticleSystem>();
            exp.Play();
            
            SetSound.PlaySound("Explosion");
            Destroy(gameObject, exp.main.duration);
        }
    }
}

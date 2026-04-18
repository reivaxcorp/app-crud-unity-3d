using UnityEngine;

public class SimpleExplosion : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        // Obtenemos la duración máxima para saber cuándo destruir el objeto
        // Sumamos el startLifetime más largo para asegurarnos de que todas terminen
        float totalDuration = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;

        // Destruimos el objeto entero después de la duración
        Destroy(gameObject, totalDuration);
    }
}
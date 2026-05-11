using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio put box")]
    public AudioSource audioSource;
    public AudioClip buildSound;

    [Header("Background music")]
    [SerializeField] GameObject backgroundMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    private void Awake()
    {
        checkRef();
        backgroundMusic.SetActive(false);
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        backgroundMusic.SetActive(true);
    }

    public void PlayBoxPutEffect()
    {
        // EFECTO ADICTIVO DE SONIDO
        if (audioSource != null && buildSound != null)
        {
            // Variamos el pitch entre 0.9 y 1.1 (un 10% arriba o abajo)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(buildSound);
        }
    }

    private void checkRef()
    {
        if (backgroundMusic == null) { Debug.LogWarning("Coloca la referencia en el inspector de BackgroundMusic"); }
    }
}

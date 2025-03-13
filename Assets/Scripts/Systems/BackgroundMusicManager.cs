using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] backgroundTracks; // Tablica z muzyką

    private int lastTrackIndex = -1; // Zapamiętujemy ostatni utwór, żeby uniknąć powtórzeń

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayRandomMusic());
    }

    IEnumerator PlayRandomMusic()
    {
        while (true)
        {
            if (!audioSource.isPlaying) // Jeśli nic nie gra, wybieramy nowy utwór
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, backgroundTracks.Length);
                } while (randomIndex == lastTrackIndex); // Unikamy powtórzenia tego samego utworu

                lastTrackIndex = randomIndex; // Zapamiętujemy wybrany utwór
                audioSource.clip = backgroundTracks[randomIndex];
                audioSource.Play();
            }
            yield return null; // Czekamy na zakończenie obecnego utworu
        }
    }
}

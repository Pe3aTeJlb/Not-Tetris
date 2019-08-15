using UnityEngine;

public class MusicController : MonoBehaviour
{

    public AudioClip[] music;
    public AudioSource audioSource;
    public int prevTrack;
    int prevs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying == false) { SelectTrack(); }
    }
    void SelectTrack()
    {

        prevs = Random.Range(0, music.Length);
        if (prevTrack != prevs)
        {
            prevTrack = prevs;
            audioSource.clip = music[prevs];
            audioSource.Play();
        }
        else { SelectTrack(); }

    }
}

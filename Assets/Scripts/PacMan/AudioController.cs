using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PacMan
{
    public class AudioController : MonoBehaviour
    {
        // Audio players components.
        public AudioSource effectsSource;
        public AudioSource musicSource;

        public AudioClip[] munches;
        public AudioClip powerMunch;
        public AudioClip aggressiveDeath;
        public AudioClip pincerDeath;
        public AudioClip shyDeath;
        public AudioClip whimsicalDeath;
        public AudioClip aggressivePlayerKill;
        public AudioClip pincerPlayerKill;
        public AudioClip shyPlayerKill;
        public AudioClip whimsicalPlayerKill;

        public AudioClip[] musicInGame;
        public AudioClip musicMenu;
        public AudioClip musicVictory;
        public AudioClip musicFailure;

        // Singleton instance.
        public static AudioController Instance;
        
        public bool PlayingInGameMusic { get; private set; }
        
        
        // Random pitch adjustment range.
        // private float _lowPitchRange = .95f;
        // private float _highPitchRange = 1.05f;
	
        // Initialize the singleton instance.
        private void Awake()
        {
            // If there is not already an instance of SoundManager, set it to this.
            if (Instance == null)
            {
                Instance = this;
            }
            //If an instance already exists, destroy whatever this object is to enforce the singleton.
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad (gameObject);
        }

        // Play a single clip through the sound effects source.
        public void Play(SoundEffect soundEffect) 
        {
            effectsSource.PlayOneShot(SoundClip(soundEffect));
        }

        // Play a single clip through the music source.
        public void PlayMusic(Music music)
        {
            musicSource.loop = true;
            musicSource.volume = 0.7f;
            musicSource.clip = MusicClip(music);
            musicSource.Play();
        }
        
        private AudioClip MunchSoundEffect()
        {
            var randomIndex = Random.Range(0, munches.Length);
            return munches[randomIndex];
        }
        
        private AudioClip InGameMusic()
        {
            var randomIndex = Random.Range(0, musicInGame.Length);
            return musicInGame[randomIndex];
        }
        
        private AudioClip SoundClip(SoundEffect soundEffect)
        {
            switch (soundEffect)
            {
                case SoundEffect.Munch:
                    return MunchSoundEffect();
                case SoundEffect.PowerMunch:
                    return powerMunch;
                case SoundEffect.Reincarnate:
                    return MunchSoundEffect();
                case SoundEffect.Failure:
                    return MunchSoundEffect();
                case SoundEffect.Victory:
                    return MunchSoundEffect();
                case SoundEffect.AggressiveEnemyKilled:
                    return aggressiveDeath;
                case SoundEffect.PincerEnemyKilled:
                    return pincerDeath;
                case SoundEffect.ShyEnemyKilled:
                    return shyDeath;
                case SoundEffect.WhimsicalEnemyKilled:
                    return  whimsicalDeath;
                case SoundEffect.PlayerKilledByAggressive:
                    return aggressivePlayerKill;
                case SoundEffect.PlayerKilledByPincer:
                    return pincerPlayerKill;
                case SoundEffect.PlayerKilledByShy:
                    return  shyPlayerKill;
                case SoundEffect.PlayerKilledByWhimsical:
                    return whimsicalPlayerKill;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundEffect), soundEffect, null);
            }
        }

        private AudioClip MusicClip(Music music)
        {
            switch (music)
            {
                case Music.Menu:
                    PlayingInGameMusic = false;
                    return musicMenu;
                case Music.InGame:
                    PlayingInGameMusic = true;
                    return InGameMusic();
                case Music.Victory:
                    PlayingInGameMusic = false;
                    return musicVictory;
                case Music.Failure:
                    PlayingInGameMusic = false;
                    return musicFailure;
                default:
                    throw new ArgumentOutOfRangeException(nameof(music), music, null);
            }
        }
    }

    public enum SoundEffect
    {
        Munch,
        PowerMunch,
        AggressiveEnemyKilled,
        PincerEnemyKilled,
        ShyEnemyKilled,
        WhimsicalEnemyKilled,
        PlayerKilledByAggressive,
        PlayerKilledByPincer,
        PlayerKilledByShy,
        PlayerKilledByWhimsical,
        Reincarnate,
        Failure,
        Victory
    }

    public enum Music
    {
        Menu,
        InGame,
        Victory,
        Failure
    }
}
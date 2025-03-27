//  Author:
//  Salman Younas <salman.younas0007@gmail.com>
//
//  Copyright (c) 2018 Appic Studio

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonMonoBehavior<AudioManager> {
	
	[SerializeField] private AudioClip btnSound;
	[SerializeField] private AudioClip swooshSound;

	private AudioSource soundsSource;

	protected override void Awake() {
		base.Awake ();
		DontDestroyOnLoad (this.gameObject);

		soundsSource = GetComponent<AudioSource> ();
		soundsSource.volume = PlayerPrefs.GetFloat("SoundsVolume", soundsSource.volume);

	}

	void Start() {
		EnableSound (Prefs.IsSoundOn);
	}

	public void PlayBtnSound() {
		soundsSource.clip = btnSound;
		soundsSource.Play ();
	}

	public void PlaySwooshSound() {
		soundsSource.clip = swooshSound;
		soundsSource.Play ();
	}

	public void EnableSound(bool enable) {
		soundsSource.volume = enable ? 1 : 0;
		Prefs.IsSoundOn = enable;
		PlayerPrefs.SetFloat("SoundsVolume", soundsSource.volume);
	}

	public void ToggleSound() {
		EnableSound (!Prefs.IsSoundOn);
	}

}

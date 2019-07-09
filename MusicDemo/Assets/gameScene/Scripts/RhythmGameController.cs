//----------------------------------------------
//            	   Koreographer                 
//    Copyright © 2014-2019 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System;
namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Rhythm Game/Rhythm Game Controller")]
	public class RhythmGameController : MonoBehaviour
	{
		#region Fields

		[Tooltip("The Event ID of the track to use for target generation.")]
		[EventID]
		public string eventID;
        
        //判定区时间
		[Tooltip("The number of milliseconds (both early and late) within which input will be detected as a Hit.")]
		[Range(8f, 400f)]
		public float hitWindowRangeInMS = 80;

        [Tooltip("perfect区判定范围")]
        public float greatHitRange = 20;

        [Tooltip("good区判定范围")]
        public float goodHitRange = 40;

        [Tooltip("bad区判定范围")]
        public float badHitRange = 60;

        [Tooltip("正向miss区判定范围")]
        public float missHitRange = 80;

        [Tooltip("反向miss区判定范围")]
        public float negativeMissHitRange = 30;
        
        
        //note速度
        [Tooltip("The number of units traversed per second by Note Objects.")]
		public float noteSpeed = 1f;

		[Tooltip("The archetype (blueprints) to use for generating notes.  Can be a prefab.")]
		public NoteObject noteObjectArchetype;

        [Tooltip("Another Object, 可以回血的NOTE")]
        
        public NoteObject HPnoteObject;

		[Tooltip("The list of Lane Controller objects that represent a lane for an event to travel down.")]
		public List<LaneController> noteLanes = new List<LaneController>();

		[Tooltip("The amount of time in seconds to provide before playback of the audio begins.  Changes to this value are not immediately handled during the lead-in phase while playing in the Editor.")]
		public float leadInTime;

		[Tooltip("The Audio Source through which the Koreographed audio will be played.  Be sure to disable 'Auto Play On Awake' in the Music Player.")]
		public AudioSource audioCom;


        public Slider HPslider;


        [Tooltip("HPnote 的比例")]
        [Range(1, 99)]
        public int HPnoteRate=0;

		// The amount of leadInTime left before the audio is audible.
		float leadInTimeLeft;

		// The amount of time left before we should play the audio (handles Event Delay).
		float timeLeftToPlay;

		// Local cache of the Koreography loaded into the Koreographer component.
		Koreography playingKoreo;

		// Koreographer works in samples.  Convert the user-facing values into sample-time.  This will simplify
		//  calculations throughout.
		int hitWindowRangeInSamples;    // The sample range within which a viable event may be hit. 检测点击判定
        int badRangeSamples;
        int goodRangeSamples;
        int perfectRangeSamples;
        int missRangeSamples;


        float starttime =0;
        bool isPlay = false;

		// The pool for containing note objects to reduce unnecessary Instantiation/Destruction.
		Stack<NoteObject> noteObjectPool = new Stack<NoteObject>();
        GlobalBGM globalBGM;
        //当前积分
        public int score;

		#endregion
		#region Properties

		// Public access to the hit window. (miss到bad区)
		public int HitWindowSampleWidth
		{
			get
			{
				return hitWindowRangeInSamples;
			}
		}
        //返回MISS区
        public int MissRangeSampleWidth
        {
            get
            {
                return missRangeSamples;
            }
        }


        //返回bad区
        public int BadRangeSamplesWidth
        {
            get
            {
                return badRangeSamples;
            }
        }
        //返回good区
        public int GoodRangeSamplesWidth
        {
            get
            {
                return goodRangeSamples;
            }
        }
        //返回perfect区
        public int PerfectRangeSamplesWidth
        {
            get
            {
                return perfectRangeSamples;
            }
        }



        // Access to the current hit window size in Unity units.
        public float WindowSizeInUnits
		{
			get
			{
                //return noteSpeed * (noteSpeed * 0.001f);
                return 8 * (70 * 0.001f);
            }
		}

        //获取notespeed
        public float getNoteSpeed
        {
            get
            {
                return noteSpeed;
            }
        }
        //更改noteSpeed
        public void setNoteSpeed(float newSpeed)
        {
            noteSpeed = newSpeed;

        }


		// The Sample Rate specified by the Koreography.
		public int SampleRate
		{
			get
			{
				return playingKoreo.SampleRate;
			}
		}

		// The current sample time, including any necessary delays.
		public int DelayedSampleTime
		{
			get
			{
				// Offset the time reported by Koreographer by a possible leadInTime amount.
				return playingKoreo.GetLatestSampleTime() - (int)(audioCom.pitch * leadInTimeLeft * SampleRate);
                
			}
		}
        

		#endregion
		#region Methods

        
		void Start()
		{
            bool isNozomi = GlobalControl.Instance.NozomiMode;
            HPSliderCtrl hPSliderCtrl = GameObject.Find("HPSlider").GetComponent<HPSliderCtrl>();
            //isNozomi = ture 则更改判定区，血量，分数unit
            if (isNozomi)
            {
                greatHitRange = 10;
                goodHitRange = 30;
                badHitRange = 50;
                missHitRange = 60;
                hPSliderCtrl.HPslider = HPslider;
                hPSliderCtrl.NozomiModeInit();


            }

            InitializeLeadIn();
            
			// Initialize all the Lanes.初始化每条轨道
            // notelanes -> 六条轨道
			for (int i = 0; i < noteLanes.Count; ++i)
			{
				noteLanes[i].Initialize(this);
			}
            int chosedKoreoNum = GlobalControl.Instance.chosedNumber;
            //初始化选择的乐曲
            
            audioCom.clip = GlobalControl.Instance.audios[chosedKoreoNum];
            // Initialize events.  使用globalcontrol初始化koreographer
            //playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            playingKoreo = GlobalControl.Instance.songs[chosedKoreoNum];

            ComboCount comboCount = GameObject.Find("ComboNum").GetComponent<ComboCount>();

            // Grab all the events out of the Koreography.

            eventID = GlobalControl.Instance.eventID;
            print(eventID);
            KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID);
			List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents();

			for (int i = 0; i < rawEvents.Count; ++i)
			{
				KoreographyEvent evt = rawEvents[i];
				string payload = evt.GetTextValue();
				
				// Find the right lane.
				for (int j = 0; j < noteLanes.Count; ++j)
				{
					LaneController lane = noteLanes[j];
					if (lane.DoesMatchPayload(payload))
					{
						// Add the object for input tracking.
						lane.AddEventToLane(evt);

						// Break out of the lane searching loop.
						break;
					}
				}
			}
            //延时设定isPlay = true;
            Invoke("setIsPlayTrue", leadInTime);
		}


        void setIsPlayTrue()
        {
            isPlay = true;
        }

		// Sets up the lead-in-time.  Begins audio playback immediately if the specified lead-in-time is zero.
		void InitializeLeadIn()
		{
			// Initialize the lead-in-time only if one is specified.
			if (leadInTime > 0f)
			{
				// Set us up to delay the beginning of playback.
				leadInTimeLeft = leadInTime;
				timeLeftToPlay = leadInTime - Koreographer.Instance.EventDelayInSeconds;
			}
			else
			{
				// Play immediately and handle offsetting into the song.  Negative zero is the same as
				//  zero so this is not an issue.
				audioCom.time = -leadInTime;
				audioCom.Play();
			}
            //初始化分数
            score = 0;
            //初始化notespeed
            
            noteSpeed = GlobalControl.Instance.noteSpeed;


            //关闭globalBGM
            globalBGM = GameObject.Find("GlobalBGM").GetComponent<GlobalBGM>();
            if (globalBGM.getIsPlay == true)
            {
                globalBGM.stopPlayBGM();
            }
		}
        
		void Update()
		{
			// This should be done in Start().  We do it here to allow for testing with Inspector modifications.
			UpdateInternalValues();

			// Count down some of our lead-in-time.
			if (leadInTimeLeft > 0f)
			{
				leadInTimeLeft = Mathf.Max(leadInTimeLeft - Time.unscaledDeltaTime, 0f);
			}

            
            



			// Count down the time left to play, if necessary.
			if (timeLeftToPlay > 0f)
			{
				timeLeftToPlay -= Time.unscaledDeltaTime;

				// Check if it is time to begin playback.
				if (timeLeftToPlay <= 0f)
				{
					audioCom.time = -timeLeftToPlay;
					audioCom.Play();

					timeLeftToPlay = 0f;
				}
			}

            //如果音乐播放结束，延时五秒跳转complete scene
            
            if(isPlay == true && audioCom.isPlaying == false)
            {
                //检查播放情况
                print("complete");
                checkAudioPlay();
            }
            
        }

        //检测audio的播放，如果播放停止，3秒后跳转
        void checkAudioPlay()
        {
            if(audioCom.isPlaying == false)
            {
                
                Invoke("GoToCompleteSongScene", 3);
            }
        }

        public void GoToCompleteSongScene()
        {
            Application.LoadLevel("completeSong");
        }
       


        // Update any internal values that depend on externally accessible fields (public or Inspector-driven).
        void UpdateInternalValues()
		{
			hitWindowRangeInSamples = (int)(0.001f * hitWindowRangeInMS * SampleRate);
            missRangeSamples = (int)(0.001f * negativeMissHitRange * SampleRate);
            badRangeSamples = (int)(0.001f * badHitRange * SampleRate);
            goodRangeSamples = (int)(0.001f * goodHitRange * SampleRate);
            perfectRangeSamples = (int)(0.001f * greatHitRange * SampleRate);

        }
            

		// Retrieves a frehsly activated Note Object from the pool.
		public NoteObject GetFreshNoteObject()
		{
			NoteObject retObj;

			if (noteObjectPool.Count > 0)
			{
				retObj = noteObjectPool.Pop();
			}
			else
			{
                //随机生成回血note
                int n = UnityEngine.Random.Range(1, 100);
                if (n < HPnoteRate)
                {
                    retObj = GameObject.Instantiate<NoteObject>(HPnoteObject);
                }
                else
                {
                    retObj = GameObject.Instantiate<NoteObject>(noteObjectArchetype);
                }
			}
			
			retObj.gameObject.SetActive(true);
			retObj.enabled = true;

			return retObj;
		}

		// Deactivates and returns a Note Object to the pool. note触碰判定区
		public void ReturnNoteObjectToPool(NoteObject obj)
		{
			if (obj != null)
			{
				obj.enabled = false;
				obj.gameObject.SetActive(false);

				noteObjectPool.Push(obj);
			}
		}

		// Restarts the game, causing all Lanes and any active Note Objects to reset or otherwise clear.
		public void Restart()
		{
			// Reset the audio.
			audioCom.Stop();
			audioCom.time = 0f;

			// Flush the queue of delayed event updates.  This effectively resets the Koreography and ensures that
			//  delayed events that haven't been sent yet do not continue to be sent.
			Koreographer.Instance.FlushDelayQueue(playingKoreo);

			// Reset the Koreography time.  This is usually handled by loading the Koreography.  As we're simply
			//  restarting, we need to handle this ourselves.
			playingKoreo.ResetTimings();

			// Reset all the lanes so that tracking starts over.
			for (int i = 0; i < noteLanes.Count; ++i)
			{
				noteLanes[i].Restart();
			}

			// Reinitialize the lead-in-timing.
			InitializeLeadIn();
		}

       

        #endregion
    }
}

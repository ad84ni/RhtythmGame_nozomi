//----------------------------------------------
//            	   Koreographer                 
//    Copyright © 2014-2019 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Rhythm Game/Lane Controller")]
	public class LaneController : MonoBehaviour
	{
		#region Fields

		[Tooltip("The Color of Note Objects and Buttons in this Lane.")]
		public Color color = Color.blue;

		[Tooltip("A reference to the visuals for the \"target\" location.")]
		public SpriteRenderer targetVisuals;

		[Tooltip("The Keyboard Button used by this lane.")]
		public KeyCode keyboardButton;



        [Tooltip("A list of Payload strings that Koreography Events will contain for this Lane.")]
		public List<string> matchedPayloads = new List<string>();

		// The list that will contain all events in this lane.  These are added by the Rhythm Game Controller.
		List<KoreographyEvent> laneEvents = new List<KoreographyEvent>();

		// A Queue that contains all of the Note Objects currently active (on-screen) within this lane.  Input and
		//  lifetime validity checks are tracked with operations on this Queue.
		Queue<NoteObject> trackedNotes = new Queue<NoteObject>();

		// A reference to the Rythm Game Controller.  Provides access to the NoteObject pool and other parameters.
		RhythmGameController gameController;
        

        
		// Lifetime boundaries.  This game goes from the top of the screen to the bottom.
		float spawnY = 0f;
		float despawnY = 0f;

        //积分器 combo计数器,判定显示文本
        ScoreCount scoreCountTimer;
        ComboCount comboCount;
        HitJudgement hitText;

        HPSliderCtrl hPSlider;

        public int score=0;
        NoteHitSound noteHitSound;

		// Index of the next event to check for spawn timing in this lane.
		int pendingEventIdx = 0;

        
		// Feedback Scales used for resizing the buttons on press.
		Vector3 defaultScale;
		float scaleNormal = 1f;
		float scalePress = 1.4f;
		float scaleHold = 1.2f;

		#endregion
		#region Properties

		// The position at which new Note Objects should spawn.
		public Vector3 SpawnPosition
		{
			get
			{
				return new Vector3(transform.position.x, spawnY);
			}
		}

		// The position at which the timing target exists.
		public Vector3 TargetPosition
		{
			get
			{
				return new Vector3(transform.position.x, transform.position.y);
			}
		}

		// The position at which Note Objects should despawn and return to the pool.
        //miss的地方
		public float DespawnY
		{
			get
			{
				return despawnY;
			}
		}
        
        public int CurrentScore
        {
            get
            {
                return score;
            }
        }
        #endregion
        #region Methods




        public void Initialize(RhythmGameController controller)
		{
			gameController = controller;
            score = 0;

            //初始化全局note计数器
            for (int i =0;i<5;i++)
            {
                GlobalControl.Instance.notes[i] = 0;
            }
            //GameObject.Find("ScoreCount").SendMessage("UpdateScore",score);
            //初始化分数计数器，判定文本和combo计数器
            scoreCountTimer = GameObject.Find("scoreText").GetComponent<ScoreCount>();
            hitText = GameObject.Find("HitJudgementText").GetComponent<HitJudgement>();
            comboCount = GameObject.Find("ComboNum").GetComponent<ComboCount>();
            hPSlider = GameObject.Find("HPSlider").GetComponent<HPSliderCtrl>();
            noteHitSound = GameObject.Find("NoteSound").GetComponent<NoteHitSound>();

        }

		// This method controls cleanup, resetting the internals to a fresh state.
		public void Restart(int newSampleTime = 0)
		{
            comboCount.ResetComboAndNote();
            
			// Find the index of the first event at or beyond the target sample time.
			for (int i = 0; i < laneEvents.Count; ++i)
			{
				if (laneEvents[i].StartSample >= newSampleTime)
				{
					pendingEventIdx = i;
					break;
				}
			}

			// Clear out the tracked notes.
			int numToClear = trackedNotes.Count;
			for (int i = 0; i < numToClear; ++i)
			{
				trackedNotes.Dequeue().OnClear();
			}
		}

		void Start()
		{
			// Get the vertical bounds of the camera.  Offset by a bit to allow for offscreen spawning/removal.
			float cameraOffsetZ = -Camera.main.transform.position.z;
			spawnY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, cameraOffsetZ)).y + 1f;
			despawnY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, cameraOffsetZ)).y - 1f;

			// Update our visual color.
			targetVisuals.color = color;

			// Capture the default scale set in the Inspector.
			defaultScale = targetVisuals.transform.localScale;
		}


        public void GoToCompleteSongScene()
        {
            Application.LoadLevel("completeSong");
        }


        void Update()
		{
			// Clear out invalid entries.
            //判定miss的note
			while (trackedNotes.Count > 0 && trackedNotes.Peek().IsNoteMissed())
			{
				
                CheckNoteMiss();
               

            }

            

            // Check for new spawns.
            CheckSpawnNext();

			// Check for input.  Note that touch controls are handled by the Event System, which is all
			//  configured within the Inspector on the buttons themselves, using the same functions as
			//  what is found here.  Touch input does not have a built-in concept of "Held", so it is not
			//  currently supported.
			if (Input.GetKeyDown(keyboardButton))
			{
				CheckNoteHit();
                //CheckNoteMiss();
                SetScalePress();
			}
			else if (Input.GetKey(keyboardButton))
			{
				SetScaleHold();
			}
			else if (Input.GetKeyUp(keyboardButton))
			{
				SetScaleDefault();
			}

            
		}

		// Adjusts the scale with a multiplier against the default scale.
		void AdjustScale(float multiplier)
		{
			targetVisuals.transform.localScale = defaultScale * multiplier;
		}

		// Uses the Target position and the current Note Object speed to determine the audio sample
		//  "position" of the spawn location.  This value is relative to the audio sample position at
		//  the Target position (the "now" time).
		int GetSpawnSampleOffset()
		{
			// Given the current speed, what is the sample offset of our current.
			float spawnDistToTarget = spawnY - transform.position.y;
			
			// At the current speed, what is the time to the location?
			double spawnSecsToTarget = (double)spawnDistToTarget / (double)gameController.noteSpeed;
			
			// Figure out the samples to the target.
			return (int)(spawnSecsToTarget * gameController.SampleRate);
		}

		// Checks if a Note Object is hit.  If one is, it will perform the Hit and remove the object
		//  from the trackedNotes Queue. 并且更新分数
		public void CheckNoteHit()
		{
            int Notenum = trackedNotes.Count;
            //如果只是空按的话播放点击音
            if (Notenum == 0)
            {
                noteHitSound.OnNoteHitted(true);
                return;
            }
            //是否进入判定区
            bool isHit = trackedNotes.Peek().IsNoteHittable();
            //是否miss
            bool isMiss = trackedNotes.Peek().IsNoteMissed();
            // Always check only the first event as we clear out missed entries before. 进入判定区且不是miss
            

            if (isHit == true && isMiss == false && Notenum > 0)
			{
                //打击判定  返回类型，-1 不计为note 0 miss, 1 bad, 2 good 3 great 4 perfect
                //判定点击类型
                NoteObject hitNote = trackedNotes.Dequeue();
                int type = hitNote.NoteHitType();
                
                //globalcontrol的note计数器+1
                GlobalControl.Instance.notes[type] += 1;
                int addScore =  type* 100;
                print(addScore);
                hitText.ChangeHitJudgementText(type);
                comboCount.UpdateComboNum(type);
                
                if(hitNote.noteType == "hpnote" )
                {
                    hPSlider.HealNoteHitted(type);
                }
                

                //更改分数
                score = scoreCountTimer.score;
                score = score + addScore;
                scoreCountTimer.UpdateScore(score);
                hitNote.OnHit(true);
			}
            //进入note判定区且为miss的note
            
		}

        //用于每帧判断是否miss，note在判定线上方时不存在miss，只有反向miss
        public void CheckNoteMiss()
        {
            int Notenum = trackedNotes.Count;
            if(Notenum == 0) { return; }
            //是否进入判定区
            bool isHit = trackedNotes.Peek().IsNoteHittable();
            //是否miss
            bool isMiss = trackedNotes.Peek().IsNoteMissed();
            
            
            if (Notenum > 0 &&  isMiss == true )
            {
                print("miss object");
                NoteObject hitNote = trackedNotes.Dequeue();
                int type = 0;
                GlobalControl.Instance.notes[type] += 1;

                hitText.ChangeHitJudgementText(type);
                comboCount.UpdateComboNum(type);
                print("update combo");
                //减10%血量
                if (hitNote.noteType == "hpnote")
                {
                    hPSlider.HealNoteHitted(type);
                }
                //获取notehitsound播放器与HP滑条hpslider  
                hPSlider.MissNote();
                hitNote.OnHit(false);

            }
        }
      


		// Checks if the next Note Object should be spawned.  If so, it will spawn the Note Object and
		//  add it to the trackedNotes Queue.
		void CheckSpawnNext()
		{
			int samplesToTarget = GetSpawnSampleOffset();
			
			int currentTime = gameController.DelayedSampleTime;
			
			// Spawn for all events within range.
			while (pendingEventIdx < laneEvents.Count &&
			       laneEvents[pendingEventIdx].StartSample < currentTime + samplesToTarget)
			{
				KoreographyEvent evt = laneEvents[pendingEventIdx];
				
				NoteObject newObj = gameController.GetFreshNoteObject();
				newObj.Initialize(evt, color, this, gameController);
                //加入新生成的note
				trackedNotes.Enqueue(newObj);
				
				pendingEventIdx++;
			}
		}

		// Adds a KoreographyEvent to the Lane.  The KoreographyEvent contains the timing information
		//  that defines when a Note Object should appear on screen.
		public void AddEventToLane(KoreographyEvent evt)
		{
			laneEvents.Add(evt);
		}

		// Checks to see if the string value passed in matches any of the configured values specified
		//  in the matchedPayloads List.
		public bool DoesMatchPayload(string payload)
		{
			bool bMatched = false;

			for (int i = 0; i < matchedPayloads.Count; ++i)
			{
				if (payload == matchedPayloads[i])
				{
					bMatched = true;
					break;
				}
			}

			return bMatched;
		}

		// Sets the Target scale to the original default scale.
		public void SetScaleDefault()
		{
			AdjustScale(scaleNormal);
		}

		// Sets the Target scale to the specified "initially pressed" scale.
		public void SetScalePress()
		{
			AdjustScale(scalePress);
		}

		// Sets the Target scale to the specified "continuously held" scale.
		public void SetScaleHold()
		{
			AdjustScale(scaleHold);
		}
		
		#endregion
	}
}

//----------------------------------------------
//            	   Koreographer                 
//    Copyright © 2014-2019 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
	[AddComponentMenu("Koreographer/Demos/Rhythm Game/Note Object")]
	public class NoteObject : MonoBehaviour
	{
		#region Fields

		[Tooltip("The visual to use for this Note Object.")]
		public SpriteRenderer visuals;

		// If active, the KoreographyEvent that this Note Object wraps.  Contains the relevant timing information.
		KoreographyEvent trackedEvent;
        
		// If active, the Lane Controller that this Note Object is contained by.
		LaneController laneController;

		// If active, the Rhythm Game Controller that controls the game this Note Object is found within.
		RhythmGameController gameController;
        
        //note音效
        NoteHitSound noteHitSound;

        public string noteType;
		#endregion
		#region Static Methods
		
		// Unclamped Lerp.  Same as Vector3.Lerp without the [0.0-1.0] clamping.
		static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			return new Vector3 (from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		#endregion
		#region Methods

		// Prepares the Note Object for use. 初始化
		public void Initialize(KoreographyEvent evt, Color color, LaneController laneCont, RhythmGameController gameCont)
		{
			trackedEvent = evt;
			visuals.color = color;
			laneController = laneCont;
			gameController = gameCont;
            noteHitSound = GameObject.Find("NoteSound").GetComponent<NoteHitSound>();
            UpdatePosition();
		}

		// Resets the Note Object to its default state. 重置
		void Reset()
		{
			trackedEvent = null;
			laneController = null;
			gameController = null;
		}

		void Update()
		{
			UpdateHeight();

			UpdatePosition();

			if (transform.position.y <= laneController.DespawnY)
			{
				gameController.ReturnNoteObjectToPool(this);
				Reset();
			}
		}

		// Updates the height of the Note Object.  This is relative to the speed at which the notes fall and 
		//  the specified Hit Window range.
		void UpdateHeight()
		{
			float baseUnitHeight = visuals.sprite.rect.height / visuals.sprite.pixelsPerUnit;
            //note的大小 
            //float targetUnitHeight = gameController.WindowSizeInUnits * 2f;	// Double it for before/after.

            float targetUnitHeight = gameController.WindowSizeInUnits * 2f; // Double it for before/after.


            Vector3 scale = transform.localScale;
			scale.y = targetUnitHeight / baseUnitHeight;	
			transform.localScale = scale;
		}

		// Updates the position of the Note Object along the Lane based on current audio position.
		void UpdatePosition()
		{
			// Get the number of samples we traverse given the current speed in Units-Per-Second.
			float samplesPerUnit = gameController.SampleRate / gameController.noteSpeed;

			// Our position is offset by the distance from the target in world coordinates.  This depends on
			//  the distance from "perfect time" in samples (the time of the Koreography Event!).
			Vector3 pos = laneController.TargetPosition;
			pos.y -= (gameController.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit;
			transform.position = pos;
		}

		// Checks to see if the Note Object is currently hittable or not based on current audio sample
		//  position and the configured hit window width in samples (this window used during checks for both
		//  before/after the specific sample time of the Note Object). 判断note是否进入判定区
		public bool IsNoteHittable()
		{
			int noteTime = trackedEvent.StartSample;
			int curTime = gameController.DelayedSampleTime;
            // hitWindow就是RhythmGameController中的判定区时间（默认80ms）
			int hitWindow = gameController.HitWindowSampleWidth;

			return (Mathf.Abs(noteTime - curTime) <= hitWindow);
		}

		// Checks to see if the note is no longer hittable based on the configured hit window width in
		//  samples.
		public bool IsNoteMissed()
		{
			bool bMissed = false;

			if (enabled)
			{
				int noteTime = trackedEvent.StartSample;
				int curTime = gameController.DelayedSampleTime;
                int missWindow = gameController.MissRangeSampleWidth ;
				int hitWindow = gameController.HitWindowSampleWidth;
                if(noteTime - curTime < ((-1) * (missWindow)) && ((noteTime - curTime) < 0))
                {
                    print("miss happens");
                }
                //进入negative miss区
                bMissed = ((noteTime - curTime) <= -missWindow) && ((noteTime - curTime) < 0);
                return bMissed;
            }
			
			return bMissed;
		}
        //判定是否为bad
        public bool IsNoteBad()
        {
            bool bBad = true;
            if(enabled)
            {
                int noteTime = trackedEvent.StartSample;
                int curTime = gameController.DelayedSampleTime;
                int badWindow = gameController.BadRangeSamplesWidth;
                int missWindow = gameController.HitWindowSampleWidth;

                bBad = (curTime - noteTime >= badWindow) && (curTime - noteTime < missWindow);

            }
            return bBad;

        }
        //判定类型
        public int NoteHitType()
        {
            int type = -1;
            if (enabled)
            {
                //被检测为hit的最大范围
                int hitWindow = gameController.HitWindowSampleWidth;
                int noteTime = trackedEvent.StartSample;
                int curTime = gameController.DelayedSampleTime;
                //miss,bad,good,great,perfect区
                int missWindow = gameController.HitWindowSampleWidth;
                int badWindow = gameController.BadRangeSamplesWidth;
                int goodWindow = gameController.GoodRangeSamplesWidth;
                int perfectWindow = gameController.PerfectRangeSamplesWidth;


                int result = System.Math.Abs(curTime - noteTime);
                if (result >= missWindow && result < hitWindow) { type = 0; }
                else if (result < missWindow && result >= badWindow) { type = 1; }
                else if (result < badWindow && result >= goodWindow) { type = 2; }
                else if (result < goodWindow && result >= perfectWindow) { type = 3; }
                else if (result < perfectWindow && result >= 0) { type = 4; }
                else { type = -1; }
                //返回类型，0 miss, 1 bad, 2 good 3 great 4 perfect
                return type;
            }
            return type;
        }




		// Returns this Note Object to the pool which is controlled by the Rhythm Game Controller.  This
		//  helps reduce runtime allocations.
		void ReturnToPool()
		{
			gameController.ReturnNoteObjectToPool(this);
			Reset();
		}

		// Performs actions when the Note Object is hit.
		public void OnHit(bool isHit)
		{
			ReturnToPool();
            noteHitSound.OnNoteHitted(isHit);

        }
        //空点屏幕 hitNode = false 意味着note还没到判定区，不做miss判定
        public void OnMissHit(bool hitNote)
        {
            if(hitNote == false)
            {
                noteHitSound.OnNoteHitted(true);
            }
            else
            {
                noteHitSound.OnNoteHitted(false);
            }

        }


		// Performs actions when the Note Object is cleared.
		public void OnClear()
		{
			ReturnToPool();
		}

		#endregion
	}
}

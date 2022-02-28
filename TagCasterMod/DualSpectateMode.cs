using Events.CarCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace TagCasterMod
{
    class DualSpectateMode
    {
        static bool dualSpectateActive = false;

        static SpectatorLogicHack storedSpectatorCam;

        public static void ActivateDualSpectate(Entry e, CarCamera initialCarCam, SpectatorCameraLogic spc)
        {
            if (dualSpectateActive)
            {
                Console.WriteLine("Dual spectate already active!");
                return;
            }


            Console.WriteLine("Dual spectate mode is being activated");
            dualSpectateActive = true;

            var pdf = PlayerDataFake.InitializeFakePlayerData(G.Sys.ProfileManager_.CurrentProfile_, false);
            pdf.carCamera_.isSpectating_ = true;
            pdf.carCamera_.camera_.cullingMask |= 1 << 1;
            pdf.carCamera_.carStats_ = initialCarCam.carStats_;
            pdf.carCamera_.activeCameraMode_ = initialCarCam.activeCameraMode_;
            pdf.carCamera_.profile_ = G.Sys.ProfileManager_.CurrentProfile_;

            var specLogicHack = pdf.carCamera_.GetOrAddComponent<SpectatorLogicHack>();
            specLogicHack.carCamera_ = pdf.carCamera_;

            storedSpectatorCam = specLogicHack;


            initialCarCam.camera_.rect = new Rect(0, 0, 0.5f, 1);
            pdf.CarCamera_.camera_.rect = new Rect(0.5f, 0, 0.5f, 1);

        }

        public static void Reset()
        {
            storedSpectatorCam = null;
            dualSpectateActive = false;
        }

        static float showNameFor = 0.0f;
        internal static void ShowPlayerNamesTemporarily(string to)
        {
            Entry.watermark.text = $"p2 view set to: {to}";
            showNameFor = 2.0f;
        }
       
        public static void UpdateDualSpectate()
        {
            if (storedSpectatorCam && dualSpectateActive)
            {
                if (showNameFor <= 0.0f)
                {
                    showNameFor = 0.0f;
                    Entry.watermark.text = "";
                }
                else
                {
                    showNameFor -= Time.deltaTime;
                }
            }
        }
    }

    class SpectatorLogicHack : SpectatorCameraLogic
    {
        new void Awake()
        {
            this.myInput_ = new InputStates();
        }

        new void Start()
        {
            this.currentMode_ = G.Sys.GameManager_.Mode_;
            this.carCamera_.StartSpectating();
        }

        new void Update()
        {
            if (!this.carCamera_.IsSpectating_) return;

            if ((Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Period)) || this.target_ == null || this.carCamera_.Target_ == null)
            {
                try
                {
                    FindNextTarget();
                }
                catch (ArgumentException e)
                {
                    // nothing, just want to handle the error
                    // since tbh it doesn't look like anything breaks
                }
                finally
                {
                    Console.WriteLine($"[Dual Spectate] Set p2 view to {this.target_.carLogic_.playerData_.name_}");
                    DualSpectateMode.ShowPlayerNamesTemporarily(this.target_.carLogic_.playerData_.name_);
                }

            }
        }
    }
}

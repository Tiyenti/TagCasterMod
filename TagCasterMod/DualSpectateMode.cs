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

        public static void ActivateDualSpectate(CarCamera initialCarCam, SpectatorCameraLogic spc)
        {
            if (dualSpectateActive)
            {
                Console.WriteLine("Dual spectate already active!");
                return;
            }


            Console.WriteLine("Dual spectate mode is being activated");

            var secondCamB = new GameObject();
            var secondCam = secondCamB.AddComponent<Camera>();
            var secondCarCam = secondCamB.AddComponent<CarCamera>();
            secondCarCam.cameraModes_ = initialCarCam.cameraModes_;
            secondCarCam.activeCameraMode_ = initialCarCam.activeCameraMode_;
            secondCarCam.playerDataOwner_ = initialCarCam.playerDataOwner_;
            
            var secondCamLogic = secondCamB.AddComponent<SpectatorCameraLogic>();
            secondCamLogic.target_ = spc.target_;
            
            secondCamB.AddComponent<VREffectController>();
            var lod = secondCamB.AddComponent<CarLevelOfDetail>();
            lod.AddWatcher(secondCarCam);
            secondCamB.AddComponent<PointOfInterestCamera>();
            secondCamB.AddComponent<CameraShake>();
            secondCamB.AddComponent<DepthOfField>();

            initialCarCam.camera_.rect = new Rect(0, 0, 0.5f, 1);
            secondCam.rect = new Rect(0.5f, 0, 0.5f, 1);

            dualSpectateActive = true;

        }
    }
}

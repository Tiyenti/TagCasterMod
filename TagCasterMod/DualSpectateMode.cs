using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TagCasterMod
{
    class DualSpectateMode
    {
        static bool dualSpectateActive = false;

        public static void ActivateDualSpectate(CarCamera initialCarCam)
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
            secondCarCam.playerDataOwner_ = new PlayerDataLocal();
            
            var secondCamLogic = secondCamB.AddComponent<SpectatorCameraLogic>();
            
            secondCamB.AddComponent<VREffectController>();

            initialCarCam.camera_.rect = new Rect(0, 0, 0.5f, 1);
            secondCam.rect = new Rect(0.5f, 0, 0.5f, 1);

            dualSpectateActive = true;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TagCasterMod
{
    class DualSpectateMode
    {


        public static void ActivateDualSpectate(Camera initialCarCam)
        {
            var secondCamB = new GameObject();
            var secondCam = secondCamB.AddComponent<Camera>();
            var secondCarCam = secondCamB.AddComponent<CarCamera>();
            var secondCamLogic = secondCamB.AddComponent<SpectatorCameraLogic>();
            
            secondCamB.AddComponent<VREffectController>();

            initialCarCam.rect = new Rect(0, 0, 0.5f, 1);
            secondCam.rect = new Rect(0.5f, 0, 0.5f, 1);

        }
    }
}

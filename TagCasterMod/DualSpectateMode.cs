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

        static Camera secondCam;

        public static void ActivateDualSpectate(Entry e, CarCamera initialCarCam, SpectatorCameraLogic spc)
        {
            if (dualSpectateActive)
            {
                Console.WriteLine("Dual spectate already active!");
                return;
            }


            Console.WriteLine("Dual spectate mode is being activated");
            dualSpectateActive = true;

            var secondCamObj = UnityEngine.Object.Instantiate(initialCarCam.gameObject);
            secondCam = secondCamObj.GetComponent<Camera>();

            initialCarCam.camera_.rect = new Rect(0, 0, 0.5f, 1);
            secondCam.rect = new Rect(1f, 0, 0.5f, 1);

            secondCamObj.RemoveComponent<AdjustRadialBlur>();
            secondCamObj.RemoveComponent<PlayerSpecificRenderingCamera>();

        }

        public static void UpdateDualSpectate()
        {
            if (secondCam && dualSpectateActive)
            {
                secondCam.Render();
            }
        }
    }
}

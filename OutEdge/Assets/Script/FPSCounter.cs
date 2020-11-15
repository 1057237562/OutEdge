using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController;

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof(Text))]
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        public static int m_FpsAccumulator = 0;
        public static float m_FpsNextPeriod = 0;
        public static int m_CurrentFps;
        const string display = "{0} FPS";
        public float averageFPS;
        private Text m_Text;
        //public GameObject track;
        
        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            m_Text = GetComponent<Text>();
        }


        public void Update()
        {
            // measure average frames per second
            if (transform.parent.GetComponent<Canvas>().enabled)
            {
                m_FpsAccumulator++;
                if (Time.realtimeSinceStartup > m_FpsNextPeriod)
                {
                    m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                    averageFPS = (averageFPS * (Time.realtimeSinceStartup / fpsMeasurePeriod - 1) + m_CurrentFps) / (Time.realtimeSinceStartup / fpsMeasurePeriod);
                    m_FpsAccumulator = 0;
                    m_FpsNextPeriod += fpsMeasurePeriod;
                    m_Text.text = string.Format(display, m_CurrentFps) + Environment.NewLine +"X:"+(int)rfpc.transform.position.x + " Y:"+ (int)rfpc.transform.position.y + " Z:"+(int)rfpc.transform.position.z;
                    if (averageFPS - m_CurrentFps < averageFPS / 4)
                    {
                        Loom.Current.desireloaded = Mathf.Max((int)Math.Round(Loom.Current.currentloaded + 1, MidpointRounding.AwayFromZero), Loom.Current.desireloaded);
                        Loom.Current.currentloaded = 0;
                    }
                }
            }
        }
    }
}

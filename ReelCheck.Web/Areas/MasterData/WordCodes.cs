using VRH.Log4Pro.MultiLanguageManager;

namespace ReelCheck.Web.Areas.MasterData
{
    /// <summary>
    /// MasterData area szókódjai.
    /// </summary>
    public static class WordCodes
    {
        private const string m_huHU = "hu-HU";

        public static class MasterData
        {
            public static class Reel
            {
                [InitializeTranslation("Tekercsellenőrzések", m_huHU)]
                public static class Title { }

                [InitializeTranslation("Az első nézet szerint", m_huHU)]
                public static class InitView0_Title { }

                [InitializeTranslation("A második nézet szerint", m_huHU)]
                public static class InitView1_Title { }

                [InitializeTranslation("A harmadik nézet szerint", m_huHU)]
                public static class InitView2_Title { }
            }
        }
    }
}
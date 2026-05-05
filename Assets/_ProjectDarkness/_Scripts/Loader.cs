using UnityEngine;
using UnityEngine.SceneManagement;


namespace ProjectDarkness
{
    public static class Loader
    {
        public static bool IsHost;

        public enum Scene
        {
            MainMenuScene,
            GameScene,
            LoadingScene,
        }

        private static Scene _targetScene;

        public static void Load(Scene targetScene)
        {
            _targetScene = targetScene;

            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        public static void LoaderCallback()
        {
            SceneManager.LoadScene(_targetScene.ToString());
        }
    }

}
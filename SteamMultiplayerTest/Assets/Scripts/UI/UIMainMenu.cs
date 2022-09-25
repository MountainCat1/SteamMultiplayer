using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private UIMenuScreen mainMenuScreen;

        private readonly List<UIMenuScreen> _menuScreenStack = new();
        private UIMenuScreen _presentMenuScreen;

        #region Unity Methods

        private void Start()
        {
            SetScreen(mainMenuScreen);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                GoBack();
            }
        }

        #endregion
        
        #region Menu Navigation

        /// <summary>
        /// Shows menuScreen and adds previous one to the stack
        /// </summary>
        /// <param name="menuScreen"></param>
        public void ShowScreen(UIMenuScreen menuScreen)
        {
            _menuScreenStack.Add(_presentMenuScreen);
            HideMenuScreen(_presentMenuScreen);

            _presentMenuScreen = menuScreen;
            _presentMenuScreen.Show();
        }

        /// <summary>
        /// Shows menuScreen and clears the stack
        /// </summary>
        /// <param name="menuScreen"></param>
        public void SetScreen(UIMenuScreen menuScreen)
        {
            _menuScreenStack.Clear();
            HideMenuScreen(_presentMenuScreen);

            _presentMenuScreen = menuScreen;
            _presentMenuScreen.Show();
        }

        /// <summary>
        /// Shows the menuScreen at the top of stack and removes it
        /// </summary>
        public void GoBack()
        {
            if (_presentMenuScreen == mainMenuScreen)
                return;

            if (_menuScreenStack.Any())
            {
                SetScreen(mainMenuScreen);
                return;
            }

            var newMenuScreen = _menuScreenStack[^1];

            HideMenuScreen(_presentMenuScreen);
            _presentMenuScreen = newMenuScreen;
            newMenuScreen.Show();

            _menuScreenStack.RemoveAt(_menuScreenStack.Count - 1);
        }

        /// <summary>
        /// Quits game -- calls <see cref="Application"/>.<see cref="Application.Quit(int)"/>
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }
        
        /// <summary>
        /// Hides menu screen, no stack actions performed
        /// </summary>
        /// <param name="menuScreen"></param>
        private void HideMenuScreen(UIMenuScreen menuScreen)
        {
            if (!menuScreen)
                return;

            if (_presentMenuScreen == menuScreen)
                _presentMenuScreen = null;

            menuScreen.Hide();
        }

        #endregion
    }
}
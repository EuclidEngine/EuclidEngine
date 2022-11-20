using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace EuclidEngine
{
    /// @brief Contains behavioral code concerning the main EuclidEngine plugin window.
    public class EuclidEngineWindow : EditorWindow
    {
        /// @brief Callback used to display the main EuclidEngine window
        [MenuItem("Window/Euclid Engine/Euclid Engine")]
        public static void ShowWindow()
        {
            var window = GetWindow<EuclidEngineWindow>("Euclid Engine");

            window.titleContent = new GUIContent("Euclid Engine");
            window.minSize = new Vector2(400, 500);
        }

        /// @brief Called on enablement of the window.
        private void OnEnable()
        {
            // Reference to the root of the window.
            var root = rootVisualElement;

            // Associates a stylesheet to our root. Thanks to inheritance, all root’s
            // children will have access to it.
            root.styleSheets.Add(Resources.Load<StyleSheet>("EuclidEngine_Style"));

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var quickToolVisualTree = Resources.Load<VisualTreeAsset>("EuclidEngine_Main");
            quickToolVisualTree.CloneTree(root);

            // Queries all the buttons (via type) in our root and passes them
            // in the SetupButton method.
            var buttons = root.Query<Button>();
            buttons.ForEach(SetupButton);

            var textfields = root.Query<TextField>();
            textfields.ForEach(SetupTextField);
        }

        /// @brief Contains buttons that apply Euclid Engine features
        List<Button> eeFeaturesButtonsList = new List<Button>();
        private void OnGUI()
        {
            // Checking integrity
            if (eeFeaturesButtonsList == null || searchBar == null) return;

            // Ignoring spaces
            string searchText = searchBar.text.Replace(" ", "");

            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var button in eeFeaturesButtonsList)
                {
                    // We are forced to declare style in a variable because VisualElement.style is a function, so it can not be set
                    var style = button.parent.style;
                    style.display = DisplayStyle.Flex;
                }
                return;
            }

            // Searches for buttons containing 'searchText'
            foreach (var button in eeFeaturesButtonsList)
            {
                // Activates or deactivates them
                bool containsSearchInput = (button.name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);

                // We are forced to declare style in a variable because VisualElement.style is a function, so it can not be set
                var style = button.parent.style;
                style.display = containsSearchInput ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        /// @brief Contains the search bar
        private TextField searchBar = null;
        /// @brief Identifies and stock the search bar in a private attribute
        /// @param textField Contains the potential search bar
        private void SetupTextField(TextField textField)
        {
            if (searchBar == null && textField.parent.tooltip != "Search bar") return;

            GUI.FocusControl(textField.name);
            searchBar = textField;
        }

        /// @brief Sets up a button and its callback mainly
        /// @param button Reference to the button
        private void SetupButton(Button button)
        {
            // Reference to the VisualElement inside the button that serves
            // as the button’s icon.
            //var buttonIcon = button.Q(className: "quicktool-button-icon");

            // Icon’s path in our project.
            //var iconPath = "Icons/" + button.parent.name + "-icon";

            // Loads the actual asset from the above path.
            //var iconAsset = Resources.Load<Texture2D>(iconPath);

            // Applies the above asset as a background image for the icon.
            //buttonIcon.style.backgroundImage = iconAsset;

            // Instantiates our primitive object on a left click.
            button.clickable.clicked += () => AssociateCallback(button.name);
            if (callbacksByNames.ContainsKey(button.name))
                eeFeaturesButtonsList.Add(button);

            // Sets a basic tooltip to the button itself.
            button.tooltip = button.parent.name;
        }

        // Used in EuclidEngineWindow.AssociateCallback
        static Dictionary<string, Action> callbacksByNames = new Dictionary<string, Action>() {
        { "CreateNonEuclideanArea", EuclidEngineWindow.CreateEEArea },
    };

        /// @brief Associates callbacks considering an UI element's name using the callbacksByNames Dictionary.
        /// @param Name of the UI element
        private void AssociateCallback(string uiName)
        {
            Action callback;

            if (callbacksByNames.TryGetValue(uiName, out callback))
            {
                // If callback is contained.
                callback();
            }
            else
            {
                Debug.LogError(uiName + " doesn't have any callbacks declared in `callbacksByNames`");
                return;
            }

        }

        /// @brief Callback that creates an EuclidEngineArea.
        private static void CreateEEArea()
        {
            AreaEditor.CreateMenuCreateArea(Vector3.one, Vector3.one, Vector3.one, Quaternion.identity);
        }
    }
};

#endif
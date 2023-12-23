using System;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace WpfBrowser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var settings = new CefSettings();
            settings.Locale = "de-DE";
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(settings);
            }

            this.Loaded += MainWindow_Loaded;

            browser.Address = textBox.Text;
            textBox.KeyDown += TextBox_KeyDown;
            browser.AddressChanged += Browser_AddressChanged;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            browser.Address = "https://www.google.com";
        }
        private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string currentUrl = browser.Address.ToLower();

            if (currentUrl.Contains("youtube.com") && currentUrl.Contains("/watch?v="))
            {
                ExecuteJavaScriptScript();
            }

            textBox.Text = browser.Address;
        }
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string text = textBox.Text;

                bool isUrl = text.Contains(".");

                if (isUrl)
                {
                    browser.Address = text;
                }
                else
                {
                    browser.Address = "https://www.google.com/search?q=" + Uri.EscapeDataString(text);
                }
            }
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                UpdateTitle(this, EventArgs.Empty);
            }
        }

        private void UpdateTitle(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Title = browser.Title;
            });
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            browser.Address = textBox.Text;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (browser.CanGoBack)
            {
                browser.Back();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (browser.CanGoForward)
            {
                browser.Forward();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            browser.Reload();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            browser.Address = textBox.Text;
        }

        private void DevToolsButton_Click(object sender, RoutedEventArgs e)
        {
            browser.ShowDevTools();
        }

        private void ExecuteJavaScriptScript()
        {

            browser.ExecuteScriptAsync("(function()\n {\n    //\n    //      Config\n    //\n\n    // Enable The Undetected Adblocker\n    const adblocker = true;\n\n    // Enable The Popup remover\n    const removePopup = true;\n\n    // Enable debug messages into the console\n    const debug = true;\n\n    //\n    //      CODE\n    //\n\n    // Specify domains and JSON paths to remove\n    const domainsToRemove = [\n        '*.youtube-nocookie.com/*'\n    ];\n    const jsonPathsToRemove = [\n        'playerResponse.adPlacements',\n        'playerResponse.playerAds',\n        'adPlacements',\n        'playerAds',\n        'playerConfig',\n        'auxiliaryUi.messageRenderers.enforcementMessageViewModel'\n    ];\n\n    // Observe config\n    const observerConfig = {\n        childList: true,\n        subtree: true\n    };\n\n    const keyEvent = new KeyboardEvent(\"keydown\", {\n      key: \"k\",\n      code: \"KeyK\",\n      keyCode: 75,\n      which: 75,\n      bubbles: true,\n      cancelable: true,\n      view: window\n    });\n\n    let mouseEvent = new MouseEvent(\"click\", {\n      bubbles: true,\n      cancelable: true,\n      view: window,\n    });\n\n    //This is used to check if the video has been unpaused already\n    let unpausedAfterSkip = 0;\n\n    if (debug) console.log(\"Remove Adblock Thing: Remove Adblock Thing: Script started\");\n    // Old variable but could work in some cases\n    window.__ytplayer_adblockDetected = false;\n\n    if(adblocker) addblocker();\n    if(removePopup) popupRemover();\n    if(removePopup) observer.observe(document.body, observerConfig);\n\n    // Remove Them pesski popups\n    function popupRemover() {\n        removeJsonPaths(domainsToRemove, jsonPathsToRemove);\n        setInterval(() => {\n\n            const fullScreenButton = document.querySelector(\".ytp-fullscreen-button\");\n            const modalOverlay = document.querySelector(\"tp-yt-iron-overlay-backdrop\");\n            const popup = document.querySelector(\".style-scope ytd-enforcement-message-view-model\");\n            const popupButton = document.getElementById(\"dismiss-button\");\n            // const popupButton2 = document.getElementById(\"ytp-play-button ytp-button\");\n\n            const video1 = document.querySelector(\"#movie_player > video.html5-main-video\");\n            const video2 = document.querySelector(\"#movie_player > .html5-video-container > video\");\n\n            const bodyStyle = document.body.style;\n\n            bodyStyle.setProperty('overflow-y', 'auto', 'important');\n\n            if (modalOverlay) {\n                modalOverlay.removeAttribute(\"opened\");\n                modalOverlay.remove();\n            }\n\n            if (popup) {\n                if (debug) console.log(\"Remove Adblock Thing: Popup detected, removing...\");\n\n                if(popupButton) popupButton.click();\n                // if(popupButton2) popupButton2.click();\n                popup.remove();\n                unpausedAfterSkip = 2;\n\n                fullScreenButton.dispatchEvent(mouseEvent);\n              \n                setTimeout(() => {\n                  fullScreenButton.dispatchEvent(mouseEvent);\n                }, 500);\n\n                if (debug) console.log(\"Remove Adblock Thing: Popup removed\");\n            }\n\n            // Check if the video is paused after removing the popup\n            if (!unpausedAfterSkip > 0) return;\n\n            // UnPause The Video\n            unPauseVideo(video1);\n            unPauseVideo(video2);\n\n        }, 1000);\n    }\n    // undetected adblocker method\n    function addblocker()\n    {\n        setInterval(() =>\n                    {\n            const skipBtn = document.querySelector('.videoAdUiSkipButton,.ytp-ad-skip-button');\n            const ad = [...document.querySelectorAll('.ad-showing')][0];\n            const sidAd = document.querySelector('ytd-action-companion-ad-renderer');\n            const displayAd = document.querySelector('div#root.style-scope.ytd-display-ad-renderer.yt-simple-endpoint');\n            const sparklesContainer = document.querySelector('div#sparkles-container.style-scope.ytd-promoted-sparkles-web-renderer');\n            const mainContainer = document.querySelector('div#main-container.style-scope.ytd-promoted-video-renderer');\n            const feedAd = document.querySelector('ytd-in-feed-ad-layout-renderer');\n            const mastheadAd = document.querySelector('.ytd-video-masthead-ad-v3-renderer');\n            const sponsor = document.querySelectorAll(\"div#player-ads.style-scope.ytd-watch-flexy, div#panels.style-scope.ytd-watch-flexy\");\n            const nonVid = document.querySelector(\".ytp-ad-skip-button-modern\");\n\n            if (ad)\n            {\n                const video = document.querySelector('video');\n                video.playbackRate = 10;\n                video.volume = 0;\n                video.currentTime = video.duration;\n                skipBtn?.click();\n            }\n\n            sidAd?.remove();\n            displayAd?.remove();\n            sparklesContainer?.remove();\n            mainContainer?.remove();\n            feedAd?.remove();\n            mastheadAd?.remove();\n            sponsor?.forEach((element) => {\n                 if (element.getAttribute(\"id\") === \"panels\") {\n                    element.childNodes?.forEach((childElement) => {\n                      if (childElement.data.targetId && childElement.data.targetId !==\"engagement-panel-macro-markers-description-chapters\")\n                          //Skipping the Chapters section\n                            childElement.remove();\n                          });\n                       } else {\n                           element.remove();\n                       }\n             });\n            nonVid?.click();\n        }, 50)\n    }\n    // Unpause the video Works most of the time\n    function unPauseVideo(video)\n    {\n        if (!video) return;\n        if (video.paused) {\n            // Simulate pressing the \"k\" key to unpause the video\n            document.dispatchEvent(keyEvent);\n            unpausedAfterSkip = 0;\n            if (debug) console.log(\"Remove Adblock Thing: Unpaused video using 'k' key\");\n        } else if (unpausedAfterSkip > 0) unpausedAfterSkip--;\n    }\n    function removeJsonPaths(domains, jsonPaths)\n    {\n        const currentDomain = window.location.hostname;\n        if (!domains.includes(currentDomain)) return;\n\n        jsonPaths.forEach(jsonPath => {\n            const pathParts = jsonPath.split('.');\n            let obj = window;\n            let previousObj = null;\n            let partToSetUndefined = null;\n        \n            for (const part of pathParts) {\n                if (obj.hasOwnProperty(part)) {\n                    previousObj = obj; // Keep track of the parent object.\n                    partToSetUndefined = part; // Update the part that we may set to undefined.\n                    obj = obj[part];\n                } else {\n                    break; // Stop when we reach a non-existing part.\n                }\n            }\n        \n            // If we've identified a valid part to set to undefined, do so.\n            if (previousObj && partToSetUndefined !== null) {\n                previousObj[partToSetUndefined] = undefined;\n            }\n        });\n    }\n    // Observe and remove ads when new content is loaded dynamically\n    const observer = new MutationObserver(() =>\n    {\n        removeJsonPaths(domainsToRemove, jsonPathsToRemove);\n    });\n})();");
        }

    }
}

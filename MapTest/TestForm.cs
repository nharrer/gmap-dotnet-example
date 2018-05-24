using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MapTest
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            FixBrowserEmulation();

            InitializeComponent();

            bool designTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            if (!designTime) {
                mapBrowser.ScriptErrorsSuppressed = false;

                string docFile = Path.Combine(Application.StartupPath, "maptest.html");
                string documentText = File.ReadAllText(docFile);
                mapBrowser.DocumentText = documentText;
            }
        }

        // see: 
        // https://stackoverflow.com/questions/17922308/use-latest-version-of-internet-explorer-in-the-webbrowser-control
        // https://blog.malwarebytes.com/101/2016/01/a-brief-guide-to-feature_browser_emulation/
        private static void FixBrowserEmulation()
        {
            var appName = Process.GetCurrentProcess().ProcessName + ".exe";

            // 11000 (0x2AF8) - Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed 
            // in IE11 edge mode. Default value for IE11.
            int? mode = 0x2AF8;

            try {
                const string regpath = @"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
                using (RegistryKey regkey = Registry.CurrentUser.CreateSubKey(regpath)) {
                    if (regkey == null) {
                        Debug.WriteLine("Error: Can not access: " + @"HKEY_CURRENT_USER\\" + regpath);
                        return;
                    }

                    var currentMode = regkey.GetValue(appName) as int?;
                    if (currentMode == mode) {
                        Debug.WriteLine("Info: FEATURE_BROWSER_EMULATION is correct.");
                        return;
                    }

                    regkey.SetValue(appName, mode, RegistryValueKind.DWord);

                    currentMode = regkey.GetValue(appName) as int?;
                    if (currentMode == mode) {
                        Debug.WriteLine("Info: FEATURE_BROWSER_EMULATION set to " + currentMode);
                    } else {
                        Debug.WriteLine("Info: FEATURE_BROWSER_EMULATION modification failed. Current value: " + currentMode);
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
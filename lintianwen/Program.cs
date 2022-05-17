using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace lintianwen
{
    static class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer4 = new lintianwen.LicenseInitializer();
        private static LicenseInitializer m_AOLicenseInitializer3 = new lintianwen.LicenseInitializer();
        private static LicenseInitializer m_AOLicenseInitializer2 = new lintianwen.LicenseInitializer();
        private static LicenseInitializer m_AOLicenseInitializer1 = new lintianwen.LicenseInitializer();
        private static LicenseInitializer m_AOLicenseInitializer = new lintianwen.LicenseInitializer();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer4.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSchematics, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE, esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability, esriLicenseExtensionCode.esriLicenseExtensionCodeTracking });
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer3.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSchematics, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE, esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability, esriLicenseExtensionCode.esriLicenseExtensionCodeTracking });
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer2.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngine, esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB, esriLicenseProductCode.esriLicenseProductCodeArcServer, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer1.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSchematics, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE, esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability, esriLicenseExtensionCode.esriLicenseExtensionCodeTracking });
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSchematics, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE, esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability, esriLicenseExtensionCode.esriLicenseExtensionCodeTracking });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainForm());
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer1.ShutdownApplication();
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer2.ShutdownApplication();
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer3.ShutdownApplication();
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer4.ShutdownApplication();
        }
    }
}

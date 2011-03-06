using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Tftp.Net.UnitTests.Runner
{
    /// <summary>
    /// This program automatically openes NUnit and loads the Tftp.Net Unit tests.
    /// </summary>
    class Program
    {
        //Set this to the folder where you installed NUnit.
        private static readonly string NUNIT_FOLDER = @"C:\Program Files (x86)\NUnit 2.5.9\bin\net-2.0";

        //The executable that will be used to run the tests
        private static readonly string NUNIT_EXE = @"NUnit.exe"; 

        public static void Main()
        {
#if DEBUG
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = NUNIT_FOLDER;
            setup.ConfigurationFile = Path.Combine(NUNIT_FOLDER, "NUnit.exe.config");

            AppDomain nunitDomain = AppDomain.CreateDomain("NUnit", null, setup);
            nunitDomain.ExecuteAssembly(Path.Combine(NUNIT_FOLDER, NUNIT_EXE), null,
                new string[] { Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Tftp.Net.UnitTests.dll") });
#else
#error Unit tests are currently only run for the Debug release.
#endif
        }

    }
}

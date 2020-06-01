using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditProfilesTest
{
    [TestClass]
    public class TestGenerateNewFileName
    {
        [TestMethod]
        public void TestMethod1()
        {
            string fileNameWithPath = $@"C:\Users\msutterfield\Desktop\EditProfiles\NC1\LPS\zM-6280A Neutral Current (LPS) LVL 1 Alarm (Classic) .occ";

            string actual = new EditProfiles.Operations.ProcessFiles().GenerateNewFileName(fileNameWithPath);

            string expected = $@"C:\Users\msutterfield\Desktop\EditProfiles\NC1\LPS\modified files\alarms\regulator 1\profile 1\M-6200B_zM-6280A Neutral Current (LPS) LVL 1 Alarm (Classic)_Reg 1_P1_Fwd Pwr_60Hz.occ";

            Assert.AreEqual(expected, actual);
        }
    }
}

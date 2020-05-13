using System;
using System.IO;
using System.Text.RegularExpressions;
using EditProfiles.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EditProfilesTest
{
    [TestClass]
    public class TestGenerateNewFileNameFunctions
    {
        string FileNameWithPath = $@"\\VOLTA\Eng_Lab\Software Updates\EditProfiles\M-6280A Tests\M-6280A_Profile1_Var Control VT Correction & Phase Compensation(LPS) (SV, Fwd Pwr).occ";
        string ExpectedTestName = $"Runup";

        string FileNameWithoutRevision = $"M-6200B Runup AD 60Hz";
        string ExpectedKeywords = $"M-6200B_AD_60Hz";

        string ExpectedSubFolderName = $"Runup";

        [TestMethod]
        public void CheckTestName()
        {
            // Remove every matching pattern to exposed root test name like "Bandwidth"
            string testName = new AnalyzeValues().Extract(input: Path.GetFileNameWithoutExtension(FileNameWithPath), pattern: new AnalyzeValues().TestFileNamePatterns, keywords: new AnalyzeValues().FileNameKeywords);

           // Assert.AreEqual(ExpectedTestName, testName);

            // temp storage to keep replacement words.
            string keywords = new AnalyzeValues().Replace(input: FileNameWithoutRevision, pattern: new AnalyzeValues().TestFileNamePatterns, keywords: new AnalyzeValues().FileNameKeywords);

            //Assert.AreEqual(ExpectedKeywords, keywords);
            
            // temp storage to keep replacement words.
            string testSubFolderName = new AnalyzeValues().Replace(input: testName, pattern: new AnalyzeValues().TestFolderNamePatterns, keywords: new AnalyzeValues().FolderNameKeywords);

           // Assert.AreEqual(ExpectedSubFolderName, testSubFolderName);

            // index of first '_' is right after product number
            int productNameLength = keywords.IndexOf('_') + 1;

            // split replacement words to insert test file name and new short name for "Regulator #" in the file name, also append file extension.
            for (int CurrentRegulatorValue = 0; CurrentRegulatorValue < 3; CurrentRegulatorValue++)
            {
                string modifiedFolderName = Path.Combine("modified files", testSubFolderName);
                string regulatorFolderName = $"regulator {CurrentRegulatorValue + 1}";
                string profileFolderName = $"profile {keywords.Substring(productNameLength + 1, 1)}";
                string testFolderName = Path.Combine(modifiedFolderName, Path.Combine(regulatorFolderName, new Regex(@"(?<profile>\b[Pp](\d)\b\w*)", RegexOptions.None, TimeSpan.FromMilliseconds(100)).IsMatch(FileNameWithoutRevision) ? profileFolderName : string.Empty)).ToLower();
                string newFileName = Path.Combine(Path.Combine(Path.GetDirectoryName(FileNameWithPath), testFolderName), $"{keywords.Substring(0, productNameLength)}{testName}_Reg {CurrentRegulatorValue + 1}_{keywords.Substring(productNameLength, keywords.Length - productNameLength)}{Path.GetExtension(FileNameWithPath)}");

                string ExpectedNewFileName = $@"\\eng04-win10\c$\Users\tbircek\Desktop\m6200b\drb#186-run-up\ready-for-modbus-mod-test\{testName.ToLower()}\regulator {CurrentRegulatorValue + 1}\M-6200B_Runup_Reg {CurrentRegulatorValue + 1}_";

                if (newFileName.Length > 248)
                {
                    // mode full filename
                    newFileName = Path.Combine(Path.GetDirectoryName(FileNameWithPath), Path.Combine("modified files", Path.GetFileName(FileNameWithPath)));
                }

                Assert.AreEqual(ExpectedNewFileName, newFileName);
            }


        }
    }
}

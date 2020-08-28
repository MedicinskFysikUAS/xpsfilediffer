using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<List<string>> treatmentPlanPageList = new List<List<string>>();
            for (int i = 0; i < 3; i++)
            {
                List<string> stringList = new List<string>();
                for (int j = 0; j < 4; j++)
                {
                    string tmp = "item " + j + "in " + i;
                    stringList.Add(tmp);
                }
                treatmentPlanPageList.Add(stringList);
            }



            // Set a variable to the Documents path.
            string docPath =
              Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
            {
                foreach (var item in treatmentPlanPageList)
                {

                    foreach (string line in item)
                    {
                        outputFile.WriteLine(line);
                    }
                }
            }



            //WpfApp1.TabType tabType = WpfApp1.TabType.PROSTATE;
            //WpfApp1.TreatmentPlan treatmentPlan = new WpfApp1.TreatmentPlan(treatmentPlanPageList, tabType);
            //WpfApp1.Specifications specifications = new WpfApp1.Specifications();
            //WpfApp1.Comparator comparator = new WpfApp1.Comparator(specifications);
            //comparator.treatmentPlan = treatmentPlan;

            //List<WpfApp1.LiveCatheter> tccLiveCatheters = new List<WpfApp1.LiveCatheter>();
            //for (int i = 0; i < 3; i++)
            //{
            //    WpfApp1.LiveCatheter liveCatheter = new WpfApp1.LiveCatheter();
            //    liveCatheter.setCatheterNumber(1);
            //    List<Tuple<string, string>> positonTimePairs = new List<Tuple<string, string>>();
            //    for (int j = 0; j < 2; j++)
            //    {
            //        Tuple<string, string> tuple = new Tuple<string, string>(i.ToString(), j.ToString());
            //        positonTimePairs.Add(tuple);
            //    }

            //}

            //WpfApp1.TccPlan tccPlan = new WpfApp1.TccPlan(treatmentPlanPageList, tccLiveCatheters);
            //comparator.tccPlan = tccPlan;
            //bool skipApprovalTest = true;
            //bool useRelativeEpsilon = true;
            //List<List<string>> resultRows = new List<List<string>>();
            //resultRows.AddRange(comparator.resultRows(skipApprovalTest, useRelativeEpsilon));
            //foreach (var item in resultRows)
            //{
            //    foreach (var subItem in item)
            //    {
            //        var tmp = subItem;
            //    }
            //}
            Assert.AreEqual("Erik", "Erik");

        }
    }
}

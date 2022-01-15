﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace DSProject
{
    interface ILogger
    { 
        void Error(string message);
        void Warning(string message);
        void Info(string message);
    }

    // Required functions that should be implemented
    interface IConsole
    {
        void ReadFiles();

        void ReadFiles(string drugsPath,
            string diseasesPath, string drugsEffectsPath,
            string diseasesDrugsPath);

        void IncreaseDrugsCost(int inflationRate);

        int CalcPrescription(string[] drugs);

        String FindDrug(string drugName);

        String FindDisease(string diseaseName);

        void AddDrug(string drugName, int drugPrice);

        void AddDisease(string diseaseName);

        void DeleteDrug(string drugName);

        void DeleteDisease(string diseaseName);

        String DrugMalfunction(string[] drugs);

        String DiseaseMalfunction(string[] drugs);
    }

    class MyConsole: IConsole
    {

        private DiseaseDrugDb Db;
        private MyOperator Operator;

        public MyConsole(DiseaseDrugDb db, MyOperator op)
        {
            this.Db = db;
            this.Operator = op;
        }

        /// <inheritdoc />
        public void ReadFiles()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void ReadFiles(string drugsPath, 
            string diseasesPath, string drugsEffectsPath, 
            string diseasesDrugsPath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void IncreaseDrugsCost(int inflationRate) // Proxy is not tested
        {
           this.Operator.ApplyInflationRate(inflationRate);
        }

        /// <inheritdoc />
        public int CalcPrescription(string[] drugs) // Proxy is not tested
        {
            return this.Operator.CalcPrescription(drugs);
        }

        /// <inheritdoc />
        public string FindDrug(string drugName) // Proxy is not tested
        {
            return this.Operator.FindDrugAssociated(drugName);
        }

        /// <inheritdoc />
        public string FindDisease(string diseaseName) // Proxy is not tested
        {
            return this.Operator.FindDiseaseDrugs(diseaseName);
        }

        /// <inheritdoc />
        public void AddDrug(string drugName, int drugPrice)  // Proxy is not tested
        {
            String input1 = drugName + " : " + drugPrice;
            
        }

        /// <inheritdoc />
        public void AddDisease(string diseaseName)  // Proxy is not tested
        {
            // Choose several drugs to associate with the disease
            Random rand = new Random();
            int[] idx = new int[rand.Next(1, 5)];

            String output = diseaseName + " :";
            for (int i = 0; i < idx.Length; i++)
            {
                idx[i] = rand.Next(0, this.Db.DrugsNames.Count);
                output += " (" + this.Db.DrugsNames[idx[i]] + "," + (rand.Next(0, 2) == 1 ? "+" : "-") + ") ;";
            }
            //

            bool outcome = 
                this.Operator.CreateDisease(output.TrimEnd(this.Operator.TrimParams));
            if (outcome)
            {
                Console.WriteLine("The disease was created !");
            }
            else
            {
                Console.WriteLine("The disease was NOT created !");
            }
        }

        /// <inheritdoc />
        public void DeleteDrug(string drugName)  // Proxy is not tested
        {
            bool result = this.Operator.DeleteDrug(drugName);

            if (result)
            {
                Console.WriteLine("Drug was founded & deleted !");
                return;
            }
            else
            {
                Console.WriteLine("Drug was not founded !");
                return;
            }
        }

        /// Used for Deleting Disease
        public void DeleteDisease(string diseaseName)  // Proxy is not tested
        {
            bool result = this.Operator.DeleteDisease(diseaseName);

            if (result)
            {
                Console.WriteLine("Disease was found & deleted");
                return;
            }
            else
            {
                Console.WriteLine("Disease was NOT found !");
                return;
            }
        }

        /// <inheritdoc />
        public string DrugMalfunction(string[] drugs)  // Proxy is not tested
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string DiseaseMalfunction(string[] drugs)  // Proxy is not tested
        {
            throw new NotImplementedException();
        }
    }

    class MyLogger: ILogger
    {
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR : " + message);
            Console.ResetColor();
        }

        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning : " + message);
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("INFO : " + message);
            Console.ResetColor();
        }
    }

    // include two functions for checking existence of diseases & drugs
    interface IContains
    {
        bool ContainsDisease(String name);
        bool ContainsDrug(String name);
    }

    // Find the drugs which are associated with a specific disease
    // Find the drugs & diseases which are associated with a another drug
    interface IFinder
    {
        String FindDiseaseDrugs(String name);
        String FindDrugAssociated(String name);
    }

    // Using for persisting our data about drugs & diseases
    interface IPersistence
    {
        bool PersistDiseases(string path);
        bool PersistDrugs(string path);
        bool PersistDiseasesDrugs(string path);
        bool PersistDrugsEffects(string path);
    }

    // Apply CRUD operations to MyOperator class
    interface ICrd
    {
        bool CreateDisease(string name);
        void CreateDrug(string name,
            string drugEffects, 
            string diseasesDrugs,
            string selfDrugEffects);
        bool DeleteDrug(string name);
        bool DeleteDisease(string name);
    }

    // Application BD class in sync mode
    class DiseaseDrugDb
    {
        public List<String> DrugsNames;
        public List<String> DiseaseNames;
        public List<String> DiseaseDrugsNames;
        public List<String> DrugsEffectsNames;

        public DiseaseDrugDb() // 170 Milliseconds for first time 
        {
            var watch10 = System.Diagnostics.Stopwatch.StartNew();

            this.DiseaseNames =
                new List<string>(
                    File.ReadAllLines(
                        @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\diseases.txt"));

            this.DrugsNames =
                new List<string>(
                    File.ReadAllLines(
                        @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\drugs.txt"));

            this.DiseaseDrugsNames =
                new List<string>(
                    File.ReadAllLines(
                        @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\alergies.txt"));

            this.DrugsEffectsNames =
                new List<string>(
                    File.ReadAllLines(
                        @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\effects.txt"));

            watch10.Stop();
            Console.WriteLine("The fucking time of Reading all files : " + watch10.ElapsedMilliseconds);
        }
    }

    // Application BD class in Async mode
    class DiseaseDrugDbAsync
    {
        public String[] DrugsNames;
        public String[] DiseaseNames;
        public String[] DiseaseDrugsNames;
        public String[] DrugsEffectsNames;

        public DiseaseDrugDbAsync()
        {
            var watch10 = System.Diagnostics.Stopwatch.StartNew();
            this.DoItAsync();
            watch10.Stop();
            Console.WriteLine("The fucking time of Reading all files async : " + watch10.ElapsedMilliseconds);
        }

        private async void DoItAsync()
        { 
            
            this.DiseaseNames = 
                await File.ReadAllLinesAsync(@"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\diseases.txt");
            
            this.DrugsNames = 
                await File.ReadAllLinesAsync(@"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\drugs.txt");

            this.DiseaseDrugsNames = 
                await File.ReadAllLinesAsync(@"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\alergies.txt");

            this.DrugsEffectsNames =
                await File.ReadAllLinesAsync(@"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\effects.txt");
            
        }
    }

    // Do all of fundamental operations for us
    class MyOperator: IFinder, IContains, IPersistence, ICrd
    {
        private DiseaseDrugDb DB;

        private String Temp;
        private int TempResult; // Used in CalcPrescription Computing
        public Char[] TrimParams;

        private String DiseasePath;
        private String DiseaseDrugsPath;
        private String DrugsPath;
        private String DrugsEffectsPath;

        private String SDiseasePath;
        private String SDiseaseDrugsPath;
        private String SDrugsPath;
        private String SDrugsEffectsPath;

        private ReaderWriterLockSlim RWL;

        public MyOperator(DiseaseDrugDb db)
        {
            this.DB = db;

            this.DiseasePath = 
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\diseases.txt";

            this.DiseaseDrugsPath = 
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\alergies.txt";

            this.DrugsEffectsPath =
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\effects.txt";

            this.DrugsPath =
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\drugs.txt";

            this.SDiseasePath =
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\diseases_2.txt";

            this.SDiseaseDrugsPath =
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\alergies_2.txt";

            this.SDrugsEffectsPath =
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\effects_2.txt";

            this.SDrugsPath =
                @"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\drugs_2.txt";


            this.TrimParams = new[] {';', ' '};
        }

        /// It will return the actual line which contains the disease
        public string FindDiseaseDrugs(string name)
        {
            var watch10 = System.Diagnostics.Stopwatch.StartNew();

            String[] dual;
            String[] after;
            String result = name + " : ";
            foreach (var line in this.DB.DiseaseDrugsNames)
            {
                dual = line.Split(":");
                dual[0] = dual[0].Trim();
                dual[1] = dual[1].Trim();
                if (dual[0] == name)
                {
                    after = dual[1].Split(";");
                    for (int i = 0; i < after.Length; i++)
                    {
                        after[i] = after[i].Trim();
                        if (after[i].Contains('+'))
                        {
                            result += after[i] + " ; ";
                        }
                    }
                    break;
                }
            }

            watch10.Stop();
            Console.WriteLine("The Time for DiseaseDrugs function : " + watch10.ElapsedMilliseconds);
            return result.TrimEnd(this.TrimParams);
        }

        // It will print three lines first for diseases, third for drugs have effects on target,
        // second for target has effects on other drugs
        public string FindDrugAssociated(string name)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            String[] dual;
            String[] after;
            String result1 = name + " : ";
            this.Temp = name + " : ";

            Thread th = new Thread(this.FindDrugAssociatedHelper);
            th.Start(name);

            foreach (var line in this.DB.DiseaseDrugsNames)
            {
                if (line.Contains(name))
                {
                    dual = line.Split(":");
                    dual[0] = dual[0].Trim();
                    dual[1] = dual[1].Trim();

                    //result1 += dual[0];

                    after = dual[1].Split(";");
                    for (int i = 0; i < after.Length; i++)
                    {
                        if (after[i].Contains(name))
                        {
                            result1 += "(" + dual[0] + "," + after[i].Split(",")[1] + "; ";
                            break;
                        }
                    }
                }
            }

            String result2 = "";
            foreach (var drugEffects in this.DB.DrugsEffectsNames)
            {
                dual = drugEffects.Split(":");
                if (dual[0].Contains(name))
                {
                    result2 = drugEffects;
                    break;
                }
            }

            th.Join();

            watch.Stop();
            Console.WriteLine("The Time for FindDrugsAssociation function : " + watch.ElapsedMilliseconds);
            return result1.TrimEnd(this.TrimParams) + "\n" + this.Temp.TrimEnd(this.TrimParams) + 
                   "\n" + result2;
        }

        /// <inheritdoc />
        private void FindDrugAssociatedHelper(object name)
        {
            String name_ = name as string;
            String[] dual;
            String[] after;

            foreach (var line in this.DB.DrugsEffectsNames)
            {
                dual = line.Split(":");
                dual[0] = dual[0].Trim();
                dual[1] = dual[1].Trim();

                if (dual[1].Contains(name_))
                {
                    after = dual[1].Split(";");

                    //this.Temp += dual[0];
                    for (int i = 0; i < after.Length; i++)
                    {
                        if (after[i].Contains(name_))
                        {
                            this.Temp += "(" + dual[0] + "," + after[i].Split(",")[1] + "; ";
                            break;
                        }
                    }
                }
            }

            return;
        }

        /// <inheritdoc />
        public bool ContainsDisease(string name)
        {
            var watch10 = System.Diagnostics.Stopwatch.StartNew();

            var result = this.DB.DiseaseNames.Contains(name);

            watch10.Stop();
            Console.WriteLine("The Time for ContainsDrug function : " + watch10.ElapsedMilliseconds);
            return result;
        }

        /// <inheritdoc />
        public bool ContainsDrug(string name)
        {
            var watch10 = System.Diagnostics.Stopwatch.StartNew();

            bool result = false;
            foreach (var drug in this.DB.DrugsNames)
            {
                result = drug.Contains(name);
                if (result == true)
                {
                    break;
                }
            }

            watch10.Stop();
            Console.WriteLine("The Time for ContainsDrug function : " + watch10.ElapsedMilliseconds);
            return result;
        }

        /// <inheritdoc />
        public bool PersistDiseases(string path)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            File.WriteAllLines(@path, this.DB.DiseaseNames);

            watch.Stop();
            Console.WriteLine("The time for persisting Diseases : " + watch.ElapsedMilliseconds);
            return true;
        }

        /// <inheritdoc />
        public bool PersistDrugs(string path)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            File.WriteAllLines(@path, this.DB.DrugsNames);

            watch.Stop();
            Console.WriteLine("The time for persisting Drugs : " + watch.ElapsedMilliseconds);
            return true;
        }

        /// <inheritdoc />
        public bool PersistDiseasesDrugs(string path)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            File.WriteAllLines(@path, this.DB.DiseaseDrugsNames);
            
            watch.Stop();
            Console.WriteLine("The time for persisting DiseaseDrugs : " + watch.ElapsedMilliseconds);
            return true;
        }

        /// <inheritdoc />
        public bool PersistDrugsEffects(string path)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            File.WriteAllLines(@path, this.DB.DrugsEffectsNames);

            watch.Stop();
            Console.WriteLine("The time for persisting DrugsEffects : " + watch.ElapsedMilliseconds);
            return true;
        }

        /// <inheritdoc />
        public bool CreateDisease(string input) // The input should be in document format // Test is required Need handle duplicated diseases
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            String[] dual = input.Split(":");
            bool modifiedFlag = false;

            if (!this.DB.DiseaseNames.Contains(dual[0]))
            {
                this.DB.DiseaseNames.Add(dual[0]);
                this.DB.DiseaseDrugsNames.Add(input);
                modifiedFlag = true;
            }

            if (modifiedFlag)
            {
                // Theses addresses must get from input
                this.PersistDiseasesDrugs(
                    this.DiseaseDrugsPath);
                this.PersistDiseases(
                    this.DiseasePath);
            } 

            watch.Stop();
            Console.WriteLine("The time for creating a disease" +
                              " with associated drugs : " + watch.ElapsedMilliseconds);

            return modifiedFlag;
        }

        /// <inheritdoc />
        public void CreateDrug( // test is required
            string drugName, // drug_aa : price
            string drugsEffects, //  must be in this format   drug_aa:drug_name,drug_effect;drug_name,drug_effect
            string diseasesDrugs, // must be in this format   drug_aa:disease_name,+;disease_name,-
            string selfDrugEffects // must be in document format
            ) // Test is required
        {

            if (this.DB.DrugsNames.Contains(drugName))
            {
                Console.WriteLine("The drug is repetitious");
                return;
            }

            Thread th1 = new Thread(this.CreateDrugHelper);
            Thread th2 = new Thread(this.CreateDrugHelper2);

            th1.Start(drugsEffects);
            th2.Start(diseasesDrugs);

            this.DB.DrugsNames.Add(drugName);
            this.PersistDrugs(this.DrugsPath);

            th1.Join();
            th2.Join();

            this.DB.DrugsEffectsNames.Add(selfDrugEffects);
            this.PersistDrugsEffects(this.DrugsEffectsPath);
        }

        private void CreateDrugHelper(object input)
        {
            string drugsEffects = input as string;
            String[] dual = drugsEffects.Split(":");
            String[] after = dual[1].Split(";");
            Dictionary<String, String> drugsEffectsDict = new Dictionary<string, string>();

            String[] tmp;
            foreach (var str in after)
            {
                tmp = str.Split(",");
                drugsEffectsDict.Add(tmp[0], tmp[1]);
            }

            foreach (var kv in drugsEffectsDict)
            {
                for (int i = 0; i < this.DB.DrugsEffectsNames.Count; i++)
                {
                    if (this.DB.DrugsEffectsNames[i].Split(":")[0].Contains(kv.Key))
                    {
                        this.DB.DrugsEffectsNames[i] += " ; (" + dual[0] + "," + kv.Value + ")";
                        drugsEffectsDict.Remove(kv.Key);
                        break;
                    }
                }
            }

            this.PersistDrugsEffects(this.DrugsEffectsPath);
        }

        private void CreateDrugHelper2(object input)
        {
            string diseasesDrugs = input as string;
            String[] dual = diseasesDrugs.Split(":"); // dual[0] is the drug_name like drug_aa
            String[] after = dual[1].Split(";");
            Dictionary<String, String> diseaseEffects = new Dictionary<string, string>();

            String[] tmp;
            foreach (var str in after)
            {
                tmp = str.Split(",");
                diseaseEffects.Add(tmp[0], tmp[1]);
            }

            foreach (var kv in diseaseEffects)
            {
                for (int i = 0; i < this.DB.DiseaseDrugsNames.Count; i++)
                {
                    if (this.DB.DiseaseDrugsNames[i].Contains(kv.Key))
                    {
                        this.DB.DiseaseDrugsNames[i] += " ; (" + dual[0] + "," + kv.Value + ")";
                        diseaseEffects.Remove(kv.Key);
                        break;
                    }
                }
            }

            this.PersistDiseasesDrugs(this.DiseaseDrugsPath);
        }

        /// Deletes specific drug from drugs.txt
        public bool DeleteDrug(string name)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            bool modifiedFlag = false;
            Thread th = new Thread(this.DeleteDrugHelper);
            Thread th2 = new Thread(this.YetAnotherDeleteDrugHelper);

            th.Start(name);
            th2.Start(name);

            for (int i = 0; i < this.DB.DrugsNames.Count; i++)
            {
                if (this.DB.DrugsNames[i].Contains(name))
                {
                    this.DB.DrugsNames.RemoveAt(i);
                    modifiedFlag = true;
                    break;
                }
            }

            if (modifiedFlag) // Theses addresses must get from input
            {
                this.PersistDrugs(
                    this.SDrugsPath);
            }

            th.Join();
            th2.Join();

            watch.Stop();
            Console.WriteLine("The time for deleting Drug : " + watch.ElapsedMilliseconds);

            return modifiedFlag;
        }

        /// Deletes specific drug from alergies.txt 
        private void DeleteDrugHelper(object name)
        {
            String name_ = name as string;
            String[] dual;
            String[] after;
            String tmp = "";
            bool modifiedFlag = false;

            for (int i = 0; i < this.DB.DiseaseDrugsNames.Count; i++)
            {
                if (this.DB.DiseaseDrugsNames[i].Contains(name_))
                {
                    modifiedFlag = true;
                    dual = this.DB.DiseaseDrugsNames[i].Split(":");
                    after = dual[1].Split(";");

                    tmp += dual[0] + ":";

                    for (int j = 0; j < after.Length; j++)
                    {
                        if (!after[j].Contains(name_))
                        {
                            tmp += after[j] + ";";
                        }
                    }

                    if (tmp.Contains('('))
                    {
                        this.DB.DiseaseDrugsNames[i] = tmp.TrimEnd(this.TrimParams);
                    }
                    else
                    {
                        this.DB.DiseaseDrugsNames.RemoveAt(i);
                        i -= 1;
                    }
                }

                tmp = "";
            }

            if (modifiedFlag) // Theses addresses must get from input
            {
                this.PersistDiseasesDrugs(
                    this.SDiseaseDrugsPath);
            }
        }

        /// Delete specific drug from effects.txt
        private void YetAnotherDeleteDrugHelper(object name)
        {
            string name_ = name as string;
            String[] dual;
            String[] after;
            String tmp = "";
            bool modifiedFlag = false;

            for (int j = 0; j < this.DB.DrugsEffectsNames.Count; j++)
            {
                dual = this.DB.DrugsEffectsNames[j].Split(":");
                if (dual[0].Contains(name_))
                {
                    this.DB.DrugsEffectsNames.RemoveAt(j);
                    j -= 1;
                    modifiedFlag = true;
                }
                else if (dual[1].Contains(name_))
                {
                    modifiedFlag = true;
                    after = dual[1].Split(";");

                    tmp += dual[0] + ":";
                    for (int i = 0; i < after.Length; i++)
                    {
                        if (!after[i].Contains(name_))
                        {
                            tmp += after[i];
                        }
                    }

                    if (tmp.Contains('('))
                    {
                        this.DB.DrugsEffectsNames[j] = tmp.TrimEnd(this.TrimParams);
                    }
                    else
                    {
                        this.DB.DrugsEffectsNames.RemoveAt(j);
                        j -= 1;
                    }
                }

                tmp = "";
            }

            if (modifiedFlag) // Theses addresses must get from input
            {
                this.PersistDrugsEffects(
                    this.SDrugsEffectsPath);
            }
        }

        /// Deletes specific disease from disease.txt
        public bool DeleteDisease(string name)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            bool modifiedFlag = false;
            Thread th = new Thread(this.DeleteDiseaseHelper);

            th.Start(name);

            if (this.DB.DiseaseNames.Contains(name))
            {
                modifiedFlag = true;
                this.DB.DiseaseNames.Remove(name);
            }

            if (modifiedFlag) // These addresses should take from input
            {
                this.PersistDiseases(
                    this.SDiseasePath);
            }

            th.Join();

            watch.Stop();
            Console.WriteLine("The time for deleting Disease : " + watch.ElapsedMilliseconds);

            return modifiedFlag;
        }

        /// Deletes specific disease from alergies.txt 
        private void DeleteDiseaseHelper(object name)
        {
            String name_ = name as string;
            bool modifiedFlag = false;

            for (int i = 0; i < this.DB.DiseaseDrugsNames.Count; i++)
            {
                if (this.DB.DiseaseDrugsNames[i].Contains(name_))
                {
                    modifiedFlag = true;
                    this.DB.DiseaseDrugsNames.RemoveAt(i);
                    break;
                }
            }

            if (modifiedFlag) // Theses addresses must get from input
            {
                this.PersistDiseasesDrugs(
                    this.SDiseaseDrugsPath);
            }
        }

        public void ApplyInflationRate(int infRate)
        {
            Thread th1 = new Thread(this.ApplyInflationRateHelper);
            th1.Start(infRate);

            String[] dual;
            for (int i = 0; i < this.DB.DrugsNames.Count / 2; i++)
            {
                dual = this.DB.DrugsNames[i].Split(":");
                int price = int.Parse(dual[1].Trim());
                price *= infRate;

                this.DB.DrugsNames[i] = dual[0] + ": " + price;
            }

            th1.Join();
            this.PersistDrugs(this.DrugsPath);
        }

        private void ApplyInflationRateHelper(object input)
        {
            int infRate = (int) input;

            String[] dual;
            for (int i = (this.DB.DrugsNames.Count / 2); i < this.DB.DrugsNames.Count; i++)
            {
                dual = this.DB.DrugsNames[i].Split(":");

                int price = int.Parse(dual[1].Trim());
                price *= infRate;

                this.DB.DrugsNames[i] = dual[0] + ": " + price;
            }
        }

        public int CalcPrescription(string[] inputs)
        {
            this.RWL = new ReaderWriterLockSlim();
            List<String> inputs1 = new List<string>(inputs[0..(inputs.Length/2)]);
            List<String> inputs2 = new List<string>(inputs[(inputs.Length/2)..(inputs.Length + 1)]);
            Thread th1 = new Thread(CalcPrescriptionHelper);
            
            th1.Start(inputs1);
            int result = 0;

            try
            {
                this.RWL.EnterReadLock();

                foreach (var drug in this.DB.DrugsNames)
                {
                    string[] dual = drug.Split(":");
                    dual[0] = dual[0].Trim();
                    dual[1] = dual[1].Trim();
                    for (int i = 0; i < inputs2.Count; i++)
                    {
                        if (dual[0] == inputs2[i])
                        {
                            result += int.Parse(dual[1]);
                            inputs2.RemoveAt(i);
                            break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                this.RWL.ExitReadLock();
            }

            th1.Join();

            result += this.TempResult;
            this.TempResult = 0;

            return result;
        }

        private void CalcPrescriptionHelper(object inputs)
        {
            this.TempResult = 0;
            List<String> inputs2 = inputs as List<String>;

            try
            {
                this.RWL.EnterReadLock();
                foreach (var drug in this.DB.DrugsNames)
                {
                    string[] dual = drug.Split(":");
                    dual[0] = dual[0].Trim();
                    dual[1] = dual[1].Trim();
                    for (int i = 0; i < inputs2.Count; i++)
                    {
                        if (dual[0] == inputs2[i])
                        {
                            this.TempResult += int.Parse(dual[1]);
                            inputs2.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);

            }
            finally
            {
                this.RWL.ExitReadLock();
            }
        }
    }

    class Program
    {
        private static DiseaseDrugDb DB;

        private static MyConsole Cons;

        // Initialize the Console
        private static void InitConsole(DiseaseDrugDb db, MyOperator op)
        {
            if (Program.Cons == null)
            {
                Cons = new MyConsole(db, op);
            }
        }

        // Initialized the DB
        private static void InitDb()
        {
            if (Program.DB == null)
            {
                DB = new DiseaseDrugDb();
            }
        }

        static void Main(string[] args)
        {
            InitDb();
            MyOperator op = new MyOperator(DB);
            InitConsole(DB, op);

            //Console
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("hello enter something :");
            Console.ResetColor();
            Console.WriteLine("Just for manipulation");
            Console.ReadLine();
            //Console

            // Console.WriteLine(op.ContainsDisease("Dis_xmbjdyijco"));

            // Console.WriteLine(op.ContainsDrug("Drug_hvtiayzegc"));

            // Console.WriteLine(op.FindDiseaseDrugs("Dis_lbqblqdzoo"));

            // Console.WriteLine(op.FindDrugAssociated("Drug_vfsskclbhk"));
            // Console.WriteLine(op.FindDrugAssociated("Drug_vobddjeuyu"));
            // Console.WriteLine(op.FindDrugAssociated("Drug_mlsvozghuj"));

            // op.PersistDiseases(@"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\diseases_2.txt");

            // op.DeleteDrug("Drug_ugqzkbyrrr");
            // op.PersistDrugs(@"C:\Users\Asus\Desktop\DS-Final-Project\DS-Final-Project\datasets\drugs_2.txt");

            // op.DeleteDisease("Dis_xmbjdyijco"); // Test the diseases.txt look at end of file
            // op.DeleteDisease("Dis_lbqblqdzoo"); // Test the alergies.txt look at end of file


            // op.DeleteDrug("Drug_ucxnqwcpsf"); // Test the drugs.txt, look at end of file
            // op.DeleteDrug("Drug_vobddjeuyu"); // Test the alergies.txt look at end of file
            // op.DeleteDrug("Drug_uecqvzgzwq"); // Test the alergies.txt look at end of file
            // op.DeleteDrug("Drug_wdqjyjytrl"); // Test the alergies.txt look at end of file
            // op.DeleteDrug("Drug_xkmxoweplh"); // Test the alergies.txt look at end of file
            // op.DeleteDrug("Drug_mlsvozghuj"); // Test the effects.txt look at end of file

            // op.CreateDisease("Dis_aaaaaaaaaa : (Drug_ddddddd,+) ; (Drug_eeeeeee,-) ; (Drug_dfwdfwdfw,+)"); // Test for creating disease

            // op.CreateDrug( // Test for create drug
            //     "Drug_aaaaaaaaaa : 9999",
            //     "Drug_aaaaaaaaaa:Drug_ugqzkbyrrr,Eff_tvhidekyud;Drug_rxqdjdgkva,Eff_bsmcbsnxps",
            //     "Drug_aaaaaaaaaa:Dis_qfwtffeczg,+;Dis_xikkgsfmlz,-",
            //     "Drug_aaaaaaaaaa : (Drug_ugqzkbyryr,Eff_kbbhexfirm) ; (Drug_qlihgxyjok,Eff_fsmsfgmihc)");


            // op.ApplyInflationRate(2);

            String[] inputs = new[] {"kfroiaefi", "safdhaisfiu", "hdfasbi", "aushdfyadf"};
            var newin = new List<String>(inputs[3..4]);
            Console.WriteLine(newin.Count);

            return;
        }
    }
}

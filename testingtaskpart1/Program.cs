using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;


namespace testingtaskpart1
{
    class Program
    {
        static void Main(string[] args)
        {
            Process Process = new Process();
           Process = Process.Deserrialize(Process);
            for ( ; ; ) {
                Console.WriteLine(@"Чего изволите? 
1. Указать новую путь исходной папки
2. Указать путь конечной папки
3. Назначить день и время резервного копирования
4. Очистить буфер исходных папок
5. Выход");
               if (Convert.ToInt32(Console.ReadLine()) == 1)
                {
                    Process.SetSourceFolder();
                }
               else if (Convert.ToInt32(Console.ReadLine()) == 2)
                {
                    Process.SetDestinationFolder();
                }
               else if (Convert.ToInt32(Console.ReadLine()) == 3)
                {
                    Process.StartCopy();
                }
                else if (Convert.ToInt32(Console.ReadLine()) == 4)
                {
                    Process.ClearSourceBuffer();
                }
                else if (Convert.ToInt32(Console.ReadLine()) == 5)
                {
                    Process.SerrializeAndExit(Process);
                }
            }

            

        }
    }



    [DataContract]
    class Process
    {
        [DataMember]
        public List<string> Source = new List<string>();
        [DataMember]
        public string Destination;

        public void SetSourceFolder() //Добавление папок-источников
        {
            Console.Clear();
            Console.WriteLine("Введите путь в формате \"C:\\\\user\\\" без кавычек");
            string temp = Console.ReadLine();
            if ((temp != "") || (Directory.Exists(temp) == true))
            {
                Source.Add(temp);
            }
        }
        public void SetDestinationFolder() //Выставление папки назначения
        {
            Console.Clear();
            Console.WriteLine("Введите путь в формате \"C:\\\\user\\\" без кавычек");
            string temp = Console.ReadLine();
            if (temp != "")
            {
                Destination = temp;
            }
        }

        public void StartCopy() // Процесс копирования 
        {
            string time = " " + DateTime.Now.ToString();
            for (int i = 0; i < Source.Count; i++)
            {
                bool new_directory = true;
                var files = Directory.GetFileSystemEntries(Source[i]);
                Directory.CreateDirectory(Path.Combine(Destination, time));
                Destination = Destination + time;
                foreach (string file in files)
                {
                    string fileName = file.Substring(Source[i].Length + 1);
                    Directory.Move(file, Path.Combine(Destination, fileName));
                    using (FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        
                            if (new_directory == true)
                        {
                            sw.WriteLine("Началось копирование из {0} в {1}", Source[i], Destination);
                            new_directory = false;
                        }
                        sw.WriteLine("Скопировано ", fileName);
                    
                    }
                }
            }

        }

        public void ClearSourceBuffer() //Очищает список папок источников
        {
            Source.Clear();
        }

        public void SerrializeAndExit(Process process) //Сохраняет список папок до следующего запуска
        {
            var jsonformatter = new DataContractSerializer(typeof(Process));
            using (FileStream fs = new FileStream("settings.json", FileMode.OpenOrCreate))
            {
                jsonformatter.WriteObject(fs, process);
            }
            Environment.Exit(0);
        }

        public Process Deserrialize(Process process) //Выгружает папки из JSON файла в память программы
        {
            Process temp = new Process();
            using (FileStream fs = new FileStream("settings.json", FileMode.OpenOrCreate))
            {
                var sr = new StreamReader(fs);
                string text = sr.ReadToEnd();
                if (text != "")
                {
                    var jsonformatter = new DataContractSerializer(typeof(Process));
                    temp = (Process)jsonformatter.ReadObject(fs);
                }
            }
            return temp;
        }
    }
}

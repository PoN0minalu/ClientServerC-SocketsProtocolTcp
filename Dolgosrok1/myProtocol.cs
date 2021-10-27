using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace myFirstProtocol
{
    public class TMPD1Packet
    {
        private byte __type; //тип пакета 
        public const byte __type0 = 0xFF; //пакет ответа от сервера
        public const byte __type1 = 0xD; // пакет с запуском программы в консоли
        public const byte __type2 = 0x1E; // пакет с удалением файла через консоль
        public const byte __type3 = 0x45; // пакет с загрузкой указанного файла
        public const byte __type4 = 0x4A; // пакет с запросом на получение файла
        public const byte __type5 = 0xBF; // пакет с файлом для отправки с сервера на клиент
        public const byte __unknownType = 0x13; //неопознанный пакет
        private byte[] __pathToFile; // путь к файлуS
        private byte[] __paramsOfExe; // параметры запуска программы
        private byte[] __fileBytes; // данные передаваемого файла 
        private byte[] __pathToGetFile; //путь куда установить скачанный с сервера файл
        private byte[] __reply; // ответ от сервера
        private string __fileName; 

        public TMPD1Packet(int type = 0) // создаем тип пакета в зависимости от желания пользователя
        {
            switch(type)
            {
                case 1:
                    __type = __type1;
                    break;
                case 2:
                    __type = __type2;
                    break;
                case 3:
                    __type = __type3;
                    break;
                case 4:
                    __type = __type4;
                    break;
                case 5:
                    __type = __type5;
                    break;
                case 6:
                    __type = __unknownType;
                    break;
                default:
                    __type = __type0;
                    break;
            }
        }
        public void SetPathToFile(string pathToFile) //записываем путь к файлу пакет
        {
            if(__type == __type3)
            {
                string aim = pathToFile + "/" + __fileName;
                __pathToFile = Encoding.UTF8.GetBytes(aim);
                return;
            }
            __pathToFile = Encoding.UTF8.GetBytes(pathToFile);
        }
        public void SetParamsOfExe(string parametrs)
        {
            __paramsOfExe = Encoding.UTF8.GetBytes(parametrs); //записываем параметры
        }
        public void SetFileBytes(string pathToFileToDownload)
        {
            __fileBytes = File.ReadAllBytes(pathToFileToDownload);
            __fileName = Path.GetFileName(pathToFileToDownload);
        }
        public void SetPathToGetFile(string pathtogetfile)
        {
            string aim = pathtogetfile;
            if(__type == __type4)
            {
                aim = pathtogetfile + "/" + Path.GetFileName(GetPath());
            }           
            __pathToGetFile = Encoding.UTF8.GetBytes(aim);
        }
        public void SetReply(string answer)
        {
            __reply = Encoding.UTF8.GetBytes(answer); //записываем ответ сервера
        }
        public byte[] ToPack() //упаковываем все в массив байтов
        {
            var packer = new MemoryStream();
            if(__type == __type0)
            {
                packer.WriteByte(__type);
                packer.WriteByte(Convert.ToByte(__reply.Length));
                packer.Write(__reply, 0, __reply.Length);
                return packer.ToArray();
            }
            else if(__type == __type1)
            {
                if(__paramsOfExe == null)
                {
                    __paramsOfExe = Encoding.UTF8.GetBytes("");
                }    
                packer.WriteByte(__type);
                packer.WriteByte(Convert.ToByte(__pathToFile.Length));
                packer.WriteByte(Convert.ToByte(__paramsOfExe.Length)); 
                packer.Write(__pathToFile, 0, __pathToFile.Length);
                packer.Write(__paramsOfExe, 0, __paramsOfExe.Length);
                return packer.ToArray();
            }
            else if(__type == __type2)
            {
                packer.WriteByte(__type);
                packer.WriteByte(Convert.ToByte(__pathToFile.Length));
                packer.Write(__pathToFile, 0, __pathToFile.Length);
                return packer.ToArray();
            }
            else if(__type == __type3)
            {
                packer.WriteByte(__type);
                packer.WriteByte(Convert.ToByte(__pathToFile.Length));           
                packer.Write(BitConverter.GetBytes((Int32)__fileBytes.Length));
                packer.Write(__pathToFile, 0, __pathToFile.Length);
                packer.Write(__fileBytes, 0, __fileBytes.Length);
                return packer.ToArray();
            }
            else if(__type == __type4)
            {
                packer.WriteByte(__type);
                packer.WriteByte(Convert.ToByte(__pathToFile.Length));
                packer.WriteByte(Convert.ToByte(__pathToGetFile.Length));
                packer.Write(__pathToFile, 0, __pathToFile.Length);
                packer.Write(__pathToGetFile, 0, __pathToGetFile.Length);
                return packer.ToArray();
            }
            else if(__type == __type5)
            {
                packer.WriteByte(__type);
                packer.WriteByte(Convert.ToByte(__pathToGetFile.Length));
                packer.Write(BitConverter.GetBytes((Int32)__fileBytes.Length));
                packer.Write(__pathToGetFile, 0, __pathToGetFile.Length);
                packer.Write(__fileBytes, 0, __fileBytes.Length);
                return packer.ToArray();
            }
            packer.WriteByte(__unknownType);
            return packer.ToArray();
        }
        public static TMPD1Packet ToParse(byte[] buff) // парсим полученный пакет
        {
            var __type = buff[0];
            int type;
            switch(__type)
            {
                case __type0:
                    type = 0;
                    break;
                case __type1:
                    type = 1;
                    break;
                case __type2:
                    type = 2;
                    break;
                case __type3:
                    type = 3;
                    break;
                case __type4:
                    type = 4;
                    break;
                case __type5:
                    type = 5;
                    break;
                default:
                    type = 6;
                    break;
            }

            var newPacket = new TMPD1Packet(type);
            if (__type == __type0)
            {
                var sizeOfReply = Convert.ToInt32(buff[1]);
                newPacket.__reply = buff.Skip(2).Take(sizeOfReply).ToArray();
                return newPacket;
            }
            else if(__type == __type1)
            {
                var sizeOfPath = Convert.ToInt32(buff[1]);
                var sizeOfParams = Convert.ToInt32(buff[2]);
                newPacket.__pathToFile = buff.Skip(3).Take(sizeOfPath).ToArray();
                newPacket.__paramsOfExe = buff.Skip(3 + sizeOfPath).Take(sizeOfParams).ToArray();
                return newPacket;
            }
            else if(__type == __type2)
            {
                var sizeOfPath = Convert.ToInt32(buff[1]);
                newPacket.__pathToFile = buff.Skip(2).Take(sizeOfPath).ToArray();
                return newPacket;
            }
            else if(__type == __type3)
            {
                var sizeOfPath = Convert.ToInt32(buff[1]);
                var sizeOfFile = BitConverter.ToInt32(buff.Skip(2).Take(4).ToArray());
                newPacket.__pathToFile = buff.Skip(6).Take(sizeOfPath).ToArray();
                newPacket.__fileBytes = buff.Skip(6 + sizeOfPath).Take(sizeOfFile).ToArray();
                return newPacket;
            }
            else if(__type == __type4)
            {
                var sizeOfPath = Convert.ToInt32(buff[1]);
                var sizeOfGettingPath = Convert.ToInt32(buff[2]);
                newPacket.__pathToFile = buff.Skip(3).Take(sizeOfPath).ToArray();
                newPacket.__pathToGetFile = buff.Skip(3 + sizeOfPath).Take(sizeOfGettingPath).ToArray();
                return newPacket;
            }
            else if(__type == __type5)
            {
                var sizeOfGettingPath = Convert.ToInt32(buff[1]);
                var sizeOfFile = BitConverter.ToInt32(buff.Skip(2).Take(4).ToArray());
                newPacket.__pathToGetFile = buff.Skip(6).Take(sizeOfGettingPath).ToArray();
                newPacket.__fileBytes = buff.Skip(6 + sizeOfGettingPath).Take(sizeOfFile).ToArray();
                return newPacket;
            }
            newPacket.SetReply("Неизвестный тип пакета");
            return newPacket;
        }

        public string GetPath()
        {
            return Encoding.UTF8.GetString(__pathToFile);
        }
        public string GetPathForClient()
        {
            return Encoding.UTF8.GetString(__pathToGetFile);
        }
        public string GetParams()
        {
            return Encoding.UTF8.GetString(__paramsOfExe);
        }
        public string GetReply()
        {
            return Encoding.UTF8.GetString(__reply);
        }
        public byte[] GetFileBytes()
        {
            return __fileBytes;
        }
        public byte GetTypeOfPacket()
        {
            return __type;
        }
    }

    public class ManagerOfPackets
    {
        private TMPD1Packet testPacket;
        public ManagerOfPackets(TMPD1Packet __testPacket)
        {
            testPacket = __testPacket;
        }
        public TMPD1Packet DirtyWork()
        {
            switch(testPacket.GetTypeOfPacket())
            {
                case TMPD1Packet.__type0:
                    return SeeTheResult();
                case TMPD1Packet.__type1:
                    return ExecuteSomeProgram();
                case TMPD1Packet.__type2:
                    return DeleteTheFile();
                case TMPD1Packet.__type3:
                    return DownLoadFileToServer();
                case TMPD1Packet.__type4:
                    return UploadFile();
                case TMPD1Packet.__type5:
                    return DownLoadFileToClient();
                default:
                    TMPD1Packet answer = new TMPD1Packet(6);
                    Console.WriteLine("Неизвестный пакет");
                    return answer;
            }
        }

        private TMPD1Packet ExecuteSomeProgram()
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash", $"-c \"{testPacket.GetPath()} {testPacket.GetParams()}\"");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;

            proc.Start();
            StreamReader reader1 = proc.StandardOutput;
            StreamReader reader2 = proc.StandardError;
            
            string message = reader1.ReadLine();
            if(String.IsNullOrEmpty(message))
            {
                message = reader2.ReadLine();
            }

            TMPD1Packet answer = new TMPD1Packet(0);
            answer.SetReply(message);
            return answer;
        }

        private TMPD1Packet DeleteTheFile()  // удаляет файл по указанному пути либо возвращает сообщение об ошибке
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash", $"-c \"rm {testPacket.GetPath()}\"");
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            StreamReader reader = proc.StandardError;
            string message = reader.ReadLine();
            if(String.IsNullOrEmpty(message))
            {
                message = "файл успешно удалён";
            }

            TMPD1Packet answer = new TMPD1Packet(0);
            answer.SetReply(message);
            return answer;
        }
        private TMPD1Packet DownLoadFileToServer()
        {
            string message;
            try
            {
                FileInfo fileInfo = new FileInfo(testPacket.GetPath());
                FileStream fs = fileInfo.Create();
                fs.Write(testPacket.GetFileBytes());
                fs.Close();
                message = "файл успешно загрузился";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }      

            TMPD1Packet answer = new TMPD1Packet(0);
            answer.SetReply(message);
            return answer;
        }
        private TMPD1Packet UploadFile()
        {
            TMPD1Packet answer = new TMPD1Packet(5);

            answer.SetFileBytes(testPacket.GetPath());
            answer.SetPathToGetFile(testPacket.GetPathForClient());

            return answer;
        }
        private TMPD1Packet DownLoadFileToClient()
        {
            string message;
            try
            {
                FileInfo fileInfo = new FileInfo(testPacket.GetPathForClient());
                FileStream fs = fileInfo.Create();
                fs.Write(testPacket.GetFileBytes());
                fs.Close();
                message = "файл успешно загрузился";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }      

            Console.WriteLine(message);

            TMPD1Packet answer = new TMPD1Packet(0);
            answer.SetReply("ignore");
            return answer;
        }
        private TMPD1Packet SeeTheResult()
        {
            Console.WriteLine(testPacket.GetReply());

            TMPD1Packet answer = new TMPD1Packet(0);
            answer.SetReply("ignore");
            return answer;

        }

    }
}

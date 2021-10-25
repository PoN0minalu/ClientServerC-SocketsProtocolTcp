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
        public const byte __type3 = 0x45; // пакет с загрузкой указанного файла в систему
        public const byte __type4 = 0x4A; // пакет с получением указанного файла из системы
        private byte[] __pathToFile; // путь к файлуS
        private byte[] __paramsOfExe; // параметры запуска программы
        private byte[] __reply; // ответ от сервера

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
                default:
                    __type = __type0;
                    break;
            }
        }
        public void SetPathToFile(string pathToFile) //записываем путь к файлу пакет
        {
            __pathToFile = Encoding.UTF8.GetBytes(pathToFile);
        }
        public void SetParamsOfExe(string parametrs)
        {
            __paramsOfExe = Encoding.UTF8.GetBytes(parametrs); //записываем параметры
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
            packer.WriteByte(__type);
            packer.WriteByte(Convert.ToByte(__pathToFile.Length));
            packer.WriteByte(Convert.ToByte(__paramsOfExe.Length));
            packer.Write(__pathToFile, 0, __pathToFile.Length);
            packer.Write(__paramsOfExe, 0, __paramsOfExe.Length);
            return packer.ToArray();
        }
        public static TMPD1Packet ToParse(byte[] buff) // парсим полученный пакет
        {
            var __type = buff[0];
            int type;
            switch(__type)
            {
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
                default:
                    type = 0;
                    break;
            }

            var newPacket = new TMPD1Packet(type);
            if (__type == __type0)
            {
                var sizeOfReply = Convert.ToInt32(buff[1]);
                newPacket.__reply = buff.Skip(2).Take(sizeOfReply).ToArray();
                return newPacket;
            }
            var sizeOfPath = Convert.ToInt32(buff[1]);
            var sizeOfParams = Convert.ToInt32(buff[2]);
            newPacket.__pathToFile = buff.Skip(3).Take(sizeOfPath).ToArray();
            newPacket.__paramsOfExe = buff.Skip(3 + sizeOfPath).ToArray();
            return newPacket;
        }

        public string GetPath()
        {
            return Encoding.UTF8.GetString(__pathToFile);
        }
        public string GetParams()
        {
            return Encoding.UTF8.GetString(__paramsOfExe);
        }
        public string GetReply()
        {
            return Encoding.UTF8.GetString(__reply);
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

        public string DirtyWork()
        {
            string result;
            switch(testPacket.GetTypeOfPacket())
            {
                case TMPD1Packet.__type1:
                    result = ExecuteSomeProgram();
                    break;
                case TMPD1Packet.__type2:
                    result = DeleteTheFile();
                    break;
                default:
                    result = "Неизвестный тип";
                    break;
            }
            return result;
        }

        private string ExecuteSomeProgram()
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
            return message;
        }

        private string DeleteTheFile()  // удаляет файл по указанному пути либо возвращает сообщение об ошибке
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
            return message;
        }

    }
}

    



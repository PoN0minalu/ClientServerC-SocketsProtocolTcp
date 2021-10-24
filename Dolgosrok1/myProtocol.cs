using System;
using System.IO;
using System.Text;
using System.Linq;


namespace myFirstProtocol
{
    public class TMPD1Packet
    {
        private int __size {get; set;}
        private byte __type {get; set;}
        private const byte __type0 = 0xFF; //пустой пакет
        private const byte __type1 = 0xD; // пакет с запуском программы в консоли
        private const byte __type2 = 0x1E; // пакет с удалением файла через консоль
        private const byte __type3 = 0x45; // пакет с загрузкой указанного файла в систему
        private const byte __type4 = 0x4A; // пакет с получением указанного файла из системы

        private byte[] __pathToFile {get; set;} // массив байтов с полями данных

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
            __pathToFile = Encoding.ASCII.GetBytes(pathToFile);
        }
        public byte[] ToPack() //упаковываем все в массив байтов
        {
            var packer = new MemoryStream();
            packer.WriteByte(__type);
            packer.Write(__pathToFile, 0, __pathToFile.Length);
            return packer.ToArray();
        }
        public static TMPD1Packet ToParse(byte[] buff)
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
            newPacket.__pathToFile = buff.Skip(1).ToArray();

            return newPacket;
        }

        public string GetPath()
        {
            return Encoding.ASCII.GetString(__pathToFile);
        }
        
    }
}
    



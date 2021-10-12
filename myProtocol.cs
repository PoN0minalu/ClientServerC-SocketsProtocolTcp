using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;

namespace myFirstProtocol
{
    public class XPacket
    {
        public byte PacketType { get; private set; }
        public byte PacketSubtype { get; private set; }
        public List<XPacketField> Fields { get; set; } = new List<XPacketField>();
        private XPacket() {}
 
        public static XPacket Create(byte type, byte subtype)
        {
            return new XPacket
            {
                PacketType = type,
                PacketSubtype = subtype
            };
        }
         public byte[] ToPacket()
        {
            var packet = new MemoryStream();
 
            packet.Write(
            new byte[] {0xAF, 0xAA, 0xAF, PacketType, PacketSubtype}, 0, 5);
 
            var fields = Fields.OrderBy(field => field.FieldID);
 
            foreach (var field in fields)
            {
                packet.Write(new[] {field.FieldID, field.FieldSize}, 0, 2);
                packet.Write(field.Contents, 0, field.Contents.Length);
            }
 
            packet.Write(new byte[] {0xFF, 0x00}, 0, 2);
 
            return packet.ToArray();
        }

        public static XPacket Parse(byte[] packet)
        {
            if (packet.Length < 7)
            {   
                return null;
            }
 
            if (packet[0] != 0xAF ||
                packet[1] != 0xAA ||
                packet[2] != 0xAF)
            {
                return null;
            }
 
            var mIndex = packet.Length - 1;
 
            if (packet[mIndex - 1] != 0xFF ||
                packet[mIndex] != 0x00)
            {
                return null;
            }
 
            var type = packet[3];
            var subtype = packet[4];
 
            var xpacket = Create(type, subtype);
            var fields = packet.Skip(5).ToArray();
 
            while (true)
            {
                if (fields.Length == 2)
                {
                    return xpacket;
                }
 
                var id = fields[0];
                var size = fields[1];
 
                var contents = size != 0 ?
                fields.Skip(2).Take(size).ToArray() : null;
 
                xpacket.Fields.Add(new XPacketField
                {
                    FieldID = id,
                    FieldSize = size,
                    Contents = contents
                });
 
                fields = fields.Skip(2 + size).ToArray();
            }
        }
        public byte[] FixedObjectToByteArray(object value)
        {
            var rawsize = Marshal.SizeOf(value);
            var rawdata = new byte[rawsize];
 
            var handle = GCHandle.Alloc(rawdata,
            GCHandleType.Pinned);
 
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
 
            handle.Free();
 
            return rawdata;
        }
        private T ByteArrayToFixedObject<T>(byte[] bytes) where T: struct 
        {
            T structure;
 
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
 
            try
            {
                structure = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
 
            return structure;
        }
        public XPacketField GetField(byte id)
        {
            foreach (var field in Fields)
            {
                if (field.FieldID == id)
                {
                    return field;
                }
            }
 
            return null;
        }
        public bool HasField(byte id)
        {
            return GetField(id) != null;
        }
        public T GetValue<T>(byte id) where T : struct
        {
            var field = GetField(id);
 
            if (field == null)
            {
                throw new Exception($"Field with ID {id} wasn't found.");
            }
            var neededSize = Marshal.SizeOf(typeof(T));
 
            if (field.FieldSize != neededSize)
            {
                throw new Exception($"Can't convert field to type {typeof(T).FullName}.\n" + $"We have {field.FieldSize} bytes but we need exactly {neededSize}.");
            }
 
            return ByteArrayToFixedObject<T>(field.Contents);
        }
        public void SetValue(byte id, object structure)
        {
            if (!structure.GetType().IsValueType)
            {
                throw new Exception("Only value types are available.");
            }
 
            var field = GetField(id);
 
            if (field == null)
            {
                field = new XPacketField
                {
                    FieldID = id
                };
 
                Fields.Add(field);
            }
 
            var bytes = FixedObjectToByteArray(structure);
 
            if (bytes.Length > byte.MaxValue)
            {
                throw new Exception("Object is too big. Max length is 255 bytes.");
            }
 
            field.FieldSize = (byte) bytes.Length;
            field.Contents = bytes;
        } 
    }
    public class XPacketField
    {
        public byte FieldID { get; set; }
        public byte FieldSize { get; set; }
        public byte[] Contents { get; set; }
        
    }
    public enum XPacketType
    {
        Unknown,
        Handshake
    }
    public static class XPacketTypeManager
    {
        private static readonly Dictionary<XPacketType, Tuple<byte, byte>> TypeDictionary = new Dictionary<XPacketType, Tuple<byte, byte>>();
        public static void RegisterType(XPacketType type, byte btype, byte bsubtype)
        {   
            if (TypeDictionary.ContainsKey(type))
            {
                throw new Exception($"Packet type {type:G} is already registered.");
            }
 
            TypeDictionary.Add(type, Tuple.Create(btype, bsubtype));
        }
        public static Tuple<byte, byte> GetType(XPacketType type)
        {
            if (!TypeDictionary.ContainsKey(type))
            {
                throw new Exception($"Packet type {type:G} is not registered.");
            }
 
            return TypeDictionary[type];
        }
        public static XPacketType GetTypeFromPacket(XPacket packet)
        {
            var type = packet.PacketType;
            var subtype = packet.PacketSubtype;
 
            foreach (var tuple in TypeDictionary)
            {
                var value = tuple.Value;
 
                if (value.Item1 == type && value.Item2 == subtype)
                {
                    return tuple.Key;
                }
            }
 
            return XPacketType.Unknown;
        }
         
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class XFieldAttribute : Attribute
    {
        public byte FieldID { get; }
 
        public XFieldAttribute(byte fieldId)
        {
            FieldID = fieldId;
        }
        private static List<Tuple<FieldInfo, byte>> GetFields(Type t)
        {
            return t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(field => field.GetCustomAttribute<XFieldAttribute>() != null).Select(field => Tuple.Create(field, field.GetCustomAttribute<XFieldAttribute>().FieldID)).ToList();
        }
         public static XPacket Serialize(byte type, byte subtype, object obj, bool strict = false)
        {
            var fields = GetFields(obj.GetType());
 
            if (strict)
            {
                var usedUp = new List<byte>();
 
                foreach (var field in fields)
                {
                    if (usedUp.Contains(field.Item2))
                    {
                        throw new Exception("One field used two times.");
                    }
 
                    usedUp.Add(field.Item2);
                }
            }
 
            var packet = XPacket.Create(type, subtype);
 
            foreach (var field in fields)
            {
                packet.SetValue(field.Item2, field.Item1.GetValue(obj));
            }
 
            return packet;
        }
        public static T Deserialize<T>(XPacket packet, bool strict = false)
        {
            var fields = GetFields(typeof(T));
            var instance = Activator.CreateInstance<T>();
 
            if (fields.Count == 0)
            {
                return instance;
            }
            foreach (var tuple in fields)
            {
                var field = tuple.Item1;
                var packetFieldId = tuple.Item2;
 
                if (!packet.HasField(packetFieldId))
                {
                    if (strict)
                    {
                        throw new Exception($"Couldn't get field[{packetFieldId}] for {field.Name}");
                    }
 
                    continue;
                }
 
            var value = typeof(XPacket)
            .GetMethod("GetValue")?
            .MakeGenericMethod(field.FieldType)
            .Invoke(packet, new object[] {packetFieldId});
 
            if (value == null)
            {
                if (strict)
                {
                    throw new Exception($"Couldn't get value for field[{packetFieldId}] for {field.Name}");
                }
 
                continue;
            }
 
                field.SetValue(instance, value);
            }
 
            return instance;
        }
    }
    class TestPacket
    {
        [XField(0)]
        public int TestNumber;
 
        [XField(1)]
        public double TestDouble;
 
        [XField(2)]
        public bool TestBoolean;
    }
    public class XPacketHandshake
    {
        [XField(1)]
        public int MagicHandshakeNumber;
    }
    
}
    



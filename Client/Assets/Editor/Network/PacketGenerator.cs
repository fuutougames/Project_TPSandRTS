using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text;

namespace PacketUtilities
{
    public class PacketInfo
    {
        public string PacketName;
        public int PacketID;
        // fieldName, fieldTypeName
        public List<KeyValuePair<string, string>> AttribList;
    }
    public class PacketGenerator
    {
        private static readonly Dictionary<string, string> m_TypeMapper = new Dictionary<string, string>()
        {
            { "System.Byte", "byte" },
            { "System.Int16", "short" },
            { "System.Int32", "int" },
            { "System.Int64", "System.Int64" },
            { "System.Single", "float" },
            { "System.Boolean", "bool" },
            { "System.String", "string" }
        };

        private static readonly HashSet<string> m_BasicTypeSet = new HashSet<string>()
        {
            "byte",
            "short",
            "int",
            "System.Int64",
            "float",
            "bool",
            "string"
        };

        private static readonly Dictionary<string, string> m_WriteMethodsMap = new Dictionary<string, string>()
        {
            { "byte", "WriteByte" },
            { "short", "WriteShort" },
            { "int", "WriteInt" },
            { "System.Int64", "WriteInt64" },
            { "float", "WriteFloat" },
            { "bool", "WriteBool" },
            { "string", "WriteString" }
        };

        private static readonly Dictionary<string, string> m_ReadMethodsMap = new Dictionary<string, string>()
        {
            { "byte", "ReadByte" },
            { "short", "ReadShort" },
            { "int", "ReadInt" },
            { "System.Int64", "ReadInt64" },
            { "float", "ReadFloat" },
            { "bool", "ReadBool" },
            { "string", "ReadString" }
        };

        [MenuItem("Tools/Network/GeneratePackets")]
        public static void Process()
        {
            MakeBackup();
            DeleteDirRecursively(NetworkEditorConst.PACKET_GENERATE_PATH);
            Directory.CreateDirectory(NetworkEditorConst.PACKET_GENERATE_PATH);
            Directory.CreateDirectory(NetworkEditorConst.ASSEMBLER_GENERATE_PATH);

            List<PacketInfo> infos = ExtractPacketInfo();
            GeneratePacketID(infos);
            GenerateAssemblers(infos);
            GenerateDisassemblers(infos);
            GeneratePackets(infos);

            AssetDatabase.Refresh();
        }

        public static void MakeBackup()
        {
            string path = NetworkEditorConst.BACKUP_PATH + System.DateTime.Now.ToFileTimeUtc().ToString() + '/';

            string packetsBackupPath = path + "Scripts/Network/Packets/";
            string assemblersBackupPath = path + "Scripts/Network/Utilities/";

            CopyFilesFromTo(NetworkEditorConst.PACKET_GENERATE_PATH, packetsBackupPath);
            CopyFilesFromTo(NetworkEditorConst.ASSEMBLER_GENERATE_PATH, assemblersBackupPath);
        }

        public static List<PacketInfo> ExtractPacketInfo()
        {
            string nspace = "Network.Packets.Structs";
            Assembly assembly = Assembly.LoadFrom(NetworkEditorConst.ASSEMBLY_PATH);
            var query = from t in assembly.GetTypes()
                        where t.IsClass && t.Namespace == nspace
                        select t;

            List<Type> types = query.ToList();

            Dictionary<string, int> packetNameIDMap = new Dictionary<string, int>();
            HashSet<int> packetIDSet = new HashSet<int>();
            Type packetIDType = types.Find(type => type.Name == "PacketID");
            if (packetIDType != null)
            {
                FieldInfo[] packetIDFields = packetIDType.GetFields();
                foreach (FieldInfo field in packetIDFields)
                {
                    try
                    {
                        if (packetIDSet.Contains((int)field.GetValue(null)))
                        {
                            Debug.LogError(string.Format("Packet ID-{0} is duplicated!", (int)field.GetValue(null)));
                            return null;
                        }
                        packetNameIDMap.Add(field.Name, (int)field.GetValue(null));
                        packetIDSet.Add((int)field.GetValue(null));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("Packet Format Error! Field {0} is not able to convert to short", field.Name));
                        throw e;
                    }
                }
            }

            List<PacketInfo> packetInfos = new List<PacketInfo>();
            int autoPacketId = 0;
            foreach (Type t in types)
            {
                if (t.Name == "PacketID")
                    continue;

                int packId;
                if (!packetNameIDMap.TryGetValue(t.Name, out packId))
                {
                    do
                    {
                        ++autoPacketId;
                        if (autoPacketId >= ushort.MaxValue)
                        {
                            Debug.LogError("Pack ID Generate failed, too much packet defined!");
                            return null;
                        }
                    } while (packetIDSet.Contains(autoPacketId));

                    packId = autoPacketId;

                    packetIDSet.Add(packId);
                    packetNameIDMap.Add(t.Name, packId);


                    Debug.Log(string.Format("<color=blue>Packet {0}'s ID is not assigned, automatically assign as {1}</color>", t.Name, packId));
                }

                if (packId > ushort.MaxValue)
                {
                    Debug.Log(string.Format("Packet ID of {0} is not valid, packet must lay within range [1, {1}]", t.Name, ushort.MaxValue));
                    return null;
                }
            }

            foreach (Type t in types)
            { 
                if (t.Name == "PacketID")
                    continue;

                int packId = packetNameIDMap[t.Name];
                List<KeyValuePair<string, string>> attribs = new List<KeyValuePair<string, string>>();
                FieldInfo[] fields = t.GetFields();
                // TODO: assembly attrib list
                for (int i = 0; i < fields.Length; ++i)
                {
                    FieldInfo field = fields[i];
                    string attribType;
                    if (field.FieldType.FullName.Contains("Network.Packets.Structs.")
                        && packetNameIDMap.ContainsKey(field.FieldType.FullName.Substring(24)))
                    {
                        attribType = field.FieldType.FullName.Substring(24);
                        if (attribType == t.Name)
                        {
                            Debug.LogError(string.Format("{0} parse error, recursively definition is not allow!", t.Name));
                            return null;
                        }
                        attribType = NetworkEditorConst.GENERATED_PACKET_PREFIX + attribType;
                    }
                    else if (!m_TypeMapper.TryGetValue(field.FieldType.FullName, out attribType))
                    {
                        if (!field.FieldType.FullName.Contains("List"))
                        {
                            Debug.LogError(string.Format("Type of {0} in {1} is not support!", field.Name, t.Name));
                            return null;
                        }

                        Regex rx = new Regex(@"(?:\[\[)(?<metastr>.*)(?:\]\])", RegexOptions.Compiled);
                        Match match = rx.Match(field.FieldType.FullName);
                        if (!match.Success)
                        {
                            Debug.LogError(string.Format("{0} in {1} type parse error!", field.Name, t.Name));
                            return null;
                        }

                        string metaStr = match.Groups["metastr"].Value;
                        string typename = metaStr.Split(',')[0].Trim();

                        if (!m_TypeMapper.TryGetValue(typename, out attribType))
                        {
                            if (!typename.Contains("Network.Packets.Structs."))
                            {
                                Debug.LogError(string.Format("Type of {0} in {1} is not support!", field.Name, t.Name));
                                Debug.LogError(string.Format("List template type must be one of classes under" +
                                    " namespace Network.Packets.Structs or system type defined in PacketGenerator.m_TypeMapper"));
                                return null;
                            }

                            attribType = "List<" + NetworkEditorConst.GENERATED_PACKET_PREFIX + typename.Substring(24) + ">";
                        }
                        else
                        {
                            attribType = "List<" + attribType + ">";
                        }

                    }
                    attribs.Add(new KeyValuePair<string, string>(field.Name, attribType));
                }

                PacketInfo info = new PacketInfo
                {
                    PacketName = NetworkEditorConst.GENERATED_PACKET_PREFIX + t.Name,
                    PacketID = packId,
                    AttribList = attribs
                };

                packetInfos.Add(info);
            }

            packetInfos.Sort((item1, item2) =>
            {
                if (item1.PacketID < item2.PacketID)
                    return -1;
                if (item1.PacketID > item2.PacketID)
                    return 1;
                return 0;
            });

            return packetInfos;
        }

        public static void GeneratePacketID(List<PacketInfo> infos)
        {
            string content = ReadFile(NetworkEditorConst.TEMPLATE_PATH + NetworkEditorConst.TEMPLATEFILENAME_PACKETID);
            StringBuilder builder = new StringBuilder();
            builder.Append('\n');
            for (int i = 0; i < infos.Count; ++i)
            {
                PacketInfo info = infos[i];
                string codeLine = string.Format(NetworkEditorConst.PACKET_ID_CODE_TEMPLATE, info.PacketName, info.PacketID);
                builder.Append(codeLine);
            }
            content = content.Replace("#PACKET_ID#", builder.ToString());
            string path = NetworkEditorConst.PACKET_GENERATE_PATH + "PacketID.cs";
            WriteFile(path, content);
        }

        public static void GenerateAssemblers(List<PacketInfo> infos)
        {
            string content = ReadFile(NetworkEditorConst.TEMPLATE_PATH + NetworkEditorConst.TEMPLATEFILENAME_ASSEMBLERS);
            StringBuilder mapItemsBuilder = new StringBuilder();
            StringBuilder assemblerBuilder = new StringBuilder();
            for (int i = 0; i < infos.Count; ++i)
            {
                PacketInfo info = infos[i];
                mapItemsBuilder.Append(string.Format(NetworkEditorConst.FACTORY_MAP_ITEM_TEMPLATE, info.PacketName, "Assembler"));
                assemblerBuilder.Append(string.Format(NetworkEditorConst.ASSEMBLER_TEMPLATE, info.PacketName));
            }
            content = content.Replace("#ASSEMBLER_MAP#", mapItemsBuilder.ToString());
            content = content.Replace("#ASSEMBLERS#", assemblerBuilder.ToString());
            string path = NetworkEditorConst.ASSEMBLER_GENERATE_PATH + "Assemblers.cs";
            WriteFile(path, content);
        }

        public static void GenerateDisassemblers(List<PacketInfo> infos)
        {
            string content = ReadFile(NetworkEditorConst.TEMPLATE_PATH + NetworkEditorConst.TEMPLATEFILENAME_DISASSEMBLERS);
            StringBuilder mapItemsBuilder = new StringBuilder();
            StringBuilder disassemblerBuilder = new StringBuilder();
            for (int i = 0; i < infos.Count; ++i)
            {
                PacketInfo info = infos[i];
                mapItemsBuilder.Append(string.Format(NetworkEditorConst.FACTORY_MAP_ITEM_TEMPLATE, info.PacketName, "Disassembler"));
                disassemblerBuilder.Append(string.Format(NetworkEditorConst.DISASSEMBLER_TEMPLATE, info.PacketName));
            }
            content = content.Replace("#DISASSEMBLER_MAP#", mapItemsBuilder.ToString());
            content = content.Replace("#DISASSEMBLERS#", disassemblerBuilder.ToString());
            string path = NetworkEditorConst.ASSEMBLER_GENERATE_PATH + "Disassemblers.cs";
            WriteFile(path, content);
        }

        public static void GenerateSinglePacket(PacketInfo info)
        {
            string content = ReadFile(NetworkEditorConst.TEMPLATE_PATH + NetworkEditorConst.TEMPLATEFILENAME_PACKET);
            StringBuilder attributeBuilder = new StringBuilder();
            StringBuilder readerBuilder = new StringBuilder();
            StringBuilder writerBuilder = new StringBuilder();
            for (int i = 0; i < info.AttribList.Count; ++i)
            {
                KeyValuePair<string, string> attribInfo = info.AttribList[i];
                if (attribInfo.Value.Contains("List"))
                {
                    attributeBuilder.Append(string.Format(NetworkEditorConst.PACKET_LIST_ATTRIBUTE_TEMPLATE, attribInfo.Value, attribInfo.Key));
                }
                else
                {
                    attributeBuilder.Append(string.Format(NetworkEditorConst.PACKET_ATTRIBUTE_TEMPLATE, attribInfo.Value, attribInfo.Key));
                }
                string writeCodeLine;
                string readCodeLine;
                if (m_BasicTypeSet.Contains(attribInfo.Value))
                {
                    // basic type
                    string writeMethod = m_WriteMethodsMap[attribInfo.Value];
                    string readMethod = m_ReadMethodsMap[attribInfo.Value];
                    writeCodeLine = string.Format(NetworkEditorConst.PACKET_ATTRIBUTE_WRITE_TEMPLATE, attribInfo.Key, writeMethod);
                    readCodeLine = string.Format(NetworkEditorConst.PACKET_ATTRIBUTE_READ_TEMPLATE, attribInfo.Key, readMethod);
                }
                else if (attribInfo.Value.Contains("List"))
                {
                    // list
                    Regex rx = new Regex(@"[<](?<listtype>.+)[>]", RegexOptions.Compiled);
                    Match match = rx.Match(attribInfo.Value);
                    string listTemplateType = match.Groups["listtype"].Value;
                    if (m_BasicTypeSet.Contains(listTemplateType))
                    {
                        // a basic type list
                        string writeMethod = m_WriteMethodsMap[listTemplateType];
                        string readMethod = m_ReadMethodsMap[listTemplateType];
                        writeCodeLine = string.Format(NetworkEditorConst.PACKET_WRITE_BASICTYPE_LIST_TEMPLATE, attribInfo.Key, writeMethod);
                        readCodeLine = string.Format(NetworkEditorConst.PACKET_READ_BASICTYPE_LIST_TEMPLATE, attribInfo.Key, readMethod);
                    }
                    else
                    {
                        // a packet type list
                        writeCodeLine = string.Format(NetworkEditorConst.PACKET_WRITE_PACKET_LIST_TEMPLATE, attribInfo.Key);
                        readCodeLine = string.Format(NetworkEditorConst.PACKET_READ_PACKET_LIST_TEMPLATE, attribInfo.Key, listTemplateType);
                    }
                    
                }
                else
                {
                    // packet
                    writeCodeLine = string.Format(NetworkEditorConst.PACKET_WRITE_PACKET_TYPE_TEMPLATE, attribInfo.Key);
                    readCodeLine = string.Format(NetworkEditorConst.PACKET_READ_PACKET_TYPE_TEMPLATE, attribInfo.Key, attribInfo.Value);
                }

                readerBuilder.Append(readCodeLine);
                writerBuilder.Append(writeCodeLine);
            }
            string getPacketIDCode = string.Format(NetworkEditorConst.PACKET_GET_PACKETID_TEMPLATE, info.PacketName);
            content = content.Replace("#PACKET_GETID#", getPacketIDCode);
            content = content.Replace("#PACKET_ATTRIBS#", attributeBuilder.ToString());
            content = content.Replace("#PACKET_READ#", readerBuilder.ToString());
            content = content.Replace("#PACKET_WRITE#", writerBuilder.ToString());
            content = content.Replace("#PACKET_NAME#", info.PacketName);
            string path = NetworkEditorConst.PACKET_GENERATE_PATH + info.PacketName + ".cs";
            WriteFile(path, content);
        }

        public static List<KeyValuePair<string, string>> GeneratePackets(List<PacketInfo> infos)
        {
            List<KeyValuePair<string, string>> packets = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < infos.Count; ++i)
            {
                GenerateSinglePacket(infos[i]);
            }
            return packets;
        }


        #region Utilities
        public static string ReadFile(string path)
        {
            string content = File.ReadAllText(path);
            return content;
        }

        public static void WriteFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public static void DeleteDirRecursively(string path)
        {
            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(path);
            dirInfo.Delete(true);
        }

        public static void CopyFilesFromTo(string from, string to)
        {
            Directory.CreateDirectory(to);

            DirectoryInfo fromDirInfo = new DirectoryInfo(from);

            foreach (FileInfo file in fromDirInfo.GetFiles())
            {
                File.Copy(file.FullName, to + file.Name);
            }
            foreach (DirectoryInfo dir in fromDirInfo.GetDirectories())
            {
                CopyFilesFromTo(from + dir.Name + '/', to + dir.Name + '/');
            }
        }
        #endregion
    }
}

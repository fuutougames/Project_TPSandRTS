using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NetworkEditorConst
{
    public static readonly string TEMPLATE_PATH = Application.dataPath + "/Editor/Network/Templates/";
    public static readonly string PACKET_GENERATE_PATH = Application.dataPath + "/Scripts/Core/Network/Packets/";
    public static readonly string ASSEMBLER_GENERATE_PATH = Application.dataPath + "/Scripts/Core/Network/Utilities/";
    public static readonly string ASSEMBLY_PATH = Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp-Editor.dll";
    public static readonly string BACKUP_PATH = Application.dataPath + "/../PacketCodeBackup/";
    public static readonly string GENERATED_PACKET_PREFIX = "PACKET_";


    #region File Names

    public static readonly string TEMPLATEFILENAME_PACKETID = "PacketIDTemplate.cs.txt";
    public static readonly string TEMPLATEFILENAME_ASSEMBLERS = "Assemblers.cs.txt";
    public static readonly string TEMPLATEFILENAME_DISASSEMBLERS = "Disassemblers.cs.txt";
    public static readonly string TEMPLATEFILENAME_PACKET = "PacketTemplate.cs.txt";

    #endregion


    #region Code Template

    public static readonly string PACKET_ID_CODE_TEMPLATE = "" + 
        GenerateIndent(2) + "public const int {0} = {1};\n";

    public static readonly string FACTORY_MAP_ITEM_TEMPLATE = "" +
        GenerateIndent(3) + "{{\n" +
        GenerateIndent(4) + "PacketID.{0},\n" +
        GenerateIndent(4) + "{1}_{0}\n" +
        GenerateIndent(3) + "}},\n";

    public static readonly string ASSEMBLER_TEMPLATE = "" +
            "\n" +
            GenerateIndent(2) + "private static IPacket Assembler_{0} (ByteArray buffer)\n" +
            GenerateIndent(2) + "{{\n" +
            GenerateIndent(3) + "{0} packet = new {0}();\n" +
            GenerateIndent(3) + "packet.Read(buffer);\n" +
            GenerateIndent(3) + "return packet;\n" +
            GenerateIndent(2) + "}}\n";

    public static readonly string DISASSEMBLER_TEMPLATE = "" +
        "\n" +
        GenerateIndent(2) + "private static void Disassembler_{0} (IPacket packet, ref ByteArray buffer)\n" +
        GenerateIndent(2) + "{{\n" +
        GenerateIndent(3) + "packet.Write(buffer);\n" +
        GenerateIndent(2) + "}}\n";

    public static readonly string PACKET_ATTRIBUTE_TEMPLATE = "" +
        GenerateIndent(2) + "public {0} {1};\n";

    public static readonly string PACKET_LIST_ATTRIBUTE_TEMPLATE = "" +
        GenerateIndent(2) + "public {0} {1} = new {0}();\n";

    public static readonly string PACKET_ATTRIBUTE_WRITE_TEMPLATE = "" +
        GenerateIndent(3) + "buffer.{1}({0});\n";

    public static readonly string PACKET_ATTRIBUTE_READ_TEMPLATE = "" +
        GenerateIndent(3) + "{0} = buffer.{1}();\n";

    public static readonly string PACKET_WRITE_BASICTYPE_LIST_TEMPLATE = "" + 
        GenerateIndent(3) + "buffer.WriteShort((short){0}.Count);\n" +
        GenerateIndent(3) + "for (int i = 0; i < {0}.Count; ++i)\n" +
        GenerateIndent(3) + "{{\n" +
        GenerateIndent(4) + "buffer.{1}({0}[i]);\n" +
        GenerateIndent(3) + "}}\n";
    
    public static readonly string PACKET_READ_BASICTYPE_LIST_TEMPLATE = "" +
        GenerateIndent(3) + "{0}.Clear();\n" + 
        GenerateIndent(3) + "short {0}Len = buffer.ReadShort();\n" +
        GenerateIndent(3) + "for (int i = 0; i < {0}Len; ++i)\n" +
        GenerateIndent(3) + "{{\n" +
        GenerateIndent(4) + "{0}.Add(buffer.{1}());\n" +
        GenerateIndent(3) + "}}\n";

    public static readonly string PACKET_WRITE_PACKET_LIST_TEMPLATE = "" + 
        GenerateIndent(3) + "buffer.WriteShort((short){0}.Count);\n" +
        GenerateIndent(3) + "for (int i = 0; i < {0}.Count; ++i)\n" +
        GenerateIndent(3) + "{{\n" +
        GenerateIndent(4) + "{0}[i].Write(buffer);\n" +
        GenerateIndent(3) + "}}\n";

    public static readonly string PACKET_READ_PACKET_LIST_TEMPLATE = "" + 
        GenerateIndent(3) + "{0}.Clear();\n" + 
        GenerateIndent(3) + "short {0}Len = buffer.ReadShort();\n" +
        GenerateIndent(3) + "for (int i = 0; i < {0}Len; ++i)\n" +
        GenerateIndent(3) + "{{\n" +
        GenerateIndent(4) + "{1} {0}Item = new {1}();\n" +
        GenerateIndent(4) + "{0}Item.Read(buffer);\n" +
        GenerateIndent(4) + "{0}.Add({0}Item);\n" +
        GenerateIndent(3) + "}}\n";

    public static readonly string PACKET_WRITE_PACKET_TYPE_TEMPLATE = "" +
        GenerateIndent(3) + "{0}.Write(buffer);\n";

    public static readonly string PACKET_READ_PACKET_TYPE_TEMPLATE = "" +
        GenerateIndent(3) + "{0} = new {1} ();\n" +
        GenerateIndent(3) + "{0}.Read(buffer);\n";

    public static readonly string PACKET_GET_PACKETID_TEMPLATE = "" +
        GenerateIndent(3) + "return PacketID.{0};\n";

    #endregion


    #region Utilities
    public static string GenerateIndent(int indentCnt, int spaceCnt = 4)
    {
        string singleIndent = "";
        for (int i = 0; i < spaceCnt; ++i)
        {
            singleIndent += " ";
        }

        string indent = "";
        for (int i = 0; i < indentCnt; ++i)
        {
            indent += singleIndent;
        }

        return indent;
    }
    #endregion
}

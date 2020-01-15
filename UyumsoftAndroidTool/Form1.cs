using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;
using System.Xml.Schema;
using System.Xml;

namespace UyumsoftAndroidTool
{
    public partial class Form1 : Form
    {

        public string packageName = "com.example.myproject.models";
        public string destinationPath = "C:\\Users\\muhammet.kaya\\AndroidStudioProjects\\MyProject\\app\\src\\main\\java\\com\\example\\myproject\\models\\";
        public string Namespace ="www.tempuri.org";
        Dictionary<string, List<string>> enumDict = new Dictionary<string, List<string>>();
        Dictionary<string, List<XmlSchemaElement>> complexTypes = new Dictionary<string, List<XmlSchemaElement>>();
        Dictionary<string,List<XmlSchemaElement>> inputParamClasses = new Dictionary<string, List<XmlSchemaElement>>();
        Dictionary<string, List<XmlSchemaElement>> outputParamClasses = new Dictionary<string, List<XmlSchemaElement>>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            getComplexTypesAndEnums();
            printComplexTypeClasses();
            printEnum();
            copyDefaultClasses(destinationPath);
        }
        static void ServiceTest()
        {

            UriBuilder uriBuilder = new UriBuilder(@"http://localhost:51725/WebService1.asmx");
            uriBuilder.Query = "WSDL";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            webRequest.Accept = "text/xml";

            //Submit a web request to get the web service's WSDL
            ServiceDescription serviceDescription;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    serviceDescription = ServiceDescription.Read(stream);
                }
            }
            foreach (PortType portType in serviceDescription.PortTypes)
            {
                foreach (Operation operation in portType.Operations)
                {
                    Console.WriteLine(operation.Name);
                    foreach (var message in operation.Messages)
                    {
                        if (message is OperationInput)
                            Console.Out.WriteLine("Input Message: {0}", ((OperationInput)message).Message.Name);
                        if (message is OperationOutput)
                            Console.Out.WriteLine("Output Message: {0}", ((OperationOutput)message).Message.Name);

                        foreach (System.Web.Services.Description.Message messagePart in serviceDescription.Messages)
                        {
                            if (messagePart.Name != ((OperationMessage)message).Message.Name) continue;

                            foreach (MessagePart part in messagePart.Parts)
                            {
                                Console.Out.WriteLine(part.Name);
                            }
                        }
                    }
                    Console.Out.WriteLine();
                }

            }
            Types types = serviceDescription.Types;
            XmlSchema xmlSchema = types.Schemas[0];

            foreach (object item in xmlSchema.Items)
            {
                XmlSchemaElement schemaElement = item as XmlSchemaElement;
                XmlSchemaComplexType complexType = item as XmlSchemaComplexType;

                if (schemaElement != null)
                {
                    Console.Out.WriteLine("Schema Element: {0}", schemaElement.Name);

                    XmlSchemaType schemaType = schemaElement.SchemaType;
                    XmlSchemaComplexType schemaComplexType = schemaType as XmlSchemaComplexType;

                    if (schemaComplexType != null)
                    {
                        XmlSchemaParticle particle = schemaComplexType.Particle;
                        XmlSchemaSequence sequence =
                            particle as XmlSchemaSequence;
                        if (sequence != null)
                        {
                            foreach (XmlSchemaElement childElement in sequence.Items)
                            {
                                Console.Out.WriteLine("    Element/Type: {0}:{1}", childElement.Name,
                                                      childElement.SchemaTypeName.Name);
                            }
                        }
                    }
                }
                else if (complexType != null)
                {
                    Console.Out.WriteLine("Complex Type: {0}", complexType.Name);
                    OutputElements(complexType.Particle);
                }
                Console.Out.WriteLine();
            }

            Console.Out.WriteLine();
            Console.In.ReadLine();
        }

    
        public void getComplexTypesAndEnums()
        {
            UriBuilder uriBuilder = new UriBuilder(@"http://localhost/WebService1.asmx");
            uriBuilder.Query = "WSDL";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Method = "GET";
            webRequest.Accept = "text/xml";

            //Submit a web request to get the web service's WSDL
            ServiceDescription serviceDescription;
            using (WebResponse response = webRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    serviceDescription = ServiceDescription.Read(stream);
                }
            }
            
            Types types = serviceDescription.Types;
            XmlSchema xmlSchema = types.Schemas[0];
            string schemaTargetNamespace = xmlSchema.TargetNamespace;

            
           

            //get input and output param classes
            foreach(object item in serviceDescription.Messages)
            {
                System.Web.Services.Description.Message mes = item as System.Web.Services.Description.Message;
                string mesname = mes.Name;
                if (mesname.EndsWith("SoapIn"))
                {
                    foreach(MessagePart part in mes.Parts)
                    {
                        if (part.Name == "parameters")
                        {
                           // XmlQualifiedName param=part.Element;
                            inputParamClasses[part.Element.Name]=new List<XmlSchemaElement>();
                        }
                    }

                }
                else if (mesname.EndsWith("SoapOut"))
                {
                    foreach (MessagePart part in mes.Parts)
                    {
                        if (part.Name == "parameters")
                        {
                            // XmlQualifiedName param=part.Element;
                            outputParamClasses[part.Element.Name]=new List<XmlSchemaElement>();
                        }
                    }
                }
            }

            //get complex types and enums
            Dictionary<string, Dictionary<string, List<string>>> fileEnum = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (object item in xmlSchema.Items)
            {
                XmlSchemaComplexType complexType = item as XmlSchemaComplexType;
                XmlSchemaSimpleType simpleType = item as XmlSchemaSimpleType;
                XmlSchemaElement element = item as XmlSchemaElement;


                string fileName = "";
                if (complexType != null)
                {
                    fileName = destinationPath + complexType.Name + ".java";
                    try
                    {
                        //WriteImports(fileName);
                        using (StreamWriter writer = new StreamWriter(fileName, true))
                        {

                            List<XmlSchemaElement> list = OutputElements(complexType.Particle);
                            complexTypes[complexType.Name] = list;

                        }

                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine(Ex.ToString());
                    }


                }
                else if (simpleType != null)
                {
                    XmlSchemaSimpleTypeContent content = simpleType.Content;
                    if (content is XmlSchemaSimpleTypeRestriction)
                    {
                        XmlSchemaSimpleTypeRestriction res = content as XmlSchemaSimpleTypeRestriction;
                        if (res.BaseTypeName != null && Equals(res.BaseTypeName.Name, "string"))
                        {
                            if (!enumDict.ContainsKey(simpleType.Name))
                            {
                                enumDict[simpleType.Name] = new List<string>();
                            }
                            foreach (XmlSchemaFacet en in res.Facets)
                            {
                                if (en is XmlSchemaEnumerationFacet)
                                {
                                    List<string> list = enumDict[simpleType.Name];  //enum fields
                                    if (!list.Contains(en.Value)) list.Add(en.Value);
                                }
                                else
                                {
                                    break;
                                }
                            }

                        }
                    }
                }

                else if (element != null&&inputParamClasses.ContainsKey(element.Name))
                {
                    XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;
                    if (ct != null)
                    {
                       
                        inputParamClasses[element.Name] = OutputElements(ct.Particle);
                    }
                    
                    
                }
                else if (element != null && outputParamClasses.ContainsKey(element.Name))
                {
                    XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;
                    if (ct != null)
                    {

                        outputParamClasses[element.Name] = OutputElements(ct.Particle);
                    }


                }
            }

            //printEnum();
            // Console.Out.WriteLine();
            // Console.In.ReadLine();
        }

        public void printInputParamClasses()
        {

        }

        private void printComplexTypeClasses()
        {
            foreach (KeyValuePair<string, List<XmlSchemaElement>> entry in complexTypes)
            {
                string fileName = destinationPath + entry.Key + ".java";
                try
                {
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.WriteLine("package " +packageName + ";\n");
                        WriteImports(writer);
                        writer.WriteLine("public class {0} {{\n", entry.Key);

                        printFields(entry.Value, writer);
                        printGetPropertyFunc(entry.Value, writer);
                        writer.WriteLine("\npublic int getPropertyCount() {{ return {0}; }}\n", entry.Value.Count);
                        printGetPropertyInfoFunc(entry.Value, writer);
                        printSetPropertyFunc(entry.Value, writer);

                        writer.WriteLine("\n}");
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

        private void printSetPropertyFunc(List<XmlSchemaElement> fields, StreamWriter writer)
        {
            string head = @"
public void setProperty(int index, Object value)
{
    switch (index)
      {";

            writer.WriteLine(head);
            for (int i = 0; i < fields.Count; i++)
            {
                XmlSchemaElement field = fields[i];

                //string type = getClassOfField(field.SchemaTypeName.Name);
                string[] values = getValueOfField(field.SchemaTypeName.Name); //values[0] = defaultvalue  //values [1] = value
                writer.WriteLine(@"          
            case {0} :
               if(value.toString().equalsIgnoreCase(""anyType{{}}""))
                    {1} = {2};
               else
                    {1} = {3};
               
                break;"

                , i, field.Name, values[0],values[1]);

            }
            writer.WriteLine();
            //writer.WriteLine("            default : break;");

            writer.WriteLine("      }\n} ");

        }

        private void printGetPropertyInfoFunc(List<XmlSchemaElement> fields, StreamWriter writer)
        {
            string head = @"@SuppressWarnings(""unchecked"")
public void getPropertyInfo(int index, Hashtable properties, PropertyInfo info)
{
    switch (index)
      {";


            writer.WriteLine(head);
            for (int i = 0; i < fields.Count; i++)
            {
                XmlSchemaElement field = fields[i];

                string type = getClassOfField(field.SchemaTypeName.Name);

                writer.WriteLine(@"          
            case {0} :
                info.name = ""{1}"";
                info.type={2};
                break;"

                , i, field.Name, type);

            }
            writer.WriteLine();
            writer.WriteLine("            default : break;");

            writer.WriteLine("      }\n} ");


        }
        private string getClassOfField(string typename)
        {
            string type = "";
            switch (typename)
            {
                case "string":
                    type = "PropertyInfo.STRING_CLASS";
                    break;
                case "int":
                    type = "PropertyInfo.INTEGER_CLASS";
                    break;
                case "double":
                    type = "BigDecimal.class.getClass()";
                    break;
                case "decimal":
                    type = "BigDecimal.class.getClass()";
                    break;
                case "float":
                    type = "BigDecimal.class.getClass()";
                    break;
                case "boolean":
                    type = "PropertyInfo.BOOLEAN_CLASS";
                    break;
                case "dateTime":
                    type = "Date.class.getClass()";
                    break;
                default:
                    if (complexTypes.ContainsKey(typename))
                    {
                        type = "new " + typename + "().getClass()";
                    } else if (enumDict.ContainsKey(typename))
                    {
                        type = "PropertyInfo.STRING_CLASS";
                    }
                    break;




            }
            return type;
        }
        private string[] getValueOfField(string typename)
        {
            string[] values = new string[2];
            string type = "";
            string defaultValue = "";
            switch (typename)
            {
                case "string":
                    type = "value.toString()";
                    defaultValue="\"\"";
                    break;
                case "int":
                    type = "Integer.parseInt(value.toString())";
                    defaultValue = "0";
                    break;
                case "double":
                    type = "new BigDecimal(value.toString())";
                    defaultValue = "new BigDecimal(0)";
                    break;
                case "decimal":
                    type = "new BigDecimal(value.toString())";
                    defaultValue = "new BigDecimal(0)";
                    break;
                case "float":
                    type = "new BigDecimal(value.toString())";
                    defaultValue = "new BigDecimal(0)";
                    break;
                case "boolean":
                    type = "Boolean.parseBoolean(value.toString())";
                    defaultValue = "false";
                    break;
                case "dateTime":
                    type = "DateUtil.getDate(value.toString())";
                    defaultValue = "new Date(1900, 1, 1)";
                    break;
                default:
                    if (complexTypes.ContainsKey(typename))
                    {
                        type = "("+typename+")value";
                        defaultValue = "null";
                    }
                    else if (enumDict.ContainsKey(typename))
                    {
                        defaultValue = "\"\"";
                        type = "value.toString()";
                    }
                    break;




            }
            values[0] = defaultValue;
            values[1] = type;
            return values;
        }
        private void printFields(List<XmlSchemaElement> list,StreamWriter writer)
        {
            foreach (XmlSchemaElement childElement in list)
            {
                string type = childElement.SchemaTypeName.Name;
                bool isEnum = enumDict.ContainsKey(type);
                if (Equals(type, "string")) type = "String";
                else if (Equals(type, "double") || Equals(type, "float") || Equals(type, "decimal")) type = "BigDecimal";
                else if (Equals(type, "dateTime")) type = "Date";
                else if (isEnum) type = "String";

                if (!isEnum) writer.WriteLine("      public {0} {1} ;", type, childElement.Name);
                else writer.WriteLine("      public {0} {1} ;  // Enum {2} ", type, childElement.Name, childElement.SchemaTypeName.Name);

            }
        }

        private void printGetPropertyFunc(List<XmlSchemaElement> fields,StreamWriter writer )
        {
                    string head = @" 
public Object getProperty(int index)
{
    switch (index)
      {";
           
                    writer.WriteLine(head);
                    for(int i = 0; i < fields.Count; i++) {
                        writer.WriteLine("          case {0} : return {1};",i,fields[i].Name);

                    }


                    writer.WriteLine("      } \n    return null;\n} ");
                

            



         }

        private void printEnum() {


            foreach (KeyValuePair<string, List<string>> entry in enumDict)
            {
                string fileName = "C:\\Users\\muhammet.kaya\\AndroidStudioProjects\\MyProject\\app\\src\\main\\java\\com\\example\\myproject\\models\\" + entry.Key + ".java";
                try
                {

                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.WriteLine("package {0} ;", packageName);
                        //writer.WriteLine("package " + Form1.packageName + ";");
                        writer.WriteLine("\n public enum "+entry.Key + " {\n");
                        foreach (string s in entry.Value)
                        {
                            writer.WriteLine("  "+s+",");

                           
                        }
                        writer.WriteLine("\n}");

                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
              }
        }

        private void WriteImports(StreamWriter writer)
        {
           
               
                writer.WriteLine(

               @"
import java.util.Date;
import android.util.Base64;
import java.util.Hashtable;
import java.math.BigDecimal;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.PropertyInfo;
import org.ksoap2.serialization.SoapPrimitive;
import org.ksoap2.serialization.SoapSerializationEnvelope;

                "
                   );
                   

            

         }
            
        private void copyDefaultClasses(string destinationPath)
        {
            string [] filenames=Assembly.GetExecutingAssembly().GetManifestResourceNames();

            foreach (string file in filenames)
            {

                if (file.StartsWith("UyumsoftAndroidTool.DefaultClasses"))
                {

                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file);
                    StreamReader reader = new StreamReader(stream);
                    string package = "package " + packageName + " ;\n";
                    string data =package+ reader.ReadToEnd();
                    string[] list = file.Split(new string[] { "DefaultClasses." },StringSplitOptions.None);

                    string path = destinationPath +"/"+list[1];
                    using(StreamWriter writer = new StreamWriter(path))
                    {
                        writer.WriteLine(data);
                    }
                   /*
                    string dirPath = @"./DefaultClasses";
                    DirectoryInfo d = new DirectoryInfo(dirPath);
                    FileInfo[] Files = d.GetFiles("*.java");
                   */

                }
            }

        }


    //-------------------------------------------------------------------------------------------------------------
    private static List<XmlSchemaElement> OutputElements(XmlSchemaParticle particle)
        {
            List<XmlSchemaElement> list = new List<XmlSchemaElement>();
                   
                XmlSchemaSequence sequence = particle as XmlSchemaSequence;
                XmlSchemaChoice choice = particle as XmlSchemaChoice;
                XmlSchemaAll all = particle as XmlSchemaAll;

                if (sequence != null)
                {
                    Console.Out.WriteLine("  Sequence");

                    for (int i = 0; i < sequence.Items.Count; i++)
                    {
                        XmlSchemaElement childElement = sequence.Items[i] as XmlSchemaElement;
                        XmlSchemaSequence innerSequence = sequence.Items[i] as XmlSchemaSequence;
                        XmlSchemaChoice innerChoice = sequence.Items[i] as XmlSchemaChoice;
                        XmlSchemaAll innerAll = sequence.Items[i] as XmlSchemaAll;

                        if (childElement != null)
                        {
                            Console.Out.WriteLine("    Element/Type: {0}:{1}", childElement.Name,
                                                  childElement.SchemaTypeName.Name);
                           list.Add(childElement);
                           // writer.WriteLine("public {0} {1} ;", childElement.SchemaTypeName.Name, childElement.Name);
                        }
                        else OutputElements(sequence.Items[i] as XmlSchemaParticle);
                    }
                }
                else if (choice != null)
                {
                    Console.Out.WriteLine("  Choice");
                    for (int i = 0; i < choice.Items.Count; i++)
                    {
                        XmlSchemaElement childElement = choice.Items[i] as XmlSchemaElement;
                        XmlSchemaSequence innerSequence = choice.Items[i] as XmlSchemaSequence;
                        XmlSchemaChoice innerChoice = choice.Items[i] as XmlSchemaChoice;
                        XmlSchemaAll innerAll = choice.Items[i] as XmlSchemaAll;

                        if (childElement != null)
                        {
                        list.Add(childElement);
                        Console.Out.WriteLine("    Element/Type: {0}:{1}", childElement.Name,
                                                  childElement.SchemaTypeName.Name);
                        }
                        else OutputElements(choice.Items[i] as XmlSchemaParticle);
                    }

                    Console.Out.WriteLine();
                }
                else if (all != null)
                {
                    Console.Out.WriteLine("  All");
                    for (int i = 0; i < all.Items.Count; i++)
                    {
                        XmlSchemaElement childElement = all.Items[i] as XmlSchemaElement;
                        XmlSchemaSequence innerSequence = all.Items[i] as XmlSchemaSequence;
                        XmlSchemaChoice innerChoice = all.Items[i] as XmlSchemaChoice;
                        XmlSchemaAll innerAll = all.Items[i] as XmlSchemaAll;

                        if (childElement != null)
                        {
                        list.Add(childElement);
                        Console.Out.WriteLine("    Element/Type: {0}:{1}", childElement.Name,
                                                  childElement.SchemaTypeName.Name);
                        }
                        else OutputElements(all.Items[i] as XmlSchemaParticle);
                    }
                    Console.Out.WriteLine();
                }
            return list;
            
        }
    }
}

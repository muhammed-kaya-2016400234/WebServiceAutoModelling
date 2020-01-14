using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;
using System.Xml.Schema;

namespace UyumsoftAndroidTool
{
    public partial class Form1 : Form
    {

        public static string packageName= "com.example.myproject.models";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ServiceTest();
            WriteToFile();
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

        public static void WriteToFile()
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

            Dictionary<string, List<string>> enumDict = new Dictionary<string, List<string>>();
            Dictionary<string, Dictionary<string, List<string>>> fileEnum = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (object item in xmlSchema.Items)
            {
                XmlSchemaComplexType complexType = item as XmlSchemaComplexType;
                XmlSchemaSimpleType simpleType = item as XmlSchemaSimpleType;
                string fileName = "";
                if (complexType != null)
                {
                    fileName = "C:\\Users\\muhammet.kaya\\AndroidStudioProjects\\MyProject\\app\\src\\main\\java\\com\\example\\myproject\\models\\" + complexType.Name + ".java";
                    try
                    {
                        WriteImports(fileName);
                        using (StreamWriter writer = new StreamWriter(fileName,true))
                        {
                            //writer.WriteLine("package " + Form1.packageName + ";");
                           
                            writer.WriteLine(@"public class {0} {{", complexType.Name);

                            Console.Out.WriteLine("Complex Type: {0}", complexType.Name);
                            List<XmlSchemaElement> list = OutputElements(complexType.Particle);

                            foreach (XmlSchemaElement childElement in list)
                            {
                               
                                string type = childElement.SchemaTypeName.Name;
                                if (Equals(type, "string")) type = "String";
                                else if (Equals(type, "double")|| Equals(type, "float")|| Equals(type, "decimal")) type = "BigDecimal";
                                
                                writer.WriteLine("public {0} {1} ;", type, childElement.Name);
                            }

                            writer.WriteLine("}");
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
                                enumDict.Add(simpleType.Name, new List<string>());
                            }
                            foreach (XmlSchemaFacet en in res.Facets)
                            {
                                if (en is XmlSchemaEnumerationFacet)
                                {
                                    enumDict[simpleType.Name].Add(en.Value);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            
                        }
                    }
                }
                Console.Out.WriteLine();
            }
            printEnum(enumDict);
            Console.Out.WriteLine();
            Console.In.ReadLine();
        }

        private static void printEnum(Dictionary<string, List<string>> a) {


            foreach (KeyValuePair<string, List<string>> entry in a)
            {
                string fileName = "C:\\Users\\muhammet.kaya\\AndroidStudioProjects\\MyProject\\app\\src\\main\\java\\com\\example\\myproject\\models\\" + entry.Key + ".java";
                try
                {

                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        //writer.WriteLine("package " + Form1.packageName + ";");
                        writer.WriteLine("public enum "+entry.Key + " {");
                        foreach (string s in entry.Value)
                        {
                            writer.WriteLine(s+",");

                           
                        }
                        writer.WriteLine("}");

                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
              }
        }

        private static void WriteImports(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("package " + Form1.packageName + ";");
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

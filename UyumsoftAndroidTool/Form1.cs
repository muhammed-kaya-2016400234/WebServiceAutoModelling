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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServiceTest();
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
            Types types = serviceDescription.Types;
            XmlSchema xmlSchema = types.Schemas[0];

            foreach (object item in xmlSchema.SchemaTypes)
            {
                XmlSchemaComplexType complexType = item as XmlSchemaComplexType;
                if (complexType!=null)
                {
                    using (StreamWriter writer = new StreamWriter("C:\\Users\\muhammet.kaya\\Desktop\\deneme model.txt"))
                    {
                        writer.WriteLine("public class {0} {", complexType.Name);



                        writer.WriteLine("}");
                    }

                }
                //Console.WriteLine(item.Name);

               

            }
        }



        //-------------------------------------------------------------------------------------------------------------
        private static void OutputElements(XmlSchemaParticle particle)
        {
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
                        Console.Out.WriteLine("    Element/Type: {0}:{1}", childElement.Name,
                                              childElement.SchemaTypeName.Name);
                    }
                    else OutputElements(all.Items[i] as XmlSchemaParticle);
                }
                Console.Out.WriteLine();
            }
        }
    }
}

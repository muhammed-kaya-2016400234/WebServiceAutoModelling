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
        public string Namespace = "www.tempuri.org";
        string schematargetnamespace = "";
        public bool isDotNet = true;
        public bool debug = true;
        public int timeout = 100000;
        Dictionary<string, List<string>> enumDict = new Dictionary<string, List<string>>();
        Dictionary<string, List<XmlSchemaElement>> complexTypes = new Dictionary<string, List<XmlSchemaElement>>();
        Dictionary<string, List<XmlSchemaElement>> arrayClasses = new Dictionary<string, List<XmlSchemaElement>>();
        Dictionary<string, List<XmlSchemaElement>> inputParamClasses = new Dictionary<string, List<XmlSchemaElement>>();
        Dictionary<string, List<XmlSchemaElement>> outputParamClasses = new Dictionary<string, List<XmlSchemaElement>>();

        public readonly string[] basicTypes = {"string","int","boolean","dateTime","float","double","decimal" };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            parseWsdl();
            printClasses(complexTypes,"complexType");
            printClasses(inputParamClasses,"inputParamClass");
            printClasses(outputParamClasses, "outputParamClass");
            printClasses(arrayClasses, "arrayClass");
            printEnum();
            printWebServiceClass();
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

    
        public void parseWsdl()
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
            schematargetnamespace = xmlSchema.TargetNamespace;

            
           

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


               // string fileName = "";
                if (complexType != null)
                {
                    //   fileName = destinationPath + complexType.Name + ".java";
                    if (complexType.Name.StartsWith("ArrayOf"))
                    {
                        string arraytype = complexType.Name.Substring("ArrayOf".Length);
                       
                        arrayClasses["ArrayOf"+getType(arraytype)]= OutputElements(complexType.Particle); ;
                        //arrayClasses[complexType.Name]=OutputElements
                    }
                    else
                    {
                        complexTypes[complexType.Name] = OutputElements(complexType.Particle); 
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
                //get input param class fields
                else if (element != null&&inputParamClasses.ContainsKey(element.Name))
                {
                    XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;
                    if (ct != null)
                    {
                       
                        inputParamClasses[element.Name] = OutputElements(ct.Particle);
                    }
                    
                    
                }
                //get output param class fields
                else if (element != null && outputParamClasses.ContainsKey(element.Name))
                {
                    XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;
                    if (ct != null)
                    {

                        outputParamClasses[element.Name] = OutputElements(ct.Particle);
                    }


                }
            }

          
        }

        /*
        public void printInputParamClasses()
        {
            foreach (KeyValuePair<string, List<XmlSchemaElement>> entry in inputParamClasses)
            {
                string fileName = destinationPath + entry.Key + ".java";
                try
                {
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.WriteLine("package " + packageName + ";\n");
                        printImports(writer);
                        writer.WriteLine("public class {0} {{\n", entry.Key);
                        writer.WriteLine(" private static final String METHOD_NAME = \""+entry.Key+"\";");
                        writer.WriteLine(" private static final String NAMESPACE = \"" +Namespace + "\";");
                        printFields(entry.Value, writer);
                        printInputParamClassAdditionalFuncs(entry.Value,writer);
                        printGetPropertyFunc(entry.Value, writer,"");
                        writer.WriteLine("\npublic int getPropertyCount() {{ return {0}; }}\n", entry.Value.Count);
                        printGetPropertyInfoFunc(entry.Value, writer,"");
                        printSetPropertyFunc(entry.Value, writer,"");

                        writer.WriteLine("\n}");
                    }

                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                }
        }
        */
        private void printInputParamClassAdditionalFuncs(List<XmlSchemaElement> list,StreamWriter writer)
        {
            writer.WriteLine(
    @"public SoapObject GetSoapParams()
	{
         SoapObject request = new SoapObject(NAMESPACE, METHOD_NAME);   ");



            for (int i= 0;i < list.Count;i++) {
                XmlSchemaElement element = list[i];
                writer.WriteLine("      PropertyInfo p{0} = new PropertyInfo();",i);
                writer.WriteLine("      p{0}.setName(\"{1}\");",i,element.Name);
                writer.WriteLine("      p{0}.setValue({1});", i, element.Name);
                writer.WriteLine("      p{0}.setType({1});", i, getClassOfField(element.SchemaTypeName.Name));
                writer.WriteLine("      p{0}.setNamespace(\"{1}\");", i, schematargetnamespace);
                writer.WriteLine("      request.addProperty(p{0});\n", i);

            }


            writer.WriteLine("      return request;");
            writer.WriteLine("}\n");
            writer.WriteLine("public String GetSoapAction() { return NAMESPACE + METHOD_NAME;}\n");
        }
        private void printClasses(Dictionary<string, List<XmlSchemaElement>> dict,string classType)
        {
            foreach (KeyValuePair<string, List<XmlSchemaElement>> entry in dict)
            {
                string fileName = destinationPath + entry.Key + ".java";
                try
                {
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.WriteLine("package " +packageName + ";\n");
                        printImports(writer);

                        if (classType != "arrayClass")
                        {
                            writer.WriteLine("public class {0} extends BaseObject {{\n", entry.Key);
                            printFields(entry.Value, writer);
                            printClassConstructor(entry.Key, writer, entry.Value);
                        }
                        
                        else writer.WriteLine("public class {0} extends Vector<{1}> implements KvmSerializable {{\n", entry.Key, getNonPrimitiveType(entry.Value[0].SchemaTypeName.Name));

                      


                        if (classType == "arrayClass") {
                            writer.WriteLine("private static final long serialVersionUID = 1L;");
                            printAdditionalFuncsForArrayClass(entry.Key,entry.Value[0],writer);
                        }
                        if (classType == "inputParamClass")
                        {
                            writer.WriteLine("      private static final String METHOD_NAME = \"" + entry.Key + "\";");
                            writer.WriteLine("      private static final String NAMESPACE = \"" + schematargetnamespace + "\";\n");
                            printInputParamClassAdditionalFuncs(entry.Value,writer);
                        }

                        printGetPropertyFunc(entry.Value, writer,classType);
                        if (classType != "arrayClass") writer.WriteLine("\npublic int getPropertyCount() {{ return {0}; }}\n", entry.Value.Count);
                        else writer.WriteLine("\npublic int getPropertyCount() { return Math.max(1,this.size());}\n");

                        printGetPropertyInfoFunc(entry.Value, writer,classType);
                        printSetPropertyFunc(entry.Value, writer,classType);

                        if (classType == "outputParamClass")
                        {
                            printLoadSoapObjectFuncForOutput(entry.Value[0],writer);
                        }else if (classType == "complexType")
                        {
                            printloadSoapObjectForComplexTypes(entry.Key,writer);
                        }

                        
                        writer.WriteLine("\n}");
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

       
        
        private void printLoadSoapObjectFuncForOutput(XmlSchemaElement element,StreamWriter writer)
        {
            string type = element.SchemaTypeName.Name;
            string setval = "";
           // if (basicTypes.Contains(element.SchemaTypeName.Name)) { 
          
                setval = castValueToType(type, "property",varName:element.Name);
                writer.WriteLine(@"
public void loadSoapObject(Object property){{
		if(property == null) return;
		{0}
       
	}}"
, setval
,element.Name
);
                /*
            }
            
            else
            {

                writer.WriteLine(@"
public void loadSoapObject(SoapObject property){{
		if(property == null) return;
        {0}
		
	}}"
, castValueToType(type, "property",element.Name)
   

    );
            }
            */

            }

        private string castValueToType(string type,string toBeCast, string varName="item",bool createNewObject=false)
        {
            if (enumDict.ContainsKey(type)) type = "string";
            string typeClass = createNewObject ? getType(type) : "";
            switch (type)
            {
                case "string":  return typeClass+" "+varName+"="+ toBeCast+".toString();";
                case "int": return typeClass + " " + varName + "=Integer.parseInt(" + toBeCast + ".toString());";
                case "double": return typeClass + " " + varName + "=new BigDecimal(" + toBeCast+".toString());";
                case "float": return typeClass + " " + varName + "=new BigDecimal(" + toBeCast+".toString());";
                case "decimal": return typeClass + " " + varName + "=new BigDecimal(" + toBeCast+".toString());";
                case "dateTime": return typeClass + " " + varName + "=DateUtil.getDate(" + toBeCast + ".toString());";
                case "boolean": return typeClass + " " + varName + "=Boolean.parseBoolean(" + toBeCast + ".toString());";
              
                default:
                    if(createNewObject)  return type+" "+varName+" = new "+type+"(); "+varName+".loadSoapObject("+toBeCast+");";
                    else return " " + varName + " = new " + type + "(); " + varName + ".loadSoapObject(" + toBeCast + ");";
            }

        }

        private void printLoadSoapObjectFuncForArray(string arrayType,string type,StreamWriter writer)
        {


            writer.WriteLine(
@"public void loadSoapObject(Object obj){{
		if(obj == null) return;
        {0} property = ({0}) obj;
		int itemCount = property.getPropertyCount();
		if(itemCount > 0){{
			for(int loop=0;loop < itemCount;loop++){{
				",arrayType);

            if (basicTypes.Contains(type)||enumDict.ContainsKey(type))
            {
                writer.WriteLine("				" + castValueToType(type,"property.getProperty(loop)",createNewObject:true)+";");
            }
            /*
            if (type == "String")
            {
                writer.WriteLine(@"        
                String item = pii.getProperty(0).toString();");
            }
            else if (type == "int")
            {
                writer.WriteLine(@"        
                int item=Integer.parseInt(pii.getProperty(0).toString());");
            }
            else if (getType(type) == "BigDecimal")
            {
                writer.WriteLine(@"        
                BigDecimal item=new BigDecimal(pii.getProperty(0).toString());");
            }else if (type == "Date")
            {
                writer.WriteLine(@"        
                Date item=DateUtil.getDate(pii.getProperty(0).toString());;");
            }*/
            else
            {
                writer.WriteLine(@"
                
                {0} item = ({0})property.getProperty(loop);
				", type);
            }

            writer.WriteLine(@"
                this.add(item);
        	}
        }
	}  ");
            
        }
        private void printSetPropertyFunc(List<XmlSchemaElement> fields, StreamWriter writer,string classType)
        {

            if (classType == "outputParamClass"&&fields.Count>0)
            {
                writer.WriteLine(@"
public void setProperty(int index, Object value)
{{
    {0}=({1}) value;
}}"
,fields[0].Name
,getType(fields[0].SchemaTypeName.Name)
);
            }else if (classType == "arrayClass")
            {
                writer.WriteLine("public void setProperty(int arg0, Object arg1) {{this.add(({0})arg1);}}",getType(fields[0].SchemaTypeName.Name));
            }
            else
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
                    if (basicTypes.Contains(field.SchemaTypeName.Name))
                    {
                        //string type = getClassOfField(field.SchemaTypeName.Name);
                        string[] values = getValueOfField(field.SchemaTypeName.Name); //values[0] = defaultvalue  //values [1] = value
                        writer.WriteLine(@"          
            case {0} :
               if(value.toString().equalsIgnoreCase(""anyType{{}}""))
                    {1} = {2};
               else
                    {1} = {3};
               
                break;"

                        , i, field.Name, values[0], values[1]);

                    }
                    else if (enumDict.ContainsKey(field.SchemaTypeName.Name))
                    {
                        //string type = getClassOfField(field.SchemaTypeName.Name);
                        string[] values = getValueOfField(field.SchemaTypeName.Name); //values[0] = defaultvalue  //values [1] = value
                        writer.WriteLine(@"          
            case {0} :
               if(value.toString().equalsIgnoreCase(""anyType{{}}""))
                    {1} = {2};
               else
                    {1} = {3};
               
                break;"

                        , i, field.Name, "\"\"", "value.toString()");

                    }
                    else if (complexTypes.ContainsKey(field.SchemaTypeName.Name))
                    {
                        writer.WriteLine(@"
            case {0}:
                 if(value != null){{ 
                      {2} pi = ({2})value; 
                      int itemCount = pi.getPropertyCount(); 
                      if(itemCount > 0){{ 
                          {1}
                         }} 
              }} 
                  break; 
                ",i,castValueToType(field.SchemaTypeName.Name,"pi",varName:field.Name), field.SchemaTypeName.Name);

                    }
                    else if(arrayClasses.ContainsKey(field.SchemaTypeName.Name))
                    {   
                        writer.WriteLine(@"
            case {0} :
                if(value != null){{ 
                      {2} = new {1}(); 
                     {1} prp = ({1})value; 
                      {2}.loadSoapObject(prp);
                      }}
                      break; ",i,field.SchemaTypeName.Name,field.Name,castValueToType(arrayClasses[field.SchemaTypeName.Name][0].SchemaTypeName.Name,"pi",createNewObject:true));
                    }
                    writer.WriteLine();
                }
                //writer.WriteLine("            default : break;");

                writer.WriteLine("      }\n} ");
            }
        }

        private void printGetPropertyInfoFunc(List<XmlSchemaElement> fields, StreamWriter writer,string classType)
        {

            if (classType == "outputParamClass" && fields.Count > 0)
            {
                writer.WriteLine(@"@SuppressWarnings(""unchecked"")
    public void getPropertyInfo(int index, Hashtable properties, PropertyInfo info)
    {{
        info.name = ""{0}"";
        info.type={1};

}}"
,fields[0].Name
,getClassOfField(fields[0].SchemaTypeName.Name));
            }else if (classType == "arrayClass")
            {
                writer.WriteLine(@"@SuppressWarnings(""unchecked"")
    public void getPropertyInfo(int index, Hashtable properties, PropertyInfo info)
    {{
        info.name = ""{0}"";
        info.type={1};
        info.namespace=""{2}"";

}}"
, fields[0].Name
, getClassOfField(fields[0].SchemaTypeName.Name)
,schematargetnamespace);

            }
            else
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
                    if (complexTypes.ContainsKey(typename)||arrayClasses.ContainsKey(typename)||inputParamClasses.ContainsKey(typename)||outputParamClasses.ContainsKey(typename))
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
                    if (complexTypes.ContainsKey(typename) || arrayClasses.ContainsKey(typename) || inputParamClasses.ContainsKey(typename) || outputParamClasses.ContainsKey(typename))
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
                type = getType(type);

                if (!isEnum) writer.WriteLine("      public {0} {1} ;", type, childElement.Name);
                else writer.WriteLine("      public {0} {1} ;  // Enum {2} ", type, childElement.Name, childElement.SchemaTypeName.Name);

            }
        }
        private string getType(string type)
        {
            bool isEnum = enumDict.ContainsKey(type);

            if (Equals(type, "string")) type = "String";
            else if (Equals(type, "double") || Equals(type, "float") || Equals(type, "decimal")) type = "BigDecimal";
            else if (Equals(type, "dateTime")) type = "Date";
            else if (isEnum) type = "String";
            return type;
        }

        private void printGetPropertyFunc(List<XmlSchemaElement> fields,StreamWriter writer,string classType )
        {
            if (classType == "outputParamClass"&&fields.Count>0)
            {
                writer.WriteLine(@"
public Object getProperty(int index)
    {{

    	return {0};

    }}",fields[0].Name);

            }else if (classType=="arrayClass")
            {
                writer.WriteLine("public Object getProperty(int arg0) {return this.get(arg0);}");
            }
            else
            {
                string head = @" 
public Object getProperty(int index)
{
    switch (index)
      {";

                writer.WriteLine(head);
                for (int i = 0; i < fields.Count; i++)
                {
                    writer.WriteLine("          case {0} : return {1};", i, fields[i].Name);

                }


                writer.WriteLine("      } \n    return null;\n} ");


            }



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

        private void printImports(StreamWriter writer)
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
import java.util.Vector;
import org.ksoap2.serialization.KvmSerializable;

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

        private void printAdditionalFuncsForArrayClass(string arrayType,XmlSchemaElement element, StreamWriter writer)
        {
            writer.WriteLine(@" 
protected String getItemDescriptor() {{return ""{0}""; }}

protected Class getElementClass() {{ return {1}; }}
"
,element.SchemaTypeName.Name
,getClassOfField(element.SchemaTypeName.Name)
);

            printLoadSoapObjectFuncForArray(arrayType,element.SchemaTypeName.Name,writer);

            if (enumDict.ContainsKey(element.SchemaTypeName.Name))
            {
                writer.WriteLine(@"
public void add({0} item){{
    this.add(item.toString());
}}",element.SchemaTypeName.Name);
            }
        }
        
        private string getNonPrimitiveType(string typename)
        {
            switch (typename)
            {
                case "int":
                    return "Integer";
                default:
                    return getType(typename);
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
                  

                    for (int i = 0; i < sequence.Items.Count; i++)
                    {
                        XmlSchemaElement childElement = sequence.Items[i] as XmlSchemaElement;
                        XmlSchemaSequence innerSequence = sequence.Items[i] as XmlSchemaSequence;
                        XmlSchemaChoice innerChoice = sequence.Items[i] as XmlSchemaChoice;
                        XmlSchemaAll innerAll = sequence.Items[i] as XmlSchemaAll;

                        if (childElement != null)
                        {
                          
                           list.Add(childElement);
                           // writer.WriteLine("public {0} {1} ;", childElement.SchemaTypeName.Name, childElement.Name);
                        }
                        else OutputElements(sequence.Items[i] as XmlSchemaParticle);
                    }
                }
                else if (choice != null)
                {
                  
                    for (int i = 0; i < choice.Items.Count; i++)
                    {
                        XmlSchemaElement childElement = choice.Items[i] as XmlSchemaElement;
                        XmlSchemaSequence innerSequence = choice.Items[i] as XmlSchemaSequence;
                        XmlSchemaChoice innerChoice = choice.Items[i] as XmlSchemaChoice;
                        XmlSchemaAll innerAll = choice.Items[i] as XmlSchemaAll;

                        if (childElement != null)
                        {
                        list.Add(childElement);
                       
                        }
                        else OutputElements(choice.Items[i] as XmlSchemaParticle);
                    }

                  
                }
                else if (all != null)
                {
                   
                    for (int i = 0; i < all.Items.Count; i++)
                    {
                        XmlSchemaElement childElement = all.Items[i] as XmlSchemaElement;
                        XmlSchemaSequence innerSequence = all.Items[i] as XmlSchemaSequence;
                        XmlSchemaChoice innerChoice = all.Items[i] as XmlSchemaChoice;
                        XmlSchemaAll innerAll = all.Items[i] as XmlSchemaAll;

                        if (childElement != null)
                        {
                        list.Add(childElement);
                        
                        }
                        else OutputElements(all.Items[i] as XmlSchemaParticle);
                    }
                    
                }
            return list;
            
        }

        private HashSet<string> getMappings(string className)
        {
            
                HashSet<string> list = new HashSet<string>();
            
                list.Add(className);
                List<XmlSchemaElement> classList = new List<XmlSchemaElement>();
                if(complexTypes.ContainsKey(className))
                    classList.AddRange(complexTypes[className]);
                if (arrayClasses.ContainsKey(className))
                    classList.AddRange(arrayClasses[className]);
                if (inputParamClasses.ContainsKey(className))
                    classList.AddRange(inputParamClasses[className]);
                if (outputParamClasses.ContainsKey(className))
                    classList.AddRange(outputParamClasses[className]);
                foreach (XmlSchemaElement elem in classList)
                {
                    if (!basicTypes.Contains(elem.SchemaTypeName.Name)&&!enumDict.ContainsKey(elem.SchemaTypeName.Name))
                    {
                        list.UnionWith(getMappings(elem.SchemaTypeName.Name));
                    }

                }
            


            return list;
                
        }
        
        private void printWebServiceClass()
        {
            using(StreamWriter writer=new StreamWriter(destinationPath + "/WebService.java"))
            {
                writer.WriteLine(@"
package {0};

import java.util.Date;
import android.util.Log;
import java.util.Hashtable;
import java.math.BigDecimal;

import java.io.IOException;
import org.ksoap2.SoapFault;
import org.ksoap2.SoapEnvelope;
import java.net.SocketTimeoutException;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.transport.HttpTransportSE;
import org.ksoap2.serialization.SoapSerializationEnvelope;

	public final class WebService
	{{

		private Boolean debug = {1};
		private String requestDump = """";
		private String responseDump = """";
		private String faultstring = """";

		public String Address;
		public boolean IsDotNet = {2};
		public int TimeOut = {3};
		protected static final String NAMESPACE = ""{4}"";
        
        ",packageName,debug.ToString().ToLower(),isDotNet.ToString().ToLower(), timeout,schematargetnamespace);

                foreach (string elem in inputParamClasses.Keys)
                {


                    writer.WriteLine(@"
    public {0}Response {0}({0} params) throws Exception
		    {{
            SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);
			envelope.dotNet = IsDotNet;
			envelope.setOutputSoapObject(params.GetSoapParams());
", elem);


                  

                    HashSet<string> mappings = new HashSet<string>();
                    mappings.UnionWith(getMappings(elem));
                    mappings.UnionWith(getMappings(elem+"Response"));
                    foreach (string map in mappings)
                    {
                        writer.WriteLine(@"             envelope.addMapping(NAMESPACE, ""{0}"", {0}.class);",map);
                    }
                    writer.WriteLine(@"
            new MarshalDecimal().register(envelope);
			new MarshalDouble().register(envelope);
			new MarshalDate().register(envelope);
			new MarshalFloat().register(envelope);
            
            HttpTransportSE androidHttpTransport = new HttpTransportSE(Address, TimeOut);
			androidHttpTransport.debug = debug;
			
			try{{
				androidHttpTransport.call(params.GetSoapAction(), envelope);
			}}
			catch (Exception s) {{
				faultstring = s.getMessage();
				s.printStackTrace();
				Log.e(getClass().getSimpleName() + "" /{0}"", faultstring);

                return null;
                }}

            if(debug == true){{
				requestDump = androidHttpTransport.requestDump;
				responseDump = androidHttpTransport.responseDump;
			}}
	        
			{0}Response resp = null;
			SoapFault fault = getFault(envelope);
			if(fault == null){{
        		
        		//SoapObject response = (SoapObject)envelope.getResponse();
				Object obj=envelope.getResponse();
        		if(obj != null){{
        			resp = new {0}Response();
        			resp.loadSoapObject(obj);
        		}}          		
			}}
			else{{
				Log.i(getClass().getSimpleName(), fault.faultstring);
        		throw new Exception(fault.faultstring);
			}}

			return resp;

                ", elem);

            
                    writer.WriteLine(@"
            }");
                }

                writer.WriteLine(@"
    public String getRequestDump(){
		return requestDump;
	}
	
	public String getResponseDump(){
		return responseDump;
	}
	
	public String getFaultString(){
		return faultstring;
	}

	public void setDebug(Boolean isdebug){
		debug = isdebug;
	}		

	private SoapFault getFault(SoapSerializationEnvelope envelope){
		SoapFault fault = null;
		faultstring = """";
		try{
			fault = SoapFault.class.cast(envelope.bodyIn);
			Log.e(getClass().getSimpleName(), fault.faultstring);
			faultstring = fault.faultstring;
		}
		catch ( final ClassCastException ex ){
			ex.printStackTrace();
		}
		catch (Exception e) {
			e.printStackTrace();
		}
		return fault;
	}    
");



                writer.WriteLine("}");
            }
        }

        private void printClassConstructor(string className, StreamWriter writer, List<XmlSchemaElement> classParams)
        {
            
            string parameters = "";
            string assignments = "";
            
            foreach (XmlSchemaElement elem in classParams)
                {
                if (enumDict.ContainsKey(elem.SchemaTypeName.Name))
                {
                    parameters += elem.SchemaTypeName.Name + " " + elem.Name + ",";
                    assignments += "this." + elem.Name + "=" + elem.Name + ".toString();\n";
                }
                else
                {
                    parameters += getType(elem.SchemaTypeName.Name) + " " + elem.Name + ",";
                    assignments += "this." + elem.Name + "=" + elem.Name + ";\n";
                }
                
            }
            if(parameters.Length>0)
                parameters = parameters.Substring(0,parameters.Length-1);

            if (classParams.Count > 0)
            {
                writer.WriteLine(@"

public {0}(){{super();}}

public {0}({1}){{
{2}
}}
", className, parameters, assignments);

            }
 
        }

        private void printloadSoapObjectForComplexTypes(string complexType,StreamWriter writer)
        {
            writer.WriteLine(@"
public void loadSoapObject(Object obj){{
		if(obj == null) return;
        {0} property= ({0}) obj;
		int pr = getPropertyCount();
		PropertyInfo pro = new PropertyInfo();
		for(int i=0;i<pr;i++){{
			
			setProperty(i, property.getProperty(i));
		}}
	}} 
",complexType);
            
        }
    }
}

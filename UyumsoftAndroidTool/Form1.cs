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
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string webservice = this.webServiceText.Text;
            string package = this.packagenameText.Text;
            string destpath = this.destpathText.Text;
            string namespacetext = this.namespaceText.Text;
            WsdlParser parser = new WsdlParser(webservice,package,destpath,namespacetext);
            parser.execute();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

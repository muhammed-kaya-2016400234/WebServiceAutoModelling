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
        WsdlParser parser;
        bool allSelected = true;
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
            parser = new WsdlParser(webservice,package,destpath,namespacetext);
            List<string> opList=parser.parse();
            //this.checkedListBox1.Items.Add("ALL",true);
            foreach(string s in opList)
            {
                this.checkedListBox1.Items.Add(s,true);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> wantedOperations = this.checkedListBox1.CheckedItems.Cast<string>().ToList();
            parser.execute();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            allSelected = !allSelected;
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                
                this.checkedListBox1.SetItemChecked(i, allSelected);
            }
        }
    }
}

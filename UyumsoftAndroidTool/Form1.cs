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
            this.checkBox1.Checked = true;
            this.dataGridView1.Rows.Clear();
            //this.checkedListBox1.Items.Clear();
            string webservice = this.webServiceText.Text;
            string package = this.packagenameText.Text;
            string destpath = this.destpathText.Text;
            string namespacetext = this.namespaceText.Text;
            parser = new WsdlParser(webservice,package,destpath,namespacetext);
            Dictionary<string, List<XmlSchemaElement>> opList =parser.parse().Item1;
            Dictionary<string, List<XmlSchemaElement>> outputList =parser.parse().Item2;
            //this.checkedListBox1.Items.Add("ALL",true);
            foreach(string s in opList.Keys)
            {
                string parameters = " ";
                foreach(XmlSchemaElement elem in opList[s])
                {
                    parameters += parser.getListDef(elem)+"  "+elem.Name + ",";
                }
                parameters=parameters.Substring(0, parameters.Length - 1);
                //this.checkedListBox1.Items.Add(s+"("+parameters+")           return:"+parser.getListDef(outputList[s+"Response"][0]),true);
                this.dataGridView1.Rows.Add(true,s,parameters, parser.getListDef(outputList[s + "Response"][0]));
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
            List<string> wantedOperations = new List<string>();
            foreach (DataGridViewRow r in this.dataGridView1.Rows)
            {
                if(Convert.ToBoolean(r.Cells["Add"].Value)==true)
                {
                    string op = r.Cells["Method"].Value.ToString();
                    wantedOperations.Add(r.Cells["Method"].Value.ToString());
                } 
            }
          //  List<string> wantedOperations = this.checkedListBox1.CheckedItems.Cast<string>().ToList();
            parser.execute(wantedOperations);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            allSelected = !allSelected;
            foreach(DataGridViewRow r in this.dataGridView1.Rows)
            {
                r.Cells["Add"].Value = allSelected;
            }

            /*
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                
                this.checkedListBox1.SetItemChecked(i, allSelected);
            }
            */
        }
    }
}

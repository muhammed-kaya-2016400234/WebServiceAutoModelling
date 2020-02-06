namespace UyumsoftAndroidTool
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.webServiceText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.namespaceText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.destpathText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.packagenameText = new System.Windows.Forms.TextBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(624, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 39);
            this.button1.TabIndex = 0;
            this.button1.Text = "Parse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // webServiceText
            // 
            this.webServiceText.Location = new System.Drawing.Point(38, 45);
            this.webServiceText.Name = "webServiceText";
            this.webServiceText.Size = new System.Drawing.Size(508, 20);
            this.webServiceText.TabIndex = 1;
            this.webServiceText.Text = "localhost/WebService1.asmx";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Web Service URL";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Namespace";
            // 
            // namespaceText
            // 
            this.namespaceText.Location = new System.Drawing.Point(38, 87);
            this.namespaceText.Name = "namespaceText";
            this.namespaceText.Size = new System.Drawing.Size(508, 20);
            this.namespaceText.TabIndex = 3;
            this.namespaceText.Text = "www.tempuri.org";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Destination Path";
            // 
            // destpathText
            // 
            this.destpathText.Location = new System.Drawing.Point(38, 129);
            this.destpathText.Name = "destpathText";
            this.destpathText.Size = new System.Drawing.Size(508, 20);
            this.destpathText.TabIndex = 5;
            this.destpathText.Text = "C:\\\\Users\\\\muhammet.kaya\\\\AndroidStudioProjects\\\\MyProject\\\\app\\\\src\\\\main\\\\java\\" +
    "\\com\\\\example\\\\myproject\\\\models\\\\";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Package Name";
            // 
            // packagenameText
            // 
            this.packagenameText.Location = new System.Drawing.Point(38, 171);
            this.packagenameText.Name = "packagenameText";
            this.packagenameText.Size = new System.Drawing.Size(508, 20);
            this.packagenameText.TabIndex = 7;
            this.packagenameText.Text = "com.example.myproject.models";
            this.packagenameText.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(38, 231);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(438, 199);
            this.checkedListBox1.TabIndex = 9;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(624, 77);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(164, 39);
            this.button2.TabIndex = 10;
            this.button2.Text = "Generate Classes";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(38, 208);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(45, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "ALL";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.packagenameText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.destpathText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.namespaceText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.webServiceText);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox webServiceText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox namespaceText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox destpathText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox packagenameText;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}


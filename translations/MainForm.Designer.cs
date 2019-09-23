namespace translations
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Korean = new System.Windows.Forms.ListBox();
            this.English = new System.Windows.Forms.TextBox();
            this.SaveLine = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtreturnList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Korean
            // 
            this.Korean.FormattingEnabled = true;
            this.Korean.HorizontalScrollbar = true;
            this.Korean.Location = new System.Drawing.Point(12, 12);
            this.Korean.Name = "Korean";
            this.Korean.Size = new System.Drawing.Size(402, 212);
            this.Korean.TabIndex = 0;
            this.Korean.SelectedIndexChanged += new System.EventHandler(this.Korean_SelectedIndexChanged);
            // 
            // English
            // 
            this.English.Location = new System.Drawing.Point(420, 12);
            this.English.Multiline = true;
            this.English.Name = "English";
            this.English.Size = new System.Drawing.Size(402, 212);
            this.English.TabIndex = 1;
            // 
            // SaveLine
            // 
            this.SaveLine.BackColor = System.Drawing.Color.White;
            this.SaveLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveLine.ForeColor = System.Drawing.Color.Black;
            this.SaveLine.Location = new System.Drawing.Point(716, 256);
            this.SaveLine.Name = "SaveLine";
            this.SaveLine.Size = new System.Drawing.Size(106, 33);
            this.SaveLine.TabIndex = 2;
            this.SaveLine.Text = "Save Translation";
            this.SaveLine.UseVisualStyleBackColor = false;
            this.SaveLine.Click += new System.EventHandler(this.SaveLine_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(99, 296);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(723, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 299);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Search:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 325);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Search Eng:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(98, 322);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(723, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // txtreturnList
            // 
            this.txtreturnList.FormattingEnabled = true;
            this.txtreturnList.HorizontalScrollbar = true;
            this.txtreturnList.Location = new System.Drawing.Point(703, 348);
            this.txtreturnList.Name = "txtreturnList";
            this.txtreturnList.Size = new System.Drawing.Size(118, 147);
            this.txtreturnList.TabIndex = 7;
            this.txtreturnList.SelectedIndexChanged += new System.EventHandler(this.TxtreturnList_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(514, 407);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "If Multiple Words Match, Try Another:";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(32, 419);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 33);
            this.button1.TabIndex = 9;
            this.button1.Text = "Input XMLs";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(166, 429);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Search:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 495);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtreturnList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.SaveLine);
            this.Controls.Add(this.English);
            this.Controls.Add(this.Korean);
            this.Name = "MainForm";
            this.Text = "Translation Editor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Korean;
        private System.Windows.Forms.TextBox English;
        private System.Windows.Forms.Button SaveLine;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox txtreturnList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label label4;
    }
}


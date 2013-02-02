namespace CreateDataSample
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lbListGroup = new System.Windows.Forms.Label();
            this.btnLoadListGroup = new System.Windows.Forms.Button();
            this.btnBrowseListGroup = new System.Windows.Forms.Button();
            this.txtPathListGroup = new System.Windows.Forms.TextBox();
            this.btnWriteFromRSS = new System.Windows.Forms.Button();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dGVListGroupRSS = new System.Windows.Forms.DataGridView();
            this.lbListGroupNameAndLinkRSS = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.lbContent = new System.Windows.Forms.Label();
            this.btnReadContentFromRSS = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnWriteTwoListUsertoXML = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGVListGroupRSS)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(227, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Create 10.000 Users";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(384, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(136, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Create 20.000 Groups";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(547, 376);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnTest);
            this.tabPage3.Controls.Add(this.lbListGroup);
            this.tabPage3.Controls.Add(this.btnLoadListGroup);
            this.tabPage3.Controls.Add(this.btnBrowseListGroup);
            this.tabPage3.Controls.Add(this.txtPathListGroup);
            this.tabPage3.Controls.Add(this.btnWriteFromRSS);
            this.tabPage3.Controls.Add(this.btnLoadData);
            this.tabPage3.Controls.Add(this.btnBrowse);
            this.tabPage3.Controls.Add(this.txtPath);
            this.tabPage3.Controls.Add(this.btnRefresh);
            this.tabPage3.Controls.Add(this.dGVListGroupRSS);
            this.tabPage3.Controls.Add(this.lbListGroupNameAndLinkRSS);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(539, 350);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lbListGroup
            // 
            this.lbListGroup.AutoSize = true;
            this.lbListGroup.Location = new System.Drawing.Point(12, 308);
            this.lbListGroup.Name = "lbListGroup";
            this.lbListGroup.Size = new System.Drawing.Size(111, 13);
            this.lbListGroup.TabIndex = 29;
            this.lbListGroup.Text = "Tạo danh sách Group";
            // 
            // btnLoadListGroup
            // 
            this.btnLoadListGroup.Location = new System.Drawing.Point(424, 322);
            this.btnLoadListGroup.Name = "btnLoadListGroup";
            this.btnLoadListGroup.Size = new System.Drawing.Size(97, 23);
            this.btnLoadListGroup.TabIndex = 28;
            this.btnLoadListGroup.Text = "Load List Group";
            this.btnLoadListGroup.UseVisualStyleBackColor = true;
            this.btnLoadListGroup.Click += new System.EventHandler(this.btnLoadListGroup_Click);
            // 
            // btnBrowseListGroup
            // 
            this.btnBrowseListGroup.Location = new System.Drawing.Point(334, 322);
            this.btnBrowseListGroup.Name = "btnBrowseListGroup";
            this.btnBrowseListGroup.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseListGroup.TabIndex = 27;
            this.btnBrowseListGroup.Text = "Browse...";
            this.btnBrowseListGroup.UseVisualStyleBackColor = true;
            this.btnBrowseListGroup.Click += new System.EventHandler(this.btnBrowseListGroup_Click);
            // 
            // txtPathListGroup
            // 
            this.txtPathListGroup.Location = new System.Drawing.Point(15, 324);
            this.txtPathListGroup.Name = "txtPathListGroup";
            this.txtPathListGroup.Size = new System.Drawing.Size(302, 20);
            this.txtPathListGroup.TabIndex = 26;
            // 
            // btnWriteFromRSS
            // 
            this.btnWriteFromRSS.Location = new System.Drawing.Point(307, 9);
            this.btnWriteFromRSS.Name = "btnWriteFromRSS";
            this.btnWriteFromRSS.Size = new System.Drawing.Size(165, 23);
            this.btnWriteFromRSS.TabIndex = 25;
            this.btnWriteFromRSS.Text = "Ghi thông tin đọc được từ RSS ";
            this.btnWriteFromRSS.UseVisualStyleBackColor = true;
            this.btnWriteFromRSS.Click += new System.EventHandler(this.btnWriteFromRSS_Click);
            // 
            // btnLoadData
            // 
            this.btnLoadData.Location = new System.Drawing.Point(423, 264);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(97, 23);
            this.btnLoadData.TabIndex = 24;
            this.btnLoadData.Text = "Load Data";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(333, 264);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 23;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(14, 266);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(302, 20);
            this.txtPath.TabIndex = 22;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(200, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "Refresh ";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.button4_Click);
            // 
            // dGVListGroupRSS
            // 
            this.dGVListGroupRSS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVListGroupRSS.Location = new System.Drawing.Point(14, 38);
            this.dGVListGroupRSS.Name = "dGVListGroupRSS";
            this.dGVListGroupRSS.Size = new System.Drawing.Size(506, 220);
            this.dGVListGroupRSS.TabIndex = 6;
            // 
            // lbListGroupNameAndLinkRSS
            // 
            this.lbListGroupNameAndLinkRSS.AutoSize = true;
            this.lbListGroupNameAndLinkRSS.Location = new System.Drawing.Point(12, 14);
            this.lbListGroupNameAndLinkRSS.Name = "lbListGroupNameAndLinkRSS";
            this.lbListGroupNameAndLinkRSS.Size = new System.Drawing.Size(155, 13);
            this.lbListGroupNameAndLinkRSS.TabIndex = 5;
            this.lbListGroupNameAndLinkRSS.Text = "List Group Name and Link RSS";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Controls.Add(this.lbContent);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.btnReadContentFromRSS);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(539, 350);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(16, 84);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(491, 250);
            this.richTextBox1.TabIndex = 7;
            this.richTextBox1.Text = "";
            // 
            // lbContent
            // 
            this.lbContent.AutoSize = true;
            this.lbContent.Location = new System.Drawing.Point(13, 56);
            this.lbContent.Name = "lbContent";
            this.lbContent.Size = new System.Drawing.Size(87, 13);
            this.lbContent.TabIndex = 6;
            this.lbContent.Text = "Hiện thị nội dung";
            // 
            // btnReadContentFromRSS
            // 
            this.btnReadContentFromRSS.Location = new System.Drawing.Point(16, 15);
            this.btnReadContentFromRSS.Name = "btnReadContentFromRSS";
            this.btnReadContentFromRSS.Size = new System.Drawing.Size(136, 23);
            this.btnReadContentFromRSS.TabIndex = 2;
            this.btnReadContentFromRSS.Text = "Read Content From RSS";
            this.btnReadContentFromRSS.UseVisualStyleBackColor = true;
            this.btnReadContentFromRSS.Click += new System.EventHandler(this.btnReadContentFromRSS_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnWriteTwoListUsertoXML);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.button6);
            this.tabPage2.Controls.Add(this.button5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(539, 350);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnWriteTwoListUsertoXML
            // 
            this.btnWriteTwoListUsertoXML.Location = new System.Drawing.Point(185, 20);
            this.btnWriteTwoListUsertoXML.Name = "btnWriteTwoListUsertoXML";
            this.btnWriteTwoListUsertoXML.Size = new System.Drawing.Size(138, 23);
            this.btnWriteTwoListUsertoXML.TabIndex = 4;
            this.btnWriteTwoListUsertoXML.Text = "Ghi ra 2 danh sách user";
            this.btnWriteTwoListUsertoXML.UseVisualStyleBackColor = true;
            this.btnWriteTwoListUsertoXML.Click += new System.EventHandler(this.btnWriteTwoListUsertoXML_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(20, 97);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(240, 150);
            this.dataGridView1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(20, 60);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(112, 23);
            this.button6.TabIndex = 1;
            this.button6.Text = "Đọc dữ liệu từ XML";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(20, 20);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(112, 23);
            this.button5.TabIndex = 0;
            this.button5.Text = "Ghi họ tên ra XML";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(92, 67);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 30;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 400);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGVListGroupRSS)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dGVListGroupRSS;
        private System.Windows.Forms.Label lbListGroupNameAndLinkRSS;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnWriteFromRSS;
        private System.Windows.Forms.Button btnReadContentFromRSS;
        private System.Windows.Forms.Label lbContent;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnWriteTwoListUsertoXML;
        private System.Windows.Forms.Label lbListGroup;
        private System.Windows.Forms.Button btnLoadListGroup;
        private System.Windows.Forms.Button btnBrowseListGroup;
        private System.Windows.Forms.TextBox txtPathListGroup;
        private System.Windows.Forms.Button btnTest;
    }
}


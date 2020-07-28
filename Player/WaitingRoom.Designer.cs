namespace Player
{
    partial class WaitingRoom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitingRoom));
            this.cmbMoney = new System.Windows.Forms.ComboBox();
            this.btnCreateRoom = new System.Windows.Forms.Button();
            this.btnFindRoom = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbMoney
            // 
            this.cmbMoney.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoney.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMoney.FormattingEnabled = true;
            this.cmbMoney.Location = new System.Drawing.Point(131, 359);
            this.cmbMoney.Name = "cmbMoney";
            this.cmbMoney.Size = new System.Drawing.Size(119, 24);
            this.cmbMoney.TabIndex = 1;
            // 
            // btnCreateRoom
            // 
            this.btnCreateRoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateRoom.Location = new System.Drawing.Point(192, 411);
            this.btnCreateRoom.Name = "btnCreateRoom";
            this.btnCreateRoom.Size = new System.Drawing.Size(101, 64);
            this.btnCreateRoom.TabIndex = 3;
            this.btnCreateRoom.Text = "TẠO PHÒNG";
            this.btnCreateRoom.UseVisualStyleBackColor = true;
            this.btnCreateRoom.Click += new System.EventHandler(this.CreateRoom);
            this.btnCreateRoom.MouseLeave += new System.EventHandler(this.btnCreateRoom_MouseLeave);
            this.btnCreateRoom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnCreaRoom_MouseMove);
            // 
            // btnFindRoom
            // 
            this.btnFindRoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindRoom.Location = new System.Drawing.Point(192, 485);
            this.btnFindRoom.Name = "btnFindRoom";
            this.btnFindRoom.Size = new System.Drawing.Size(101, 64);
            this.btnFindRoom.TabIndex = 3;
            this.btnFindRoom.Text = "TÌM PHÒNG";
            this.btnFindRoom.UseVisualStyleBackColor = true;
            this.btnFindRoom.Click += new System.EventHandler(this.btnFindRoom_Click);
            this.btnFindRoom.MouseLeave += new System.EventHandler(this.btnFindRoom_MouseLeave);
            this.btnFindRoom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnFindRoom_MouseMove);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(312, 567);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "1000";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(118, 46);
            this.txtIP.Multiline = true;
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(151, 27);
            this.txtIP.TabIndex = 5;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnConnect.Location = new System.Drawing.Point(275, 46);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 27);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.Connect_Click);
            this.btnConnect.MouseLeave += new System.EventHandler(this.btnConnect_MouseLeave);
            this.btnConnect.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnConnect_MouseMove);
            // 
            // WaitingRoom
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(426, 638);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnFindRoom);
            this.Controls.Add(this.btnCreateRoom);
            this.Controls.Add(this.cmbMoney);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "WaitingRoom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WaitingRoom";
            this.Load += new System.EventHandler(this.WaitingRoom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbMoney;
        private System.Windows.Forms.Button btnCreateRoom;
        private System.Windows.Forms.Button btnFindRoom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button btnConnect;
    }
}
namespace ClientForm
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.OpenButton = new System.Windows.Forms.Button();
            this.InputPort = new System.Windows.Forms.TextBox();
            this.InputIp = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.InputMSG = new System.Windows.Forms.TextBox();
            this.OutputMSG = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(235, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Port : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "IP : ";
            // 
            // OpenButton
            // 
            this.OpenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OpenButton.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenButton.Location = new System.Drawing.Point(348, 30);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(77, 31);
            this.OpenButton.TabIndex = 2;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // InputPort
            // 
            this.InputPort.Font = new System.Drawing.Font("굴림", 13F);
            this.InputPort.Location = new System.Drawing.Point(238, 30);
            this.InputPort.Multiline = true;
            this.InputPort.Name = "InputPort";
            this.InputPort.Size = new System.Drawing.Size(104, 31);
            this.InputPort.TabIndex = 1;
            this.InputPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputPort_KeyDown);
            // 
            // InputIp
            // 
            this.InputIp.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.InputIp.Font = new System.Drawing.Font("굴림", 13F);
            this.InputIp.Location = new System.Drawing.Point(13, 31);
            this.InputIp.Multiline = true;
            this.InputIp.Name = "InputIp";
            this.InputIp.Size = new System.Drawing.Size(219, 31);
            this.InputIp.TabIndex = 0;
            // 
            // SendButton
            // 
            this.SendButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SendButton.FlatAppearance.BorderSize = 0;
            this.SendButton.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendButton.Location = new System.Drawing.Point(343, 567);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(81, 35);
            this.SendButton.TabIndex = 4;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = false;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // InputMSG
            // 
            this.InputMSG.Font = new System.Drawing.Font("굴림", 13F);
            this.InputMSG.Location = new System.Drawing.Point(12, 567);
            this.InputMSG.Multiline = true;
            this.InputMSG.Name = "InputMSG";
            this.InputMSG.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InputMSG.Size = new System.Drawing.Size(325, 35);
            this.InputMSG.TabIndex = 3;
            this.InputMSG.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputMsg_KeyDown);
            this.InputMSG.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.InputMsg_KeyPreview);
            // 
            // OutputMSG
            // 
            this.OutputMSG.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.OutputMSG.Cursor = System.Windows.Forms.Cursors.No;
            this.OutputMSG.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OutputMSG.Location = new System.Drawing.Point(12, 67);
            this.OutputMSG.Multiline = true;
            this.OutputMSG.Name = "OutputMSG";
            this.OutputMSG.ReadOnly = true;
            this.OutputMSG.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutputMSG.Size = new System.Drawing.Size(412, 487);
            this.OutputMSG.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(436, 615);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.InputPort);
            this.Controls.Add(this.InputIp);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.InputMSG);
            this.Controls.Add(this.OutputMSG);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Cipher-Chat (Client)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.TextBox InputPort;
        private System.Windows.Forms.TextBox InputIp;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox InputMSG;
        private System.Windows.Forms.TextBox OutputMSG;
    }
}


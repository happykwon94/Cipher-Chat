namespace ChattingForm
{
    partial class ChatForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatForm));
            this.OutputMSG = new System.Windows.Forms.TextBox();
            this.InputMSG = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.InputIp = new System.Windows.Forms.TextBox();
            this.InputPort = new System.Windows.Forms.TextBox();
            this.OpenButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OutputMSG
            // 
            this.OutputMSG.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.OutputMSG.Enabled = false;
            this.OutputMSG.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OutputMSG.Location = new System.Drawing.Point(12, 67);
            this.OutputMSG.Multiline = true;
            this.OutputMSG.Name = "OutputMSG";
            this.OutputMSG.ReadOnly = true;
            this.OutputMSG.Size = new System.Drawing.Size(412, 488);
            this.OutputMSG.TabIndex = 0;
            // 
            // InputMSG
            // 
            this.InputMSG.Font = new System.Drawing.Font("굴림", 13F);
            this.InputMSG.Location = new System.Drawing.Point(12, 568);
            this.InputMSG.Multiline = true;
            this.InputMSG.Name = "InputMSG";
            this.InputMSG.Size = new System.Drawing.Size(325, 35);
            this.InputMSG.TabIndex = 4;
            this.InputMSG.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputMsg_KeyDown);
            // 
            // SendButton
            // 
            this.SendButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SendButton.FlatAppearance.BorderSize = 0;
            this.SendButton.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendButton.Location = new System.Drawing.Point(343, 568);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(81, 35);
            this.SendButton.TabIndex = 5;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = false;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // InputIp
            // 
            this.InputIp.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.InputIp.Font = new System.Drawing.Font("굴림", 13F);
            this.InputIp.Location = new System.Drawing.Point(13, 31);
            this.InputIp.Multiline = true;
            this.InputIp.Name = "InputIp";
            this.InputIp.Size = new System.Drawing.Size(219, 30);
            this.InputIp.TabIndex = 1;
            // 
            // InputPort
            // 
            this.InputPort.Font = new System.Drawing.Font("굴림", 13F);
            this.InputPort.Location = new System.Drawing.Point(238, 30);
            this.InputPort.Multiline = true;
            this.InputPort.Name = "InputPort";
            this.InputPort.Size = new System.Drawing.Size(104, 30);
            this.InputPort.TabIndex = 2;
            this.InputPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputPort_KeyDown);
            // 
            // OpenButton
            // 
            this.OpenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OpenButton.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenButton.Location = new System.Drawing.Point(348, 30);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(77, 31);
            this.OpenButton.TabIndex = 3;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "IP : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(235, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Port : ";
            // 
            // ChatForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(436, 615);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.InputPort);
            this.Controls.Add(this.InputIp);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.InputMSG);
            this.Controls.Add(this.OutputMSG);
            this.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChatForm";
            this.Text = "Cipher-Chat (Server)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox OutputMSG;
        private System.Windows.Forms.TextBox InputMSG;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox InputPort;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox InputIp;
    }
}


namespace ClientForm
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.ChatNameLabel = new System.Windows.Forms.Label();
            this.InpucChatNameButton = new System.Windows.Forms.Button();
            this.InputChatName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ChatNameLabel
            // 
            this.ChatNameLabel.AutoSize = true;
            this.ChatNameLabel.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChatNameLabel.Location = new System.Drawing.Point(14, 20);
            this.ChatNameLabel.Name = "ChatNameLabel";
            this.ChatNameLabel.Size = new System.Drawing.Size(102, 17);
            this.ChatNameLabel.TabIndex = 8;
            this.ChatNameLabel.Text = "Chat-Name : ";
            // 
            // InpucChatNameButton
            // 
            this.InpucChatNameButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InpucChatNameButton.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InpucChatNameButton.Location = new System.Drawing.Point(141, 86);
            this.InpucChatNameButton.Name = "InpucChatNameButton";
            this.InpucChatNameButton.Size = new System.Drawing.Size(79, 29);
            this.InpucChatNameButton.TabIndex = 7;
            this.InpucChatNameButton.Text = "Setting";
            this.InpucChatNameButton.UseVisualStyleBackColor = true;
            this.InpucChatNameButton.Click += new System.EventHandler(this.InputPwdButton_Click);
            // 
            // InputChatName
            // 
            this.InputChatName.Font = new System.Drawing.Font("굴림", 13F);
            this.InputChatName.Location = new System.Drawing.Point(14, 40);
            this.InputChatName.Multiline = true;
            this.InputChatName.Name = "InputChatName";
            this.InputChatName.Size = new System.Drawing.Size(206, 27);
            this.InputChatName.TabIndex = 6;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 134);
            this.Controls.Add(this.ChatNameLabel);
            this.Controls.Add(this.InpucChatNameButton);
            this.Controls.Add(this.InputChatName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chat-Name_Set";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ChatNameLabel;
        private System.Windows.Forms.Button InpucChatNameButton;
        private System.Windows.Forms.TextBox InputChatName;
    }
}
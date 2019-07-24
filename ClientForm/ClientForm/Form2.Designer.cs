namespace ClientForm
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.Pwd = new System.Windows.Forms.Label();
            this.InputPwdButton = new System.Windows.Forms.Button();
            this.InputPwd = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Pwd
            // 
            this.Pwd.AutoSize = true;
            this.Pwd.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pwd.Location = new System.Drawing.Point(14, 20);
            this.Pwd.Name = "Pwd";
            this.Pwd.Size = new System.Drawing.Size(93, 17);
            this.Pwd.TabIndex = 5;
            this.Pwd.Text = "Password : ";
            // 
            // InputPwdButton
            // 
            this.InputPwdButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InputPwdButton.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputPwdButton.Location = new System.Drawing.Point(141, 86);
            this.InputPwdButton.Name = "InputPwdButton";
            this.InputPwdButton.Size = new System.Drawing.Size(79, 29);
            this.InputPwdButton.TabIndex = 4;
            this.InputPwdButton.Text = "Input";
            this.InputPwdButton.UseVisualStyleBackColor = true;
            this.InputPwdButton.Click += new System.EventHandler(this.InputPwdButton_Click);
            // 
            // InputPwd
            // 
            this.InputPwd.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.InputPwd.Location = new System.Drawing.Point(14, 40);
            this.InputPwd.Multiline = true;
            this.InputPwd.Name = "InputPwd";
            this.InputPwd.Size = new System.Drawing.Size(206, 27);
            this.InputPwd.TabIndex = 3;
            this.InputPwd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputPwd_KeyDown);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 134);
            this.Controls.Add(this.Pwd);
            this.Controls.Add(this.InputPwdButton);
            this.Controls.Add(this.InputPwd);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Pwd;
        private System.Windows.Forms.Button InputPwdButton;
        private System.Windows.Forms.TextBox InputPwd;
    }
}
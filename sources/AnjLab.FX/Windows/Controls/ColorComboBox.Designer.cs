namespace ColorComboTestApp
{
    partial class ColorComboBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button = new ColorComboButton();
    
            this.SuspendLayout();
            // 
            // ColorComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ColorComboBox";
            this.Size = new System.Drawing.Size(103, 23);
            this.SizeChanged += new System.EventHandler(this.ColorComboBox_SizeChanged);
            Controls.Add(button);
            this.ResumeLayout(false);

        }

        #endregion
        private ColorComboButton button;
    }
}
